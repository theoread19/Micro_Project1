using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.APICall
{
    public static class ApiCall
    {
        public static string GetApi(string ApiUrl)
        {

            var responseString = "";
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response1 = request.GetResponse())
            {
                using (var reader = new StreamReader(response1.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            return responseString;

        }

        public static void PostApi(string ApiUrl, string postData = "")
        {

            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            
            request.GetResponse();
        }


        public static void PutApi(string ApiUrl, string postData = "")
        {
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "PUT";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            request.GetResponse();
        }

        public static void DeleteApi(string ApiUrl, long id)
        {
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            var data = Encoding.ASCII.GetBytes(id.ToString());

            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            request.GetResponse();
        }
    }
}
