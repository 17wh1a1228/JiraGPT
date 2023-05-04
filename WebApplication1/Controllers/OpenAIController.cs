using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class openAIController : Controller
    {
        [HttpGet("{input}")]
        public async Task<IActionResult> GetData(string input)
        {
            string apiKey = "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";
            string response = "";
            OpenAIAPI openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = generate(input);
            completion.Model = "text-davinci-003";
            completion.MaxTokens = 4000;
            var output = await openai.Completions.CreateCompletionAsync(completion);
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Invalid input. Input cannot be null or empty.");
            }
            if (output != null)
            {
                foreach (var item in output.Completions)
                {
                    response = item.Text;
                }
              
               var formattedResponse = new
               {
                   status = "success",
                   description = response
               };
               return Ok(formattedResponse);
            }
            else
            {
                return BadRequest("Not found");
            }
        }

        private string generate(string input)
        {
            string prompt = "Create a jira ticket on " + input + "\n It should contain a title and make sure it is given as just TITLE and not ticket title etc \n" +
                            "a Description section with title as DESCRIPTION bullet points, explaining what needs to be created, why, benefits, use cases.\n" +
                            "A requirements section, containing the components required and their type and features .\n" +
                            "an Acceptance Criteria section, summarizing what needs to be achieved\n" +
                            "a test scenario/case section.";
            return prompt;
        }
    }
}
