using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JiraController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateJiraIssue([FromBody] string input)
        {
            string title = "";

            int titleIndex = input.IndexOf("TITLE:");
            if (titleIndex != -1)
            {
                int colonIndex = input.IndexOf(':', titleIndex);
                if (colonIndex != -1)
                {
                    int endIndex = input.IndexOf('D', colonIndex);
                    if (endIndex != -1)
                    {
                        title = input.Substring(colonIndex + 1, endIndex - colonIndex - 1).Trim();
                    }
                    else
                    {
                        title = input.Substring(colonIndex + 1).Trim();
                    }
                }
            }
         

            string apiUrl = "https://aswecomp680.atlassian.net/rest/api/2/issue/";
            var issueData = new
            {
                fields = new
                {
                    project = new
                    {
                        key = "JIR"
                    },
                    summary = title,
                    description = input,
                    issuetype = new
                    {
                        name = "Task"
                    }
                }
            };

            string issueJson = JsonConvert.SerializeObject(issueData);

            var request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";            
            request.Headers["Authorization"] =  GetJiraAuthToken();


            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(issueJson);
            }

            using (var response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return Ok();
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string errorMessage = reader.ReadToEnd();
                    }
                    return StatusCode((int)response.StatusCode);
                }
            }
        }

        private string GetJiraAuthToken()
        {
            // provide your username and password token of Atlassian account
            string username = "YOUR_ID";
            string password = "YOUR_TOKEN";
            string authString = $"{username}:{password}";
            byte[] authBytes = Encoding.UTF8.GetBytes(authString);
            string base64AuthString = Convert.ToBase64String(authBytes);
            return $"Basic {base64AuthString}";
        }

    }
}
