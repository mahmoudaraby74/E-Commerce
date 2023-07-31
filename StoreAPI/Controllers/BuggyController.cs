using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.ResponseModule;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly StoreDbContext storeDbContext;

        public BuggyController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }
        [HttpGet("TestText")]
        [Authorize]
        public ActionResult<string> GetText()
        {
            return "some text";
        }
        [HttpGet("notFound")]
        public ActionResult<string> GetNotFoundRequest()
        {
            var product = storeDbContext.Products.Find(1000);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(product);
        }
        [HttpGet("BadRequset")]
        public ActionResult<string> GetBadRequset()
        {
            return BadRequest(new ApiResponse(400));
        }
    }
}
