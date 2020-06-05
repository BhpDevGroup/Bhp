using Bhp.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Bhp.Server
{
    public class HttpService
    {
        string postUrl = Settings.Default.Urls.PostUrl;
        public string HttpPost(string param)
        {
            try
            {
                string responseText = string.Empty;
                byte[] bs = Encoding.UTF8.GetBytes(param);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(postUrl);
                req.Method = "POST";
                req.Timeout = 10000;
                req.ContentType = "application/json";
                req.ContentLength = bs.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                HttpWebResponse hwr = req.GetResponse() as HttpWebResponse;
                using (StreamReader myreader = new StreamReader(hwr.GetResponseStream(), Encoding.UTF8))
                {
                    responseText = myreader.ReadToEnd();
                }
                return responseText;
            }
            catch
            {
                return null;
            }
        }
    }
}
