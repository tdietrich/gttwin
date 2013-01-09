using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace gttwin.MainC
{
    static class NetControler
    {
        // Stałe do adresów 
        public const string SITE_URL = "http://pam.panel.szn.pl/pz/";
        public const string SCORE_SCRIPT = "record.php";


        public static bool SaveScore(string login, int score)
        {

            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(SITE_URL + SCORE_SCRIPT);
            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = "send=true&"+"email=" + login + "&" + "score=" + score;
            byte[] data = encoding.GetBytes(postData);

            Req.Method = WebRequestMethods.Http.Post;
            Req.ContentType = "application/x-www-form-urlencoded";
            Req.ContentLength = data.Length;


            using (Stream newStream = Req.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }
            HttpWebResponse HttpWResp = (HttpWebResponse)Req.GetResponse();

            if (HttpWResp.StatusDescription == "OK")
                return true;
            else return false;

        }

        
    }
}
