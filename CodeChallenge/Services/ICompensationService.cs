using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation GetByEmployeeId(String employeeId);
        Compensation Create(Compensation compensation);
    }
}