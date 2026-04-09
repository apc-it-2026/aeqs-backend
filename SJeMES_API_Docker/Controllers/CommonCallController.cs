using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SJWebAPI_NETCore_Docker.Controllers
{

    [Route("api/[controller]")]
    public class CommonCallController : Controller
    {
        

        Dictionary<string, Assembly> AssemblyCache = new Dictionary<string, Assembly>();
        Hashtable InstanceCache = new Hashtable();


        [HttpGet]
        public string Get()
        {
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string s = Newtonsoft.Json.JsonConvert.SerializeObject("欢迎使用广东商基Web API");

            return s;
        }


        [HttpPost]
        public string Post([FromBody]object parameter)
        {

            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();
           


            try
            {
                //var p = ReqObj.ToObj(parameter);
                GDSJFramework_NETCore.WebAPI.RequestObject p;
                try
                {
                    p = Newtonsoft.Json.JsonConvert.DeserializeObject<GDSJFramework_NETCore.WebAPI.RequestObject>(parameter.ToString());
                }
                catch (Exception)
                {
                    string RASKey = string.Empty;
                    GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);
                    RASKey = DB.GetString(@"SELECT Key2 FROM rasinfo");

                    string data = GDSJFramework_NETCore.Common.RASHelper.RASDecrypt(parameter.ToString(), RASKey);
                    p = Newtonsoft.Json.JsonConvert.DeserializeObject<GDSJFramework_NETCore.WebAPI.RequestObject>(data);
                }





                Assembly assembly = null;



                if (AssemblyCache.ContainsKey(p.DllName))
                {
                    assembly = AssemblyCache[p.DllName];
                }
                else
                {

                    assembly = Assembly.LoadFrom(p.DllName + ".dll");

                    AssemblyCache.Add(p.DllName, assembly);
                }

                Type type = assembly.GetType(p.ClassName);

                object instance = null;

                if (InstanceCache.Contains(type))
                {
                    instance = InstanceCache[type];
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                    InstanceCache.Add(type, instance);
                }

                MethodInfo mi = type.GetMethod(p.Method);


                object[] args = new object[1];

                args[0] = p;

                object obj = mi.Invoke(instance, args);
                ret = (GDSJFramework_NETCore.WebAPI.ResultObject)obj;

                if (p.IsRasResult)
                {
                    ret.RetData = GDSJFramework_NETCore.Common.RASHelper.RASEncryption(p.RasResultKey, ret.RetData);
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;

            }



            return Newtonsoft.Json.JsonConvert.SerializeObject(ret);

        }
    }
}
