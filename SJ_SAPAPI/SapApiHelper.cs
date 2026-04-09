using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SJ_SAPAPI
{
    public static class SapApiHelper
    {
        public static string URL_PREFIX = "";
        public static string USER_NAME = "";
        public static string PASSWORD = "";

        public static void GetSapApiSetting(SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            if (!string.IsNullOrEmpty(URL_PREFIX) && !string.IsNullOrEmpty(USER_NAME) && !string.IsNullOrEmpty(PASSWORD))
                return;

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);

            var sapDt = DB.GetDataTable($@"
SELECT
	sapapipath,
	sapuser,
	sappwd
FROM
	SYSORG01M
WHERE
	UPPER (org) = '{companyCode.ToUpper()}';");
            if (sapDt.Rows.Count > 0)
            {
                var sapRow = sapDt.Rows[0];
                URL_PREFIX = sapRow["sapapipath"].ToString();
                USER_NAME = sapRow["sapuser"].ToString();
                PASSWORD = sapRow["sappwd"].ToString();
            }
        }

        /// <summary>
        /// 获取特殊包装资料
        /// </summary>
        /// <param name="requestParam"></param>
        public static Special.Packaging.Information.DT_SD023_RSP GetSpecialPackagingInformation(Special.Packaging.Information.DT_SD023_REQI_REQUESTBody requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Special.Packaging.Information.DT_SD023_REQ request = new Special.Packaging.Information.DT_SD023_REQ();
                request.I_REQUEST = new Special.Packaging.Information.DT_SD023_REQI_REQUEST();
                request.I_REQUEST.Header = new Special.Packaging.Information.DT_BASEINFO_REQ();
                request.I_REQUEST.Body = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_SD023_OUT&interfaceNamespace=urn.apache.sd023";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Special.Packaging.Information.SI_SD023_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_SD023_OUTAsync(request).Result.MT_SD023_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSpecialPackagingInformation Error");
            }
        }

        /// <summary>
        /// 取ART量产BOM取ART的开发阶段对应的BOM
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static Art.Bom.DT_PP113_RSP GetArtBom(Art.Bom.DT_PP113_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Art.Bom.DT_PP113_REQ request = new Art.Bom.DT_PP113_REQ();
                request.I_REQUEST = new Art.Bom.DT_PP113_REQI_REQUEST();
                request.I_REQUEST.HEADER = new Art.Bom.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_PP113_OUT&interfaceNamespace=urn.apache.pp113";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Art.Bom.SI_PP113_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_PP113_OUTAsync(request).Result.MT_PP113_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetArtBom Error");
            }
        }

        /// <summary>
        /// 分段资料_鞋带or大底
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static Subsection.ShoelacesAndOutsole.DT_PP114_RSP GetSubsectionShoelacesAndOutsole(Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Subsection.ShoelacesAndOutsole.DT_PP114_REQ request = new Subsection.ShoelacesAndOutsole.DT_PP114_REQ();
                request.I_REQUEST = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUEST();
                request.I_REQUEST.HEADER = new Subsection.ShoelacesAndOutsole.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_PP114_OUT&interfaceNamespace=urn.apache.pp114";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Subsection.ShoelacesAndOutsole.SI_PP114_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_PP114_OUTAsync(request).Result.MT_PP114_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSubsectionShoelacesAndOutsole Error");
            }
        }

        /// <summary>
        /// 分段资料_高度翘度
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static Subsection.HeightWarpage.DT_PP115_RSP GetSubsectionHeightWarpage(Subsection.HeightWarpage.DT_PP115_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Subsection.HeightWarpage.DT_PP115_REQ request = new Subsection.HeightWarpage.DT_PP115_REQ();
                request.I_REQUEST = new Subsection.HeightWarpage.DT_PP115_REQI_REQUEST();
                request.I_REQUEST.HEADER = new Subsection.HeightWarpage.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_PP115_OUT&interfaceNamespace=urn.apache.pp115";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Subsection.HeightWarpage.SI_PP115_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_PP115_OUTAsync(request).Result.MT_PP115_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSubsectionHeightWarpage Error");
            }
        }

        /// <summary>
        /// 分段资料_logo
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static Subsection.Logo.DT_PP116_RSP GetSubsectionLogo(Subsection.Logo.DT_PP116_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Subsection.Logo.DT_PP116_REQ request = new Subsection.Logo.DT_PP116_REQ();
                request.I_REQUEST = new Subsection.Logo.DT_PP116_REQI_REQUEST();
                request.I_REQUEST.HEADER = new Subsection.Logo.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_PP116_OUT&interfaceNamespace=urn.apache.pp116";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Subsection.Logo.SI_PP116_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_PP116_OUTAsync(request).Result.MT_PP116_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSubsectionLogo Error");
            }
        }

        /// <summary>
        /// 取ART的图片
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static Art.Image.DT_PP117_RSP GetArtImage(Art.Image.DT_PP117_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                Art.Image.DT_PP117_REQ request = new Art.Image.DT_PP117_REQ();
                request.I_REQUEST = new Art.Image.DT_PP117_REQI_REQUEST();
                request.I_REQUEST.HEADER = new Art.Image.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_PP117_OUT&interfaceNamespace=urn.apache.pp117";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new Art.Image.SI_PP117_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_PP117_OUTAsync(request).Result.MT_PP117_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetArtImage Error");
            }
        }

        /// <summary>
        /// 订单包装材料预算表
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        public static OrderPackaging.MaterialBudget.DT_AEQS2_RSP GetOrderPackagingMaterialBudget(OrderPackaging.MaterialBudget.DT_AEQS2_REQI_REQUESTBODY requestParam, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                OrderPackaging.MaterialBudget.DT_AEQS2_REQ request = new OrderPackaging.MaterialBudget.DT_AEQS2_REQ();
                request.I_REQUEST = new OrderPackaging.MaterialBudget.DT_AEQS2_REQI_REQUEST();
                request.I_REQUEST.HEADER = new OrderPackaging.MaterialBudget.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = requestParam;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_AEQS2_OUT&interfaceNamespace=urn.apache.aeqs2";
                var endpoint = new System.ServiceModel.EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new OrderPackaging.MaterialBudget.SI_AEQS2_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_AEQS2_OUTAsync(request).Result.MT_AEQS2_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetOrderPackagingMaterialBudget Error");
            }
        }

        /// <summary>
        /// 鞋款材料成分维护
        /// </summary>
        /// <param name="artNo"></param>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public static MaintenanceShoeMaterial.DT_SD105_RSP GetMaintenanceOfShoeMaterialComposition(MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY[] artArr, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            try
            {
                GetSapApiSetting(ReqObj);
                MaintenanceShoeMaterial.DT_SD105_REQ request = new MaintenanceShoeMaterial.DT_SD105_REQ();
                request.I_REQUEST = new MaintenanceShoeMaterial.DT_SD105_REQI_REQUEST();
                request.I_REQUEST.HEADER = new MaintenanceShoeMaterial.DT_BASEINFO_REQ();
                request.I_REQUEST.BODY = artArr;

                string url = URL_PREFIX + "/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_AEQS&receiverParty=&receiverService=&interface=SI_SD105_OUT&interfaceNamespace=urn.apache.sd105";
                var endpoint = new EndpointAddress(url);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                var wsclient = new MaintenanceShoeMaterial.SI_SD105_OUTClient(binding, endpoint);
                wsclient.ClientCredentials.UserName.UserName = USER_NAME;
                wsclient.ClientCredentials.UserName.Password = PASSWORD;

                var rsp = wsclient.SI_SD105_OUTAsync(request).Result.MT_SD105_RSP;

                return rsp;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMaintenanceOfShoeMaterialComposition Error");
            }
        }

    }
}
