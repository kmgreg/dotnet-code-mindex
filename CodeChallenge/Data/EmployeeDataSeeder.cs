using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const String EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";
        private const String COMPENSATION_SEED_DATA_FILE = "resources/CompensationSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Employees.Any())
            {
                List<Employee> employees = LoadEmployees();
                await _employeeContext.Employees.AddRangeAsync(employees);

                await _employeeContext.SaveChangesAsync();

                // Load the test compensation
                List<Compensation> compensations = LoadCompensations();
                await _employeeContext.Compensations.AddRangeAsync(compensations);
                await _employeeContext.SaveChangesAsync();

            }
        }

        private List<Employee> LoadEmployees()
        {
            using (FileStream fs = new FileStream(EMPLOYEE_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Employee> employees = serializer.Deserialize<List<Employee>>(jr);
                FixUpReferences(employees);

                return employees;
            }
        }

        private void FixUpReferences(List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                select new { Id = employee.EmployeeId, EmployeeRef = employee };

            employees.ForEach(employee =>
            {
                
                if (employee.DirectReports != null)
                {
                    var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                    foreach (Employee report in employee.DirectReports)
                    {
                        var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                        referencedEmployees.Add(referencedEmployee);
                    }
                    employee.DirectReports = referencedEmployees;
                }
            });
        }

        private List<Compensation> LoadCompensations()
        {
            using (FileStream fs = new FileStream(COMPENSATION_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Compensation> compensations = serializer.Deserialize<List<Compensation>>(jr);

                AddFullEmployees(compensations);
                return compensations;
            }
        }

        // Need to add correct employee references
        private void AddFullEmployees(List<Compensation> compensations)
        {
            compensations.ForEach(compensation =>
            {
                var employee = _employeeContext.Employees.Include(e => e.DirectReports)
                    .FirstOrDefault(e=> e.EmployeeId == compensation.Employee.EmployeeId);
                compensation.Employee = employee;
                compensation.Id = Guid.NewGuid().ToString();
            });
        }
    }
}
