using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WXTest.ASHX
{
    /// <summary>
    /// Handler2 的摘要说明
    /// </summary>
    public class Handler2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string token = "k5QNM13lM3wXtifsrza9H36XcyNitDpQyzetEIlOqcx7E7DTUD4m1oYntkeLY6YyliO7kOR73HTCfx6RaAfIUspH_pJsr_HMH2bn9ZZS64OpEYBk62D8VhBtqj196r5ywKWMuhCMWdTTkcvm8uSK-LePARftYxgE6MWTHJyMp2D__7g0A-FMBCe6oVvsRwMA-zUgxWYfBc3Fu_d3oVUphdd7yLqpNTepo3HiPVXKhcfK7qUZJ5ZArdhxkDCcaq8R-g0YsvTXAeZ7VDiuPw4A1WnI37zlv8qfVbFV-G76b_U";
            //string token = GetAccess_Token();
            string postDataStr = "{\"touser\" : \"@all\",\"toparty\" : \"\",\"totag\" : \" TagID1 | TagID2 \","
                + "\"msgtype\" : \"text\",\"agentid\" : 1000002,\"text\" : {\"content\" :\"11\"},\"safe\":0}";

            WebRequest request = WebRequest.Create(" https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + token);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream myRequestStream = request.GetRequestStream();
            //StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myRequestStream.Write(bytes, 0, bytes.Length);
            myRequestStream.Close();
            //myStreamWriter.Close();

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.Default);
            string str = reader.ReadToEnd();
            
            
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