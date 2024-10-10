using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger<CompensationController> _logger;
        private readonly ICompensationService _compensationService;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpGet("{employeeId}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String employeeId)
        {
            _logger.LogDebug($"Received request for compensation for '{employeeId}'");
            var compensation = _compensationService.GetByEmployeeId(employeeId);
            if (compensation == null)
            {
                return NotFound();
            }
            return Ok(compensation);
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received request to create compensation for '{compensation.Employee.EmployeeId}'");
            var result = _compensationService.Create(compensation);
            if (result == null)
            {
                return NotFound();
            }
            return CreatedAtRoute("getCompensationByEmployeeId", new { employeeId = compensation.Employee.EmployeeId }, compensation);
        }
    }
}