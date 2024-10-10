using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


using System.Net;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;
        private static Employee _dummyEmployee;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
            _dummyEmployee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Ok()
        {
            // Arrange
            var salary = 1234;
            var effectiveDate = DateTime.Parse("2023-01-06T17:16:40");

            var compensation = new Compensation()
            {
                Employee = _dummyEmployee,
                Salary = salary,
                EffectiveDate = effectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(salary, newCompensation.Salary);
            Assert.AreEqual(effectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(_dummyEmployee.EmployeeId, newCompensation.Employee.EmployeeId);
        }

        [TestMethod]
        public void CreateCompensation_Employee_Doesnt_Exist()
        {
            // Arrange
            var salary = 1234;
            var effectiveDate = DateTime.Parse("2023-01-06T17:16:40");

            var fakeEmployee = new Employee()
            {
                EmployeeId = "BadID"
            };

            var compensation = new Compensation()
            {
                Employee = fakeEmployee,
                Salary = salary,
                EffectiveDate = effectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensation_Does_Exist()
        {
            // Arrange
            var salary = 1234;
            var effectiveDate = DateTime.Parse("2023-01-06T17:16:40");

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{_dummyEmployee.EmployeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(salary, newCompensation.Salary);
            Assert.AreEqual(effectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(_dummyEmployee.EmployeeId, newCompensation.Employee.EmployeeId);
        }

        [TestMethod]
        public void GetCompensation_Does_Not_Exist()
        {

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/badID");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensation_Gets_Most_Recent()
        {
            // First we'll make an "older" effective date compensation and add it to the db
            var salary = 123;
            var effectiveDate = DateTime.Parse("2019-01-06T17:16:40");

            var compensation = new Compensation()
            {
                Employee = _dummyEmployee,
                Salary = salary,
                EffectiveDate = effectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;

            // Then Perform the get
            var expectedSalary = 1234;
            var expectedEffectiveDate = DateTime.Parse("2023-01-06T17:16:40");

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{_dummyEmployee.EmployeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            // Should be equal to original seed data
            var newCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, newCompensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(_dummyEmployee.EmployeeId, newCompensation.Employee.EmployeeId);
        }

        [TestMethod]
        public void GetCompensation_Gets_Current()
        {
            // First we'll make an effective date in the future and add it to the db
            var salary = 123;
            var effectiveDate = DateTime.Parse("2029-01-06T17:16:40");

            var compensation = new Compensation()
            {
                Employee = _dummyEmployee,
                Salary = salary,
                EffectiveDate = effectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;

            // Then Perform the get
            var expectedSalary = 1234;
            var expectedEffectiveDate = DateTime.Parse("2023-01-06T17:16:40");

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{_dummyEmployee.EmployeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            // Should be equal to original seed data
            var newCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, newCompensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(_dummyEmployee.EmployeeId, newCompensation.Employee.EmployeeId);
        }
    }
}
