using Newtonsoft.Json;
using System.Text;

namespace TranslateService.Models
{
    public class MyService
    {

        

        public static async Task TranslateText()
        {
             string key = "656cacfddf424c9f84ebe6cdb8ea2610";
             string endPoint = "https://api.cognitive.microsofttranslator.com/";

             string region = "westeurope";
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage requestMessage = new HttpRequestMessage())
                {
                    string Result = "";

                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri(endPoint + "translate?api-version=3.0&fromto=fr&to=uk&to=pl&to=de");
                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Region", region);

                    var inputStr = "You are really cool programmers and I hope that you'll do your fucking best to realize all your potential!";



                    object[] body = new object[] { new { Text = inputStr } };

                    string request = JsonConvert.SerializeObject(body);

                    requestMessage.Content = new StringContent(request, Encoding.Unicode, "application/json");

                    HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                   string text = httpResponseMessage.Content.ReadAsStringAsync().Result;

                   // Console.WriteLine(text);

                    RootForTranslate[] obj = JsonConvert.DeserializeObject<RootForTranslate[]>(text);

                    foreach (RootForTranslate root in obj)
                    {
                        //Console.WriteLine(root.detectedLanguage.language);
                        //Console.WriteLine(root.detectedLanguage.score);

                        foreach (TranslationForTranslate item2 in root.translations)
                        {
                            //Console.WriteLine($"Language: {item2.to}");
                             Result=item2.text;

                        }


                    }
                    
                }
            }
        }
    }
}
