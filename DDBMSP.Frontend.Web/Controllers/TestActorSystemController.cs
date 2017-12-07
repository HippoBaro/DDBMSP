using System;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public class TestActorSystemController : Controller
    {
        [HttpGet("/test/{id}")]
        public Task<string> GetInfo(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return friend.Test();
        }
        
        [HttpGet("/test/{id}/exists")]
        public async Task<bool> Exists(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return (await friend.GetState()).Exists;
        }
        
    }
}