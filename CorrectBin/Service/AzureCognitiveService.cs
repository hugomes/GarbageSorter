using CorrectBin.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CorrectBin.Service
{
    public class AzureCognitiveService
    {
        static string subsKey = "9977b9f942434f05bfd861ea797748bd";
        static string endpoint = "https://correctbin.cognitiveservices.azure.com/";
        static string uriBase = endpoint + "vision/v2.1/analyze";

        public static async Task<ImageInfo> MakeAnalysisRequest(byte[] imageByteData)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subsKey);

                string requestParam = "visualFeatures=Categories,Description";

                string uri = uriBase + "?" + requestParam;

                HttpResponseMessage response;

                using (ByteArrayContent content = new ByteArrayContent(imageByteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ImageInfo>(contentString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
