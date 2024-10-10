namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        public int numberOfReports { get; set; }
        public ReportingStructure(Employee employee, int numberOfReports)
        {
            Employee = employee;
            this.numberOfReports = numberOfReports;
        }
    }
}