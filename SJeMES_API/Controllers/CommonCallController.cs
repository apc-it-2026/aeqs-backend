using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SJeMES_Framework_NETCore.WebAPI;
using System.IO;
using Microsoft.AspNetCore.Http;
using SJ_QCMAPI.Common;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace SJeMES_API.Controllers
{

    [Route("api/[controller]")]
    public class CommonCallController : Controller
    {
         
        [HttpGet]
        public string Get()
        {
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string s = Newtonsoft.Json.JsonConvert.SerializeObject("欢迎使用广东商基Web API"); 

            return s;
        }


        [HttpPost]
        public string Post([FromBody]object parameter)
        {
            string lanauage = Request.Headers["lanauage"];
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.WebAPI.RequestObject p = new SJeMES_Framework_NETCore.WebAPI.RequestObject();

            string guid = string.Empty;
            SJeMES_Framework_NETCore.WebAPI.RequestObject.CurrentLanguage = lanauage;


            try
            {
                //var p = ReqObj.ToObj(parameter);
                try
                {
                    p = Newtonsoft.Json.JsonConvert.DeserializeObject<SJeMES_Framework_NETCore.WebAPI.RequestObject>(parameter.ToString());
                }
                catch (Exception)
                {
                    string RASKey = string.Empty;
                    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                    RASKey = DB.GetString(@"SELECT Key2 FROM rasinfo");

                    string data = SJeMES_Framework_NETCore.Common.RASHelper.RASDecrypt(parameter.ToString(), RASKey);
                    p = Newtonsoft.Json.JsonConvert.DeserializeObject<SJeMES_Framework_NETCore.WebAPI.RequestObject>(data);
                }

                bool tokenContinue = false;//是否跳过token校验
                List<string> continueClassNameList = new List<string>()
                {
                    "SJ_BDMAPI.UILAN_APP".ToLower()
                };
                if (continueClassNameList.Contains(p.ClassName.ToLower()))
                    tokenContinue = true;

                string UserToken = p.UserToken;
                if (!string.IsNullOrEmpty(UserToken) 
                    && !p.Method.Equals("GetPdaMultilingual")//获取多语言信息方法
                    && !p.Method.Equals("PDALogin")//登录方法
                    && !tokenContinue)
                {
                    string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(UserToken);
                    SJeMES_Framework_NETCore.WebAPI.RequestObject.CurrentUserCode = usercode;
                }

                //guid = SJeMES_Framework_NETCore.Web.System.Log(p);



                Assembly assembly = null;




#if DEBUG
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                assembly = Assembly.LoadFrom(path + @"\" + p.DllName + ".dll");
#else
                    assembly = Assembly.LoadFrom(p.DllName + ".dll");
#endif






                Type type = assembly.GetType(p.ClassName);

                object instance = null;


                instance = Activator.CreateInstance(type);


                MethodInfo mi = type.GetMethod(p.Method);


                object[] args = new object[1];

                args[0] = p;
                bool istrue = SJeMES_Framework_NETCore.Web.System.checkAction(args[0]);
                if (istrue)
                {
                    object obj = mi.Invoke(instance, args);
                    ret = (SJeMES_Framework_NETCore.WebAPI.ResultObject)obj;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No operation right now！";
                }
                if (p.IsRasResult)
                {
                    ret.RetData = SJeMES_Framework_NETCore.Common.RASHelper.RASEncryption(p.RasResultKey, ret.RetData);
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;

            }
            try
            {
                if (!string.IsNullOrEmpty(ret.ErrMsg))
                {
                    string ss = Regex.Replace(ret.ErrMsg, "[0-9]", "*");
                    for (int i = 0; i < 6; i++)
                    {
                        ss = ss.Replace("**", "*");
                    }
                    string msg = SJeMES_Framework_NETCore.Common.UIHelper.UImsg(ss, lanauage, "http://" + Request.Host.Value + "/api/CommonCall", p.UserToken);
                    var placeStr = Regex.Replace(ret.ErrMsg, "[^0-9]", "*");
                    for (int i = 0; i < 6; i++)
                    {
                        placeStr = placeStr.Replace("**", "*");
                    }
                    String[] ss2 = msg.Split('*');
                    String ss4 = string.Empty;
                    int A = 0;
                    for (int i = 0; i < placeStr.Length; i++)
                    {
                        int n = 0;
                        for (int a = 0; a < ss2.Length; a++)
                        {

                            if (placeStr[i].ToString() == "*")
                            {
                                A++;
                                ss4 += ss2[a + A];
                                n++;
                                break;
                            }
                        }
                        if (n == 0)
                        {
                            ss4 += placeStr[i].ToString();
                        }
                    }
                    ret.ErrMsg = ss4;

                }
            }
            catch (Exception)
            {

            }

            string retjson = Newtonsoft.Json.JsonConvert.SerializeObject(ret);
            return retjson;
        }

        [HttpPost("UploadCommon")]
        public async Task<UploadFileResultDto> UploadCommon(string usertoken)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            Dictionary<string, object> userDIC = null;
            if (string.IsNullOrEmpty(usertoken))
            {
                result.IsSuccess = false;
                result.ErrMsg = "The usertoken parameter is empty";
                return result;
            }
            else
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                if (userDIC == null || userDIC.Count == 0)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "invalid usertoken";
                    return result;
                }
            }

            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                result.IsSuccess = false;
                result.ErrMsg = "file cannot be empty";
                return result;
            }

            var stream = file.OpenReadStream();
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var fileSuffix = file.FileName.Split(".").Last();
            string filename = file.FileName;

            string staticRoot = @"/wwwroot";
            string BasePath = Directory.GetCurrentDirectory() + staticRoot;
            string filePath = $@"/uploadfile/{userDIC["CompanyCode"]}/{DateTime.Now:yyyy-MM-dd}";
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileSuffix;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });
            string guid = Guid.NewGuid().ToString("N");

            DB.ExecuteNonQueryOffline($@"insert into BDM_UPLOAD_FILE_ITEM(FILE_NAME,FILE_URL,createby,createdate,createtime,guid,SUFFIX)
                    values('{filename}','{filePath + '/' + NewFileName}','{userDIC["UserCode"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','{guid}','{fileSuffix}')");

            if (!Directory.Exists(BasePath + filePath))
            {
                Directory.CreateDirectory(BasePath + filePath);
            }

            var url = $"{BasePath}/{filePath}/{NewFileName}";
            using (var fs = System.IO.File.Create(url))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            result.IsSuccess = true;
            result.ReturnObj = new { guid, url = $"{filePath + '/' + NewFileName}" };
            return result;
        }

        /// <summary>
        /// 迁移nfs文件到wwwroot文件夹内
        /// </summary>
        /// <param name="usertoken"></param>
        /// <param name="nfsFilePath"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpPost("MoveNfsFile")]
        public async Task<UploadFileResultDto> MoveNfsFile(string usertoken, string nfsFilePath, string companyCode)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            try
            {
                Dictionary<string, object> userDIC = null;
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                #region 校验token
                if (string.IsNullOrEmpty(usertoken))
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "usertoken参数为空";
                    return result;
                }
                else
                {

                    userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                    if (userDIC == null || userDIC.Count == 0)
                    {
                        result.IsSuccess = false;
                        result.ErrMsg = "无效usertoken";
                        return result;
                    }
                }
                #endregion

                #region 迁移文件
                var nfspath = SysDB.GetString($@"
SELECT
	nfspath
FROM
	[dbo].[SYSORG01M]
WHERE
	UPPER (org) = '{companyCode.ToUpper()}';");
                string nfsFileFullPath = $@"{nfspath}{nfsFilePath}".Replace("/", @"\");//nfs完整路径

                var filepath = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot";
                if (!System.IO.Directory.Exists(filepath + nfsFilePath.Substring(0, nfsFilePath.LastIndexOf(@"/") + 1)))
                {
                    //不存在就创建文件
                    System.IO.Directory.CreateDirectory(filepath + nfsFilePath.Substring(0, nfsFilePath.LastIndexOf(@"/") + 1));
                }
                string file = filepath + nfsFilePath.Replace("/", @"\");

                // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                if (!System.IO.File.Exists(file))
                {
                    System.IO.File.Copy(nfsFileFullPath, file);
                }
                else
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                    System.IO.File.Copy(nfsFileFullPath, file);
                }
                #endregion

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrMsg = ex.Message;
                return result;
            }
        }

        [HttpGet("GetUploadFileMaxLength")]
        public async Task<long> GetUploadFileMaxLength(string usertoken)
        {
            long maxLength = -1;
            try
            {
                string webconfigStr = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                    XDocument doc = XDocument.Load(path + @"\web.config");
                    maxLength = Convert.ToInt64(doc.Element("configuration").Element("location").Element("system.webServer").Element("security").Element("requestFiltering").Element("requestLimits").Attribute("maxAllowedContentLength").Value);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    XDocument doc = XDocument.Load("web.config");
                    maxLength = Convert.ToInt64(doc.Element("configuration").Element("location").Element("system.webServer").Element("security").Element("requestFiltering").Element("requestLimits").Attribute("maxAllowedContentLength").Value);
                }
            }
            catch (Exception ex)
            {
            }
            return maxLength;
        }

        //[HttpPost("UploadCommon_ftp")]
        //public async Task<UploadFileResultDto> UploadCommon_ftp(string usertoken)
        //{
        //    UploadFileResultDto result = new UploadFileResultDto();
        //    Dictionary<string, object> userDIC = null;
        //    if (string.IsNullOrEmpty(usertoken))
        //    {
        //        result.IsSuccess = false;
        //        result.ErrMsg = "usertoken参数为空";
        //        return result;
        //    }
        //    else
        //    {
        //        SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

        //        userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
        //        if (userDIC == null || userDIC.Count == 0)
        //        {
        //            result.IsSuccess = false;
        //            result.ErrMsg = "无效usertoken";
        //            return result;
        //        }
        //    }

        //    var file = Request.Form.Files.FirstOrDefault();
        //    if (file == null)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrMsg = "文件不能为空";
        //        return result;
        //    }





        //    var stream = file.OpenReadStream();
        //    var data = new byte[stream.Length];
        //    stream.Read(data, 0, data.Length);
        //    var fileSuffix = file.FileName.Split(".").Last();
        //    string filename = file.FileName;

        //    string staticRoot = @"/wwwroot";
        //    string BasePath = Directory.GetCurrentDirectory() + staticRoot;
        //    string filePath = $@"/uploadfile/{DateTime.Now:yyyy-MM-dd}";
        //    string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileSuffix;
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });
        //    string guid = Guid.NewGuid().ToString("N");

        //    string ftp_user = "ftp_user";
        //    string ftp_pw = "sj#123465";
        //    var ftpIp = "ftp://112.74.49.34:21";


        //    var ftpUrl = ftpIp + filePath;

        //    //替换符号
        //    ftpUrl = ftpUrl.Replace("\\", "/");

        //    //var newFileName = $"{filename}.{fileSuffix}";

        //    //组合ftp上传文件路径
        //    string uri = ftpUrl + "/";

        //    FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri + NewFileName));

        //    FtpCheckDirectoryExist(filePath, ftpIp, ftp_user, ftp_pw);

        //    // 指定数据传输类型
        //    reqFTP.UseBinary = true;

        //    // ftp用户名和密码
        //    reqFTP.Credentials = new NetworkCredential(ftp_user, ftp_pw);

        //    // 默认为true，连接不会被关闭
        //    // 在一个命令之后被执行
        //    reqFTP.KeepAlive = false;

        //    // 指定执行什么命令
        //    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

        //    // 上传文件时通知服务器文件的大小
        //    reqFTP.ContentLength = stream.Length;

        //    // 缓冲大小设置为kb
        //    int buffLength = 2048;

        //    byte[] buff = new byte[buffLength];

        //    int contentLen;
        //    try
        //    {
        //        // 把上传的文件写入流
        //        Stream strm = reqFTP.GetRequestStream();

        //        // 每次读文件流的kb
        //        contentLen = stream.Read(buff, 0, buffLength);

        //        // 流内容没有结束
        //        while (contentLen != 0)
        //        {
        //            // 把内容从file stream 写入upload stream
        //            strm.Write(buff, 0, contentLen);
        //            contentLen = stream.Read(buff, 0, buffLength);
        //        }
        //        // 关闭流
        //        strm.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrMsg = "上传失败";
        //        return result;
        //    }

        //    DB.ExecuteNonQueryOffline($@"insert into BDM_UPLOAD_FILE_ITEM(FILE_NAME,FILE_URL,createby,createdate,createtime,guid,SUFFIX)
        //            values('{filename}','{filePath + '/' + NewFileName}','{userDIC["UserCode"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','{guid}','{fileSuffix}')");

        //    //if (!Directory.Exists(BasePath + filePath))
        //    //{
        //    //    Directory.CreateDirectory(BasePath + filePath);
        //    //}

        //   // var url = $"{BasePath}/{filePath}/{NewFileName}";
        //    //using (var fs = System.IO.File.Create(url))
        //    //{
        //    //    file.CopyTo(fs);
        //    //    fs.Flush();
        //    //}

        //    result.IsSuccess = true;
        //    result.ReturnObj = new { guid, url = $"{filePath + '/' + NewFileName}" };
        //    return result;
        //}

        //判断文件的目录是否存,不存则创建  
        public static void FtpCheckDirectoryExist(string destFilePath, string ftpIp, string user, string pwd)
        {
            string[] dirs = destFilePath.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        FtpMakeDir(curDir.Substring(0, curDir.Length - 1), ftpIp, user, pwd);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        //创建目录  
        public static Boolean FtpMakeDir(string localFile, string ftpIp, string user, string pwd)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpIp + localFile);
            req.Credentials = new NetworkCredential(user, pwd);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }

        /// <summary>
        /// 平板上传图片文件接口
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<UploadFileResultDto> Upload(int type,string usertoken,string code)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            Dictionary<string, object> userDIC = null;
            if (string.IsNullOrEmpty(usertoken))
            {
                result.IsSuccess = false;
                result.ErrMsg = "usertoken参数为空";
                return result;
            }
            else
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                if(userDIC==null||userDIC.Count==0)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "invalid usertoken";
                    return result;
                }
            }

         
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                result.IsSuccess = false;
                result.ErrMsg = "file cannot be empty";
                return result;
            }

            var stream = file.OpenReadStream();
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var fileSuffix = file.FileName.Split(".").Last();
            string filename = file.FileName;

            string staticRoot = @"/wwwroot";
            string BasePath= Directory.GetCurrentDirectory()+ staticRoot;
            string filePath = "";
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileSuffix;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken= usertoken });
            string guid = Guid.NewGuid().ToString("N");
            switch (type)
            {
                case 1://实验室送测上传图片
                    filePath = enum_filepath.enum_filepath_1;
                    //业务逻辑代码
                    
                    var codelist = code.Split('@').ToList();
                    DB.ExecuteNonQueryOffline($@"insert into QCM_INSPECTION_LABORATORY_IMAGEURL(inspection_no,testitem_code,testitem_code_seq,img_name,img_url,createby,createdate,createtime,guid)
                    values('{codelist[0]}','{codelist[1]}','{codelist[2]}','{filename}','{filePath+'/'+ NewFileName}','{userDIC["UserCode"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','{guid}')");
                    break;
                case 2://金属检验上传图片
                    filePath = enum_filepath.enum_filepath_6;
                    //业务逻辑代码 
                    Dictionary<string, object> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(code);
                    var codelist2 = dic["po_order"].ToString();
                    DB.ExecuteNonQueryOffline($@"insert into qcm_inspection_metal_image (po_order,img_name,img_url,guid,createby,createdate,createtime) 
                    values('{codelist2[0]}','{filename}','{filePath}','{guid}','{userDIC["UserCode"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')");
                    break;
                case 3://色卡厂商
                    filePath = enum_filepath.enum_filepath_7;
                    //业务逻辑代码 
                    Dictionary<string, object> dic2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(code);
                    var CARD_DATE = dic2["CARD_DATE"].ToString();
                    var VEND_NO = dic2["VEND_NO"].ToString();
                    var SHOE_NO = dic2["SHOE_NO"].ToString();
                    var PROD_NO = dic2["PROD_NO"].ToString();
                    var APTESTITEM_CODE = dic2["APTESTITEM_CODE"].ToString();

                    DB.ExecuteNonQueryOffline($@"
INSERT INTO QCM_COLOR_CARD_IMAGE (
	CARD_DATE,
	VEND_NO,
	SHOE_NO,
    PROD_NO,
    APTESTITEM_CODE,
    IMG_NAME,
    IMG_URL,
	GUID,
	CREATEBY,
	CREATEDATE,
	CREATETIME
)
VALUES
	(
		'{CARD_DATE}',
        '{VEND_NO}',
        '{SHOE_NO}',
        '{PROD_NO}',
        '{APTESTITEM_CODE}',
		'{filename}',
		'{filePath}',
		'{guid}',
		'{userDIC["UserCode"]}',
		'{DateTime.Now.ToString("yyyy-MM-dd")}',
		'{DateTime.Now.ToString("HH:mm:ss")}'
	)");
                    break;

            }
            if (!Directory.Exists(BasePath+ filePath))
            {
                Directory.CreateDirectory(BasePath + filePath);
            }

            var url = $"{BasePath}/{filePath}/{NewFileName}";
            using (var fs = System.IO.File.Create(url))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            result.IsSuccess = true;
            result.ReturnObj = new { guid,url= $"{filePath+'/'+ NewFileName}" };
            return result;
        }

        /// <summary>
        /// 平板或桌面 上传图片/文件接口
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost("Upload_PDA")]
        public async Task<UploadFileResultDto> Upload_PDA(string usertoken, int type)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            Dictionary<string, object> userDIC = null;
            if (string.IsNullOrEmpty(usertoken))
            {
                result.IsSuccess = false;
                result.ErrMsg = "The usertoken parameter is empty";
                return result;
            }
            else
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                if (userDIC == null || userDIC.Count == 0)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "无效usertoken";
                    return result;
                }
            }


            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                result.IsSuccess = false;
                result.ErrMsg = "文件不能为空";
                return result;
            }

            var stream = file.OpenReadStream();
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var fileSuffix = file.FileName.Split(".").Last();
            string filename = file.FileName;

            string staticRoot = @"/wwwroot";
            string BasePath = Directory.GetCurrentDirectory() + staticRoot;
            
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileSuffix;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });
            string guid = Guid.NewGuid().ToString("N");

            var enum_filepath= (enum_filepath1)type;
            string remark = enum_filepath.GetRemark();
            string filePath = enum_filepath.GetImagePath();
            string tabname = enum_filepath.GetTableName();
            //switch (type)
            //{
            //    case (int)enum_filepath1.enum_filepath_1://实验室送测上传图片
            //        filePath = enum_filepath1.enum_filepath_1.GetImagePath();
            //        break;
            //    case 2://金属检验上传图片
            //        filePath = enum_filepath.enum_filepath_6;
            //        break;
            //}


            if (!Directory.Exists(BasePath + filePath))
            {
                Directory.CreateDirectory(BasePath + filePath);
            }

            var url = $"{BasePath}/{filePath}/{NewFileName}";
            using (var fs = System.IO.File.Create(url))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            result.IsSuccess = true;
            result.ReturnObj = new { filename, url = $"{filePath + '/' + NewFileName}" };
            return result;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost("DeleteFile")]
        public async Task<UploadFileResultDto> DeleteFile(int type, string usertoken, string guid)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            Dictionary<string, object> userDIC = null;
            if (string.IsNullOrEmpty(usertoken))
            {
                result.IsSuccess = false;
                result.ErrMsg = "usertoken参数为空";
                return result;
            }
            else
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                if (userDIC == null || userDIC.Count == 0)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "无效usertoken";
                    return result;
                }
            }
            string BasePath = Directory.GetCurrentDirectory();
            string staticRoot = @"/wwwroot";
            string fileurl = "";
            switch (type)
            {
                case (int)enum_filepath1.enum_filepath_1:
                    //业务逻辑代码
                    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });
                    fileurl = DB.GetString($"select img_url from QCM_INSPECTION_LABORATORY_IMAGEURL where guid='{guid}'");
                    DB.ExecuteNonQueryOffline($"delete from QCM_INSPECTION_LABORATORY_IMAGEURL where guid='{guid}'");
                    break;
            }
            try
            {
                System.IO.File.Delete(BasePath +"/"+ staticRoot + fileurl);
            }
            catch
            {

            }
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 删除文件(PDA)
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost("DeleteFile_PDA")]
        public async Task<UploadFileResultDto> DeleteFile_PDA(int type, string usertoken, string url)
        {
            UploadFileResultDto result = new UploadFileResultDto();
            Dictionary<string, object> userDIC = null;
            if (string.IsNullOrEmpty(usertoken))
            {
                result.IsSuccess = false;
                result.ErrMsg = "usertoken参数为空";
                return result;
            }
            else
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
                if (userDIC == null || userDIC.Count == 0)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = "无效usertoken";
                    return result;
                }
            }

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });

            string BasePath = Directory.GetCurrentDirectory();
            string staticRoot = @"/wwwroot";
            var enum_filepath = (enum_filepath1)type;
            string tablename = enum_filepath.GetTableName();
            string img_paht = enum_filepath.GetImagePath();

            try
            {
                if(!string.IsNullOrEmpty(tablename))
                {
                    DB.ExecuteNonQueryOffline($"delete from {tablename} where img_url='{img_paht + url.Substring(url.LastIndexOf(@"/"))}'");
                }
                System.IO.File.Delete(BasePath + "/" + staticRoot + img_paht + url.Substring(url.LastIndexOf(@"/")));
            }
            catch
            {

            }
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// Winfrom上传图片
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>		Form	error CS0103: 当前上下文中不存在名称“Form”	

        [HttpPost("UploadIMG")]
        public async Task<UploadFileResultDto> UploadIMG(IFormFile file,string p)
        {
            UploadFileResultDto result = new UploadFileResultDto();

            #region 接口参数 
            string usertoken = string.Empty;
            string type = string.Empty;//类型
            Dictionary<string, object> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(p);
            if (file == null)
            {
                result.IsSuccess = false;
                result.ErrMsg = "文件不能为空";
                return result;
            }
            if (dic.Count == 0 || !dic.ContainsKey("usertoken") || !dic.ContainsKey("type"))
            {
                result.IsSuccess = false;
                result.ErrMsg = "接口缺少[usertoken]或[type]参数，请检查！";
                return result;
            }

            usertoken = dic["usertoken"].ToString();
            type = dic["type"].ToString();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(new RequestObject { UserToken = usertoken });
            SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            var userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{usertoken}'");
            string createby = userDIC["UserCode"].ToString();
            string createdate = DateTime.Now.ToString("yyyy-MM-dd");
            string createtime = DateTime.Now.ToString("HH:mm:ss");
            #endregion
             
            var stream = file.OpenReadStream();
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var fileSuffix = file.FileName.Split(".").Last();
            string filename = file.FileName;
            string staticRoot = @"/wwwroot";
            string filePath = string.Empty;
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileSuffix;
            string guid = Guid.NewGuid().ToString("N");
            switch (type)
            {
                case "1"://ART的图片上传  
                    { 
                        filePath = enum_filepath.enum_filepath_2;
                        string prod_no = dic.ContainsKey("prod_no") ? dic["prod_no"].ToString() : "";

                        string img = DB.GetString($"select img_url from bdm_prod_m where prod_no='{prod_no}'");
                        if (!string.IsNullOrEmpty(img) || img != "")//查找旧图片，如果存在删除
                        {
                            try
                            {
                                string old_imgurl = Directory.GetCurrentDirectory() + staticRoot + img;
                                System.IO.File.Delete($"{old_imgurl}");
                            }
                            catch
                            { 
                            }
                        }
                        DB.ExecuteNonQueryOffline($@"UPDATE bdm_prod_m SET img_url='{ filePath + '/' + NewFileName}',guid='{guid}',
                                                        MODIFYBY='{createby}',MODIFYDATE='{createdate}',MODIFYTIME='{createtime}' 
                                                     where prod_no='{prod_no}'");

                    }
                    break;
                case "2"://ART定制检验项目文件路径
                    {
                        filePath = enum_filepath.enum_filepath_3;
                        string prod_no = dic.ContainsKey("prod_no") ? dic["prod_no"].ToString() : "";
                        string general_testtype_no = dic.ContainsKey("general_testtype_no") ? dic["general_testtype_no"].ToString() : "";
                        string category_no = dic.ContainsKey("category_no") ? dic["category_no"].ToString() : "";
                        string testitem_code = dic.ContainsKey("testitem_code") ? dic["testitem_code"].ToString() : "";
                        string ImgName = dic.ContainsKey("ImgName") ? dic["ImgName"].ToString() : "";
                        DB.ExecuteNonQueryOffline($@"
                                insert into bdm_prod_customquality_file(prod_no,general_testtype_no,category_no,testitem_code,file_name,file_url,guid,CREATEBY,CREATEDATE,CREATETIME)
                                VALUES('{prod_no}','{general_testtype_no}','{category_no}','{testitem_code}','{ImgName}','{ filePath + '/' + NewFileName}','{guid}','{createby}','{createdate}','{createtime}')");
                    }
                    break;
                case "3"://QA鞋型品质上传文件
                    {
                        filePath = enum_filepath.enum_filepath_5;

                        string develop_season = dic.ContainsKey("develop_season") ? dic["develop_season"].ToString() : "";
                        string shoe_no = dic.ContainsKey("shoe_no") ? dic["shoe_no"].ToString() : "";
                        string file_type = dic.ContainsKey("file_type") ? dic["file_type"].ToString() : "";
                        string ImgName = dic.ContainsKey("ImgName") ? dic["ImgName"].ToString() : "";
                        DB.ExecuteNonQueryOffline($@"
                                insert into qcm_qa_shoeshape_file(develop_season,shoe_no,file_type,file_name,file_url,guid,CREATEBY,CREATEDATE,CREATETIME)
                                VALUES('{develop_season}','{shoe_no}','{file_type}','{ImgName}','{ filePath + '/' + NewFileName}','{guid}','{createby}','{createdate}','{createtime}')");
                    }
                    break;
                case "4"://QA鞋型品质图片上传
                    {
                        filePath = enum_filepath.enum_filepath_4;

                        string develop_season = dic.ContainsKey("develop_season") ? dic["develop_season"].ToString() : "";
                        string shoe_no = dic.ContainsKey("shoe_no") ? dic["shoe_no"].ToString() : "";
                        string check_date = dic.ContainsKey("check_date") ? dic["check_date"].ToString() : "";
                        string dpstage_code = dic.ContainsKey("dpstage_code") ? dic["dpstage_code"].ToString() : "";
                        string problemcategory_no = dic.ContainsKey("problemcategory_no") ? dic["problemcategory_no"].ToString() : "";
                        string problem_no = dic.ContainsKey("problem_no") ? dic["problem_no"].ToString() : "";
                        string img_name = dic.ContainsKey("img_name") ? dic["img_name"].ToString() : "";
                        DB.ExecuteNonQueryOffline($@"insert into qcm_qa_shoeshape_image (develop_season,shoe_no,check_date,dpstage_code,problemcategory_no,problem_no,img_name,img_url,guid,createby,createdate,createtime) 
                                                     values('{develop_season}','{shoe_no}','{check_date}','{dpstage_code}','{problemcategory_no}','{problem_no}','{img_name}','{filePath + '/' + NewFileName}','{guid}','{createby}','{createdate}','{createtime}')");
                    }
                    break;
                case "5"://客户投诉图片上传
                    {
                        
                        

                        string Ispic = dic.ContainsKey("Ispic") ? dic["Ispic"].ToString() : "";//图片/文件

                        if(Ispic == "1")
                            filePath = enum_filepath.enum_filepath_9;
                        else
                            filePath = enum_filepath.enum_filepath_10;

                        string COMPLAINT_NO = dic.ContainsKey("COMPLAINT_NO") ? dic["COMPLAINT_NO"].ToString() : "";//投诉编号
                        string IMG_NAME = dic.ContainsKey("IMG_NAME") ? dic["IMG_NAME"].ToString() : "";//照片名称
                        string IMG_URL = dic.ContainsKey("IMG_URL") ? dic["IMG_URL"].ToString() : "";
                       // string Type = "1"; 图片类型
                        
                        DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_CUSTOMER_COMPLAINT_FILE (COMPLAINT_NO,IMG_NAME,IMG_URL,GUID,TYPE,CREATEBY,CREATEDATE,CREATETIME)VALUES('{COMPLAINT_NO}','{IMG_NAME}','{filePath + '/' + NewFileName}','{guid}','{Ispic}','{createby}','{createdate}','{createtime}')");
                    }
                    break;
            }

            string BasePath = Directory.GetCurrentDirectory() + staticRoot;
            if (!Directory.Exists(BasePath + filePath))
            {
                Directory.CreateDirectory(BasePath + filePath);
            }
            var url = $"{BasePath}/{filePath}/{NewFileName}";
            using (var fs = System.IO.File.Create(url))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            result.IsSuccess = true;
            result.ReturnObj = new {url = $"{filePath + '/' + NewFileName}" };
            return result;
        }





        /// <summary>
        /// 上传文件路径（作废弃用）
        /// </summary>
        public class enum_filepath
        {
            /// <summary>
            /// 实验室送测提交图片
            /// </summary>
            public const string enum_filepath_1 = @"/pictrue/InspectionLaboratory";

            /// <summary>
            /// ART图片上传
            /// </summary>
            public const string enum_filepath_2 = @"/pictrue/ArtImage";

            /// <summary>
            /// ART定制检验项目文件路径
            /// </summary>
            public const string enum_filepath_3 = @"/file/ARTCustomQualityFile";

            /// <summary>
            /// QA鞋型问题点图片上传路径
            /// </summary>
            public const string enum_filepath_4 = @"/pictrue/QAShoeShapeImage";

            /// <summary>
            /// QA鞋型 文件上传 四种类型（Limited release/Disclimer/Visual standard/Other）
            /// </summary>
            public const string enum_filepath_5 = @"/file/QAShoeShapeFile";

            /// <summary>
            /// 金属检验上传图片
            /// </summary>
            public const string enum_filepath_6 = @"/pictrue/InspectionMetal";

            /// <summary>
            /// 色卡检验项目图片
            /// </summary>
            public const string enum_filepath_7 = @"/pictrue/ColorCard";
            /// <summary>
            /// 鞋面品质审核图片上传路径
            /// </summary>
            public const string enum_filepath_8 = @"/pictrue/VampQuality";
            /// <summary>
            /// 客户投诉图片上传路径
            /// </summary>
            public const string enum_filepath_9 = @"/pictrue/CustomerComplaint";
            /// <summary>
            /// 客户投诉文件上传路径
            /// </summary>
            public const string enum_filepath_10 = @"/file/CustomerComplaint";




        }






    }
}
