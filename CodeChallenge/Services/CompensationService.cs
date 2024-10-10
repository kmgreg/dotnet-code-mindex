using CodeChallenge.Models;
using System;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;


namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _compensationRepository = compensationRepository;
            _employeeRepository = employeeRepository;
        }

        public Compensation Create(Compensation compensation)
        {
            // check if employee exists
            var employee = _employeeRepository.GetById(compensation.Employee.EmployeeId);
            if (employee == null)
            {
                return null;
            }
            compensation.Employee = employee;
            if (compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
            return compensation;
        }

        public Compensation GetByEmployeeId(String employeeId)
        {
            if (!String.IsNullOrEmpty(employeeId))
            {
                return _compensationRepository.GetByEmployeeId(employeeId);
            }
            return null;
        }
    }
}