using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DDBMSP.Frontend.Web.Controllers
{
    public struct Post
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string Image { get; set; }
    }
    
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new List<Post>
            {
                new Post {
                    Url = "http://localhost:5000",
                    Title = "Test Article title",
                    Tags = new List<string> { "TAG" },
                    AuthorName = "Hippo",
                    Image = "https://casper.ghost.org/v1.0.0/images/welcome.jpg"
                },
                new Post {
                    Url = "http://localhost:5000",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                },
                new Post {
                    Url = "http://localhost:5000",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                },
                new Post {
                    Url = "http://localhost:5000",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                }
                ,new Post {
                    Url = "http://localhost:5000",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                }
            };
            
            return View("/Views/Index.cshtml", model);
        }
        
        [Route("Error")]
        public IActionResult Error(int? statusCode)
        {
            if (!statusCode.HasValue) return View("/Views/Error.cshtml");
            var viewName = statusCode.ToString();
            return View("/Views/Error.cshtml", viewName);
        }
    }
}