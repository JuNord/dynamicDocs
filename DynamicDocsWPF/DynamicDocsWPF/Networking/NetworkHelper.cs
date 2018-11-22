using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Controls;

namespace DynamicDocsWPF.Networking
{
    public class NetworkHelper
    {
        private string _baseUrl;
        
        public string BaseUrl
        {
            get => _baseUrl;
            private set
            {
                _baseUrl = value;
                if (!_baseUrl[_baseUrl.Length - 1].Equals('/'))
                {
                    _baseUrl += "/";
                }
            }
        }

        public NetworkHelper(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
        
        public byte[] GetRequest(string requestText)
        {
            var url = BaseUrl+requestText;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var bytes = new List<byte>();
                int b;
                
                while((b = stream.ReadByte()) != -1)
                {
                    bytes.Add((byte) b);
                }

                return bytes.ToArray();
            }
        }
    }
}