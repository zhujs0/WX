using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WXTest.ASHX
{
    /// <summary>
    /// GetUserId 的摘要说明
    /// </summary>
    public class GetUserId : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
            GetAccess_Token objToken = new GetAccess_Token();
            string code = context.Request["code"].ToString();
            string token = objToken.Get();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo" + "?access_token=" + token + "&code=" + code;
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = "GET";
            wRequest.ContentType = "text/html;charset=UTF-8";
            WebResponse wResponse = wRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            string str = reader.ReadToEnd();   //url返回的值  
            JObject Robj = JObject.Parse(str);
            string UserId = "";
            if (Robj["errcode"].ToString() == "0")
            {
                UserId = Robj["UserId"].ToString();
            }
            reader.Close();
            wResponse.Close();
            context.Response.Write(UserId);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}