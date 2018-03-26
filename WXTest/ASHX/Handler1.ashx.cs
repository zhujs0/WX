using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace WXTest.ASHX
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        public static string CorpToken = "mlXwcNCbuCFf3Uhe";
        public static string corpId = "ww2d764427c29d406f";
        public static string encodingAESKey = "8AMCqdlmqEbjg972t6o2x6gBmrzkeIejARAn5peJBf5";
        public static string sEchoStr = "";
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                using (Stream s = HttpContext.Current.Request.InputStream)
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    string msgBody = Encoding.UTF8.GetString(b);
                    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(CorpToken, encodingAESKey, corpId);
                    string msg_signature = HttpContext.Current.Request.QueryString["msg_signature"];
                    string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
                    string nonce = HttpContext.Current.Request.QueryString["nonce"];
                    string echostr = HttpContext.Current.Request.QueryString["echostr"];
                    #region===============首次开启消息通知时,用于验证url=================
                    sEchoStr = "";
                    var ret = wxcpt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
                    context.Response.Write(sEchoStr);
                    #endregion
                    string sMsg = "";  // 解析之后的明文  
                    int flag = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, msgBody, ref sMsg);//解密
                    if(flag==0)//解密成功
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(sMsg);
                        XmlNode node = doc.FirstChild;
                        string Path = HttpContext.Current.Server.MapPath("../LogInfo.txt");
                        string ToUserName = node["ToUserName"].InnerText;//企业corpid
                        string FromUserName = node["FromUserName"].InnerText;//消息发起人userid
                        string CreateTime= node["CreateTime"].InnerText;//发起时间
                        string MsgType = node["MsgType"].InnerText;//消息类型
                        string AgentID = node["AgentID"].InnerText;//应用id
                        switch (MsgType)
                        {
                            case "event"://事件
                                string Event = node["Event"].InnerText;//事件类型
                                switch(Event)
                                {
                                    case "click":
                                        string EventKey= node["EventKey"].InnerText;//点击事件key（按钮键）
                                        switch(EventKey)
                                        {
                                            case "eventClick"://考勤
                                                GetAccess_Token objToken = new GetAccess_Token();
                                                string Access_Token = objToken.Get();
                                                //上传普通文件类型的临时素材
                                                string postUrl = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token=" + Access_Token + "&type=file";
                                                string filePath = HttpContext.Current.Server.MapPath("../考勤记录.xlsx");
                                                WebClient webClient = new WebClient();
                                                webClient.Credentials = CredentialCache.DefaultCredentials;
                                                byte[] responseArray = webClient.UploadFile(postUrl, "POST", filePath);
                                                string result = Encoding.Default.GetString(responseArray, 0, responseArray.Length);
                                                JObject Robj = JObject.Parse(result);
                                                if(Robj["errcode"].ToString()=="0")
                                                {
                                                    string media_id = Robj["media_id"].ToString();
                                                    string postData= "{\"touser\": \"" + FromUserName + "\",\"toparty\" :\"\",\"totag\" : \"\","
                                                        + "\"msgtype\" : \"file\",\"agentid\" : " + AgentID + ",\"file\" : {"
                                                         + "\"media_id\" : \"" + media_id + "\"},\"safe\":0}";
                                                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                                                    string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + Access_Token;
                                                    byte[] pushResult = webClient.UploadData(url, "POST",bytes);
                                                    string strResult = Encoding.Default.GetString(pushResult, 0, pushResult.Length);
                                                }
                                                
                                                #region
                                                //WebRequest request = WebRequest.Create(postUrl);
                                                //request.Method = "POST";
                                                //request.ContentType = "multipart/form-data;name=\"media\",filename=\"LogInfo.txt\"";
                                                //using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
                                                //{
                                                //    int fsLen = (int)fsRead.Length;
                                                //    byte[] heByte = new byte[fsLen];
                                                //    fsRead.Read(heByte, 0, heByte.Length);
                                                //    string writePath = HttpContext.Current.Server.MapPath("../Log.txt");
                                                //    using(FileStream fsWrite = new FileStream(writePath, FileMode.Append))
                                                //    {
                                                //        fsWrite.Write(heByte, 0, heByte.Length);
                                                //    }
                                                //    request.ContentLength = fsLen;
                                                //    using (Stream requestStream = request.GetRequestStream())
                                                //    {
                                                //        requestStream.Write(heByte, 0, heByte.Length);
                                                //    }
                                                //    WebResponse response = request.GetResponse();
                                                //    Stream responseStream = response.GetResponseStream();
                                                //    StreamReader reader = new StreamReader(responseStream, Encoding.Default);
                                                //    string str = reader.ReadToEnd();
                                                //    string ResultPath= HttpContext.Current.Server.MapPath("../result.txt");
                                                //    using (StreamWriter sw = new StreamWriter(ResultPath, true))
                                                //    {
                                                //        sw.WriteLine("PostFile:"+str);
                                                //    }
                                                //}
                                                #endregion


                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ResultPath = HttpContext.Current.Server.MapPath("../result.txt");
                using (StreamWriter sw = new StreamWriter(ResultPath, true, Encoding.UTF8))
                {
                    sw.WriteLine("err:" + ex.Message);
                }
            }
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