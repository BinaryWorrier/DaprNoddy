using Dapr.Client;
using Man.Dapr.Sidekick;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DaprProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly IDaprSidecarHost daprSidecarHost;

        public WidgetController(IDaprSidecarHost daprSidecarHost)
        {
            this.daprSidecarHost = daprSidecarHost;
        }

        // PUT api/<WidgetController>/5
        [HttpPut("{id}")]
        public async Task PutAsync(string id, [FromBody] Widget widget, CancellationToken cancellationToken)
        {
            using var client = new DaprClientBuilder().Build();
            //Using Dapr SDK to publish a topic
            await client.PublishEventAsync("my-pubsub",  "test", JsonSerializer.Serialize(widget), cancellationToken);
            Console.WriteLine("Published data: " + id);
        }

        [HttpGet("status")]
        public ActionResult GetStatus() => Ok(new
        {
            process = daprSidecarHost.GetProcessInfo(),   // Information about the sidecar process such as if it is running
            options = daprSidecarHost.GetProcessOptions() // The sidecar options if running, including ports and locations
        });
    }
}
