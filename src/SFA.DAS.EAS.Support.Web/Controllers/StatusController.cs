﻿using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using SFA.DAS.EAS.Support.ApplicationServices;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[AllowAnonymous]
[Route("api/status")]
public class StatusController : ControllerBase
{
    private readonly ILogger<StatusController> _logger;
    public StatusController(ILogger<StatusController> logger) => _logger = logger;
    
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Retrieving status info from {ControllerName}.", nameof(StatusController));

        var model = new
        {
            ServiceName = "SFA DAS Employer Apprenticeship Service Support Site",
            ServiceVersion = Assembly.GetExecutingAssembly().Version(),
            ServiceTime = DateTimeOffset.UtcNow,
            Request = $" {HttpContext.Request.Method}: {HttpContext.Request.GetDisplayUrl()}"
        };
        
        return Ok(model);
    }
}