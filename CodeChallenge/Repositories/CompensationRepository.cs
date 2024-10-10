using System;
using CodeChallenge.Data;
using CodeChallenge.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(EmployeeContext employeeContext, ILogger<ICompensationRepository> logger)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.Id = Guid.NewGuid().ToString();
            _employeeContext.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            return _employeeContext.Compensations
                .Include(e => e.Employee)
                // Return the compensation that is the last to currently apply (i.e. most recent effective date before now)
                .Where(e=> e.Employee.EmployeeId == employeeId && e.EffectiveDate <= DateTime.Now).ToList()
                .MaxBy(e=>e.EffectiveDate);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}