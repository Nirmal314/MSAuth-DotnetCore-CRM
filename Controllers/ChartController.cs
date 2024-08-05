using Microsoft.AspNetCore.Mvc;
using MSAuth.Interfaces;
using MSAuth.Models;
using MSAuth.Services;

namespace MSAuth.Controllers
{
    public class ChartController : Controller
    {
        private readonly IChartService _chartService;
        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }
        public IActionResult Employees()
        {
            int hrCount = _chartService.GetContactCount("hiring_department", 414090000); //? HR
            int salesCount = _chartService.GetContactCount("hiring_department", 414090001); //? Sales
            int itCount = _chartService.GetContactCount("hiring_department", 414090002); //? IT
            int marketingCount = _chartService.GetContactCount("hiring_department", 414090003); //? Marketing

            int probationCount = _chartService.GetContactCount("hiring_employmentstatus", 414090000); //? Probation
            int employeeCount = _chartService.GetContactCount("hiring_employmentstatus", 414090001); //? Employee
            int exEmployeeCount = _chartService.GetContactCount("hiring_employmentstatus", 414090002); //? Ex-Employee

            EmployeePieChart model = new()
            {
                Departments = ["HR", "Sales", "IT", "Marketing"],
                EmployementStatus = ["Probation", "Employee", "Ex-Employee"],
                DepartmentsData = [hrCount, salesCount, itCount, marketingCount],
                EmployementStatusData = [probationCount, employeeCount, exEmployeeCount]
            };

            return View(model);
        }
    }
}
