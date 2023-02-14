using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudComputingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpGet("generate")]
        public async Task<IActionResult> Get()
        {

            FileStream fileStream = new FileStream("File.txt", FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                Random random = new Random();
                var i = 0;
                while (i < 2000000)
                {
                    await writer.WriteLineAsync(random.Next(0, 1023).ToString());
                    i++;
                }
            }

            var filePath = "File.txt";
            string[] numbers = System.IO.File.ReadAllLines(filePath);


            fileStream = new FileStream("CalculatedDocument.txt", FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                var sum = 0;
                for (var i = 0; i < numbers.Length-1; i++)
                {
                   
                    sum += Int32.Parse(numbers[i]) + Int32.Parse(numbers[i + 1]);
                    await writer.WriteLineAsync(sum.ToString());
                    sum = 0;
                }
            }

            var calculatedFilePath = "CalculatedDocument.txt";


            byte[] documentBytes = await System.IO.File.ReadAllBytesAsync(calculatedFilePath);
            MemoryStream ms = new MemoryStream(documentBytes);
            return new FileStreamResult(ms, "text/plain");

        }

        [HttpGet("download")]
        public async Task<IActionResult> Download()
        {
            byte[] documentBytes = await System.IO.File.ReadAllBytesAsync("CalculatedDocument.txt");
            MemoryStream ms = new MemoryStream(documentBytes);

            return File(documentBytes, "text/plain", "CalculatedDocument.txt");
        }
    }
}
