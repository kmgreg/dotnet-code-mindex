using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        /**
         * Get all employees who report directly to or report to someone under the provided employee
         */
        private int GetAllReports(Employee emp)
        {
            int total = 0;
            List<Employee> employees = emp.DirectReports;
            // Loop instead of recursive to save memory
            while (employees.Count > 0)
            {
                total += employees.Count;
                List<Employee> nextLevel = new List<Employee>();
                foreach (Employee report in employees)
                {
                    // compensate for EF not including a deep copy of the report structure
                    var fullEmployee = _employeeRepository.GetById(report.EmployeeId);
                    if (fullEmployee.DirectReports != null)
                    {
                        nextLevel.AddRange(fullEmployee.DirectReports);
                    }
                }
                employees = nextLevel;
            }
            return total;
        }

        public ReportingStructure GetReportingStructureById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Employee employee = _employeeRepository.GetById(id);
                if (employee != null)
                {
                    int totalReports = GetAllReports(employee);
                    return new ReportingStructure(employee, totalReports);
                }

            }

            return null;
        }
    }
}
