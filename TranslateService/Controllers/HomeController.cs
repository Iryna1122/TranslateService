using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using TranslateService.Models;

namespace TranslateService.Controllers
{
    public class HomeController : Controller
    {


        MyService myService = new MyService();

        public string keyTr = "656cacfddf424c9f84ebe6cdb8ea2610";//from azure translator
        public string endPointTr = "https://api.cognitive.microsofttranslator.com/";
        public string regionTr = "westeurope";

        public HomeController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> TranslateText()
        {
            string inputText = Request.Form["inputtext"];
            string lan = Request.Form["tolanguage"];
            string source = Request.Form["fromlanguage"];

            // TranslateText();
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage requestMessage = new HttpRequestMessage())
                {
                    string Result = "";

                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri(endPointTr + $"translate?api-version=3.0&to={lan}");
                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", keyTr);

                    requestMessage.Headers.Add("Ocp-Apim-Subscription-Region", regionTr);

                    //  var inputStr = "You are really cool programmers and I hope that you'll do your fucking best to realize all your potential!";



                    object[] body = new object[] { new { Text = inputText } };

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
                            Result = item2.text;

                            ViewBag.Text = Result;

                        }


                    }

                }
            }


            return View("Index");
        }



        [HttpPost]
        public async Task<IActionResult> ImageTranslate()
        {
            string inputText = Request.Form["textimage"];//from textarea for url img

            string keyZeer = "9d7fca2694bb4aa1993d04f50cea6184";//from cognitive service
            string endPointZeer = "https://myoliinykcognservice.cognitiveservices.azure.com/";
            string regionZeer = "westeurope";


            ComputerVisionClient computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(keyZeer)) { Endpoint = endPointZeer};

            string path_img = inputText;

            List<VisualFeatureTypes?> visualFeatures = Enum.GetValues(typeof(VisualFeatureTypes)).OfType<VisualFeatureTypes?>().ToList();
            var result = "";
            ImageAnalysis image = await computerVisionClient.AnalyzeImageAsync(path_img, visualFeatures);
            var text = await computerVisionClient.RecognizePrintedTextAsync(true, path_img);

            foreach (var regions in text.Regions)
            {
                foreach (var lines in regions.Lines)
                {
                    foreach (var words in lines.Words)
                    {
                        result += words.Text + " ";
                    }
                }

            }

            string lan = Request.Form["tolanguageimg"];
            string source = Request.Form["fromlanguageimg"];

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage())
                {

                    httpRequestMessage.Method = HttpMethod.Post;
                    httpRequestMessage.RequestUri = new Uri(endPointTr + $"translate?api-version=3.0&to={lan}");
                    httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", keyTr);
                    httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Region", regionTr);

                    object[] body = new object[] { new { Text = result } };

                    string req = JsonConvert.SerializeObject(body);

                    httpRequestMessage.Content = new StringContent(req, Encoding.Unicode, "application/json");

                    HttpResponseMessage responseMessage = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

                    string responsText = await responseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine(responsText);



                    RootForTranslate[] param = JsonConvert.DeserializeObject<RootForTranslate[]>(responsText);
                    foreach (RootForTranslate item in param)
                    {

                        foreach (TranslationForTranslate translation in item.translations)
                        {
                            var imgtranslate = translation.text;
                            ViewBag.TextImgTranslate = imgtranslate;
                        }
                    }
                }
            }



            return View("Index");
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}