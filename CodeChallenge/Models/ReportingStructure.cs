namespace CodeChallenge.Models
{
    // made this a model in case we need to save it in the future
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