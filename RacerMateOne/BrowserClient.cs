using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;


namespace RacerMateOne
{
    public class CustomWebClient : WebClient
    {
        //private Uri m_Location;
        //public Uri Location { get { return m_Location; } }

        private CookieContainer m_container = new CookieContainer();
        
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                HttpWebRequest httprequest = (request as HttpWebRequest);
                httprequest.CookieContainer = m_container;
            }
            return request;
        }
        /*
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request);
                // Perform any custom actions with the response ...
                if (response is HttpWebResponse)
                {
                    HttpWebResponse httpresponse = (response as HttpWebResponse);
                    m_container = new CookieContainer();
                    foreach (Cookie cook in httpresponse.Cookies)
                    {
                        m_container.Add(cook);
                    }
                    m_Location = httpresponse.ResponseUri;
                }
            }
            catch { response = new WebResponse; };
            return response;
        }
         * */


    }
    class BrowserClient
    {
        static CustomWebClient _webClient = new CustomWebClient();
        public static bool PostPWX(string username, string password, string filename)
        {
            //WebClient _webClient = new WebClient();
            _webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            _webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //allows for validation of SSL certificates 
            //ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            _webClient.Credentials = CredentialCache.DefaultCredentials;
            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            
            string WebPage = string.Format(
                "https://www.trainingpeaks.com/TPWebServices/EasyFileUpload.ashx?username={0}&password={1}",
                Uri.EscapeDataString(username),Uri.EscapeDataString(password));

            if (!File.Exists(filename))
                return false; 

            byte[] byteArray;
            //TODO - still have to figure out how to upload a gzip version of PWX file
#if true //NOTUSEDYET
            FileInfo fi = new FileInfo(filename);
            PerfFile.Compress(fi);
            if (!File.Exists(filename + ".gz"))
            {
                byteArray = File.ReadAllBytes(filename);
            }
            else
            {
                byteArray = File.ReadAllBytes(filename + ".gz");
            }
#else
            byteArray = File.ReadAllBytes(filename);
#endif

            // Apply UTF8 Encoding to obtain the string as a byte array.
            byte[] byteResults = _webClient.UploadData(WebPage, "POST", byteArray);

            // TODO - Need to parse data for OK result else error.
            string data = Encoding.UTF8.GetString(byteResults);
            Debug.WriteLine("Results - {0}", data);
            return true;
        }
        public static string PostRM1(string WebPage, string postdata)
        {
            string data = "";
            try
            {
                //CustomWebClient _webClient = new CustomWebClient();
                _webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                _webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //allows for validation of SSL certificates 
                //ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
                _webClient.Credentials = CredentialCache.DefaultCredentials;
                // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                //string WebPage = "http://store.racermateinc.com/store/registration_noui.php";
                //string WebPage = "http://store.racermateinc.com/store/registration2.php?";

                // Apply ASCII Encoding to obtain the string as a byte array.
                byte[] byteArray = Encoding.ASCII.GetBytes(postdata);
                // Apply UTF8 Encoding to obtain the string as a byte array.
                byte[] byteResults = _webClient.UploadData(WebPage, "POST", byteArray);
                //Uri location = _webClient.Location;
                data = Encoding.UTF8.GetString(byteResults);
                //string data = Encoding.ASCII.GetString(byteResults);
                //Debug.WriteLine("Results - {0}", data);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);

            }
            return data;
        }
        public static string GetRM1(string postdata)
        {
            string data = "";
            try
            {
                //WebClient _webClient = new WebClient();
                _webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                _webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //allows for validation of SSL certificates 
                //ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
                _webClient.Credentials = CredentialCache.DefaultCredentials;
                // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                string WebPage = "http://store.racermateinc.com/store/registration_noui.php";
                //string WebPage = "http://store.racermateinc.com/store/registration.php?";
                // Apply ASCII Encoding to obtain the string as a byte array.
                byte[] byteArray = Encoding.ASCII.GetBytes(postdata);
                // Apply UTF8 Encoding to obtain the string as a byte array.
                byte[] byteResults = _webClient.UploadData(WebPage, "POST", byteArray);
                //Uri location = _webClient.Location;

                data = Encoding.UTF8.GetString(byteResults);
                //string data = Encoding.ASCII.GetString(byteResults);
                //Debug.WriteLine("Results - {0}", data);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                
            }
            return data;
        }
    }
}
