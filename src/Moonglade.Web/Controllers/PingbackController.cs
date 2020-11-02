﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonglade.Auditing;
using Moonglade.Configuration.Abstraction;
using Moonglade.Core.Notification;
using Moonglade.Pingback;
using Moonglade.Pingback.Mvc;

namespace Moonglade.Web.Controllers
{
    [Route("pingback")]
    public class PingbackController : BlogController
    {
        private readonly IBlogConfig _blogConfig;
        private readonly IPingbackService _pingbackService;
        private readonly IBlogNotificationClient _notificationClient;

        public PingbackController(
            ILogger<PingbackController> logger,
            IBlogConfig blogConfig,
            IPingbackService pingbackService,
            IBlogNotificationClient notificationClient)
            : base(logger)
        {
            _blogConfig = blogConfig;
            _pingbackService = pingbackService;
            _notificationClient = notificationClient;
        }

        [HttpPost("")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Index()
        {
            if (!_blogConfig.AdvancedSettings.EnablePingBackReceive)
            {
                Logger.LogInformation("Pingback receive is disabled");
                return Forbid();
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var requestBody = await new StreamReader(HttpContext.Request.Body, Encoding.Default).ReadToEndAsync();

            var response = await _pingbackService.ReceivePingAsync(requestBody, ip,
                history => _notificationClient.NotifyPingbackAsync(history));

            Logger.LogInformation($"Pingback Processor Response: {response}");
            return new PingbackResult(response);
        }

        [Authorize]
        [HttpDelete("{pingbackId:guid}")]
        public async Task<IActionResult> Delete(Guid pingbackId, [FromServices] IBlogAudit blogAudit)
        {
            await _pingbackService.DeletePingbackHistory(pingbackId);
            await blogAudit.AddAuditEntry(EventType.Content, AuditEventId.PingbackDeleted,
                $"Pingback '{pingbackId}' deleted.");
            return Ok();
        }
    }
}