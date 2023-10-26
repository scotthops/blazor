using System.Net;
using AzureServerless.Models.School;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchoolLibrary;

namespace Snoopy.Function
{
    public class HttpWebApi
    {
        private readonly ILogger _logger;
        private readonly SchoolContext _context;

        public HttpWebApi(ILoggerFactory loggerFactory, SchoolContext context)
        {
            _logger = loggerFactory.CreateLogger<HttpWebApi>();
            _context = context;
        }

        [Function("Hello")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("GetStudents")]
        public HttpResponseData GetStudents([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var students = _context.Students;

            response.WriteString(JsonConvert.SerializeObject(students));

            return response;
        }

        [Function("CreateStudent")]
        public HttpResponseData CreateStudent
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req
        )
        {
            _logger.LogInformation("C# HTTP POST/posts trigger function processed a request.");
            var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            _context.Students.Add(student);
            _context.SaveChanges();
            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonConvert.SerializeObject(student));
            return response;
        }

        [Function("UpdateStudent")]
        public HttpResponseData UpdateStudent
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req,
            int id
        )
        {
            _logger.LogInformation("C# HTTP PUT/posts trigger function processed a request.");
            var student = _context.Students.FindAsync(id).Result;
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Not Found");
                return response;
            }
            var student2 = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            student.FirstName = student2.FirstName;
            student.LastName = student2.LastName;
            student.School = student2.School;
            _context.SaveChanges();
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            response2.WriteString(JsonConvert.SerializeObject(student));
            return response2;
        }

        [Function("DeleteStudent")]
        public HttpResponseData DeleteStudent
        (
          [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req,
          int id
        )
        {
            _logger.LogInformation("C# HTTP DELETE/posts trigger function processed a request.");
            var student = _context.Students.FindAsync(id).Result;
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Not Found");
                return response;
            }
            _context.Students.Remove(student);
            _context.SaveChanges();
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            response2.WriteString(JsonConvert.SerializeObject(student));
            return response2;
        }

        [Function("GetStudentById")]
        public HttpResponseData GetStudentById
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req,
            int id
        )
        {
            _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
            var student = _context.Students.FindAsync(id).Result;
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Not Found");
                return response;
            }
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            response2.WriteString(JsonConvert.SerializeObject(student));
            return response2;
        }

    }
}
