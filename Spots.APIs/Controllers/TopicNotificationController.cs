using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicNotificationController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public TopicNotificationController(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }


        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PushTopicNotification()
        {
            var note = new FcmTopicNotification(hostEnvironment);
            await note.OnGetAsync("sd","asdasdf","testtopic");
            return Ok();
        }
    }
}
