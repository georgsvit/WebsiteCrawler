using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace WebsiteCrawler.Services
{
    public static class HttpService
    {
        public static string GetFileDataByUri(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader reader = string.IsNullOrWhiteSpace(response.CharacterSet) switch
                    {
                        false => new(receiveStream, Encoding.GetEncoding(response.CharacterSet)),
                        _ => new(receiveStream)
                    };

                    string data = reader.ReadToEnd();

                    response.Close();
                    reader.Close();

                    if (String.IsNullOrWhiteSpace(data))
                        throw new Exception("File is empty");

                    return data;

                default:
                    response.Close();
                    throw new Exception($"Response status code: {response.StatusCode}");
            }
        }

        public static (string, TimeSpan) GetFileDataAndResponseTimeByUri(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            Stopwatch timer = new();

            timer.Start();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            timer.Stop();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader reader = string.IsNullOrWhiteSpace(response.CharacterSet) switch
                    {
                        false => new(receiveStream, Encoding.GetEncoding(response.CharacterSet)),
                        _ => new(receiveStream)
                    };

                    string data = reader.ReadToEnd();

                    response.Close();
                    reader.Close();

                    if (String.IsNullOrWhiteSpace(data))
                        throw new Exception("File is empty");

                    return (data, timer.Elapsed);

                default:
                    response.Close();
                    throw new Exception($"Response status code: {response.StatusCode}");
            }
        }
    }    
}
