using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace WXTest.ASHX
{
    public class GetAccess_Token
    {
        public static AccessToken Access_Token;
        private string _corpid = "ww2d764427c29d406f";
        private string _corpsecret = "92pvvVx55aLRd5OHO_yDKqTXXcdfSr3GUL3XexVIbmU";
        private string _GetTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken";

        public GetAccess_Token()
        {

        }

        public string Get()
        {
            if (Access_Token == null)
            {
                Access_Token = new AccessToken();
                string url = _GetTokenUrl + "?corpid=" + _corpid + "&corpsecret=" + _corpsecret;
                WebRequest wRequest = WebRequest.Create(url);
                wRequest.Method = "GET";
                wRequest.ContentType = "text/html;charset=UTF-8";
                WebResponse wResponse = wRequest.GetResponse();
                Stream stream = wResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
                string str = reader.ReadToEnd();   //url返回的值  
                JObject Robj = JObject.Parse(str);
                if (Robj["errcode"].ToString() == "0")
                {
                    Access_Token.Access_Token = Robj["access_token"].ToString();
                    Access_Token.GetTime = DateTime.Now.ToString();
                }
                reader.Close();
                wResponse.Close();
            }
            else
            {
                DateTime dateTime = Convert.ToDateTime(Access_Token.GetTime);
                TimeSpan ts = DateTime.Now.Subtract(dateTime);
                double value = ts.TotalSeconds;
                if (value > 7200)
                {
                    Access_Token = new AccessToken();
                    string url = _GetTokenUrl + "?corpid=" + _corpid + "&corpsecret=" + _corpsecret;
                    WebRequest wRequest = WebRequest.Create(url);
                    wRequest.Method = "GET";
                    wRequest.ContentType = "text/html;charset=UTF-8";
                    WebResponse wResponse = wRequest.GetResponse();
                    Stream stream = wResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
                    string str = reader.ReadToEnd();   //url返回的值  
                    JObject Robj = JObject.Parse(str);
                    if (Robj["errcode"].ToString() == "0")
                    {
                        Access_Token.Access_Token = Robj["access_token"].ToString();
                        Access_Token.GetTime = DateTime.Now.ToString();
                    }
                    reader.Close();
                    wResponse.Close();
                }
            }
            return Access_Token.Access_Token;
        }


    }
    public class AccessToken
    {
        public string Access_Token { get; set; }
        public string GetTime { get; set; }
    }
}