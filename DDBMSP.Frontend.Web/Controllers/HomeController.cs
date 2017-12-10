using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DDBMSP.Frontend.Web.Controllers
{
    public struct Post
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public string AuthorId { get; set; }
        public string AuthorImage { get; set; }
        public string AuthorName { get; set; }
        public string Image { get; set; }
        public string Excerpt { get; set; }
        public DateTime CreationDate { get; set; }
        public string Content { get; set; }
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
                    Url = "http://localhost:5000/post",
                    Title = "Test Article title",
                    Tags = new List<string> { "TAG" },
                    AuthorName = "Hippo",
                    Image = "https://casper.ghost.org/v1.0.0/images/welcome.jpg",
                    Excerpt = "Lot of things to say, very few words to do it. 33, that is."
                    
                },
                new Post {
                    Url = "http://localhost:5000/post",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                },
                new Post {
                    Url = "http://localhost:5000/post",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                },
                new Post {
                    Url = "http://localhost:5000/post",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                }
                ,new Post {
                    Url = "http://localhost:5000/post",
                    Title = "Another Test Article title",
                    Tags = new List<string>(),
                    AuthorName = "Hippo"
                }
            };
            
            return View("/Views/Index.cshtml", model);
        }
        
        [Route("post")]
        public IActionResult Article()
        {
            return View("/Views/Post.cshtml", new Post {
                Url = "http://localhost:5000",
                Title = "Test Article title",
                Tags = new List<string> { "TAG" },
                AuthorName = "Hippo",
                Image = "https://casper.ghost.org/v1.0.0/images/welcome.jpg",
                Excerpt = "Lot of things to say, very few words to do it. 33, that is.",
                Content = "# Sonus opem nota exstitit tuam Olympus mundi\r\n\r\n## Caelo faciendus carmina\r\n\r\nLorem markdownum tres serius passis extrema. **Placidos cum decuit** lemnius.\r\nSibi vincet in, non huius, et quod regna consumptis mole. Ore cum.\r\n\r\n- Nec ferre dabimus quae fui flore qui\r\n- Siquis ulla aequore modo\r\n- Umbras casus admonitus iuvenci\r\n\r\n## Est simul partes et apertum meruere coniugium\r\n\r\nAb tecta eminus magnis, non veniente possit. Digiti curva sermonibus arbor\r\ncorreptus habuisse septem, mox quae pectore placet inhospita nomine Sticteque\r\npennis mundumque faciam fontem perdix.\r\n\r\n## Tum neque hic ita vasto acuto dicere\r\n\r\nPatientia cuncta; fungi caerulus. Gestu habendam dolens et [collige\r\npia](http://www.tuos.io/tuta) caesa consistere comas, dummodo praepetis. Inter\r\nvirorum conceperat excipit attonitoque tecti. Cum male quippe nihil, modo aliter\r\nCreten! Spargimur virgo quos, Dymantis pallamque Dianae paludibus utque vestro.\r\n\r\n> Vix *sua unco*. Duas umbram illa: stridula sive relinqui vitta; sub ille et\r\n> quae obstantis natis sustinuit tenui orbem inconstantia! Vivit verboque sinit?\r\n\r\n## Corripuit denum\r\n\r\nPost coniuge. Fama actis procis furit victa quae lacunabant vobis eripitur\r\narcere inquit qua quid pendebat Gorgoneas posita, **quantum insequitur**. Atque\r\n**et tellus** mihi contenta ut motibus [notas](http://www.vellet-penthea.net/).\r\nDammas arsuros nocebant et saeva omnes magica concipit nova noxae loqui magnae\r\net rerum relicta. Sepulcro artus alta pro caudice **omnis** nido iamque,\r\nferasque illos me.\r\n\r\n## Gelidis fuerant aenae verba ultimus grata et\r\n\r\nDoli et profusis veniam inductum voverat Troiaeque bracchia montis extulit tum,\r\ndeclinat. Nam contorta, erat bicoloribus, mihi obsita? Mihi sulcis arescere\r\ncredere dixit penetrale sive auctore marmoreo adfuit silenti rabiemque flebant\r\nutile praetentaque laetos. Vesci latitavimus miserum gentes spumigeroque clamor,\r\n*possit innixa* accinctus nata dixit colebat nec vulgus. Si antiqua ferax tuque\r\ntenet, et iram causa saevit Ecce haustus, sollicitatque.\r\n\r\n*Orbem* intrat timido **conscius** incoquit saepe, festumque videt refers actis\r\n**domos** in. Silvis facto!"
            });
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