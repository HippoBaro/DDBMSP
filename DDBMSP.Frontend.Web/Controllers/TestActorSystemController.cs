using System.Threading.Tasks;
using DDBMSP.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public class TestActorSystemController : Controller
    {
        [HttpGet("/test/{id}")]
        public Task<string> Test(int id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return friend.Test();
        }
    }
}