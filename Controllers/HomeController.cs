using CloudComputing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudComputing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FetchFileAsync()
        {
            

            FileModel model = new FileModel();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://filegenerateapi.azurewebsites.net/");
            HttpResponseMessage response = await client.GetAsync("api/file/generate");
            
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                watch.Stop();
                model.ExecutionTime = watch.ElapsedMilliseconds;
                FileInfo fi = new FileInfo("document.txt");

                return View(model);

            }
            return View();
        }

        public async Task<IActionResult> Download()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://filegenerateapi.azurewebsites.net/");

            HttpResponseMessage response = await client.GetAsync("api/file/download");
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsByteArrayAsync();
                using var stream = System.IO.File.Create("document.txt");
                stream.Write(res, 0, res.Length);
                stream.Flush();
                stream.Close();

                return File(new FileStream("document.txt", FileMode.Open), "text/plain", "document.txt");
                
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
