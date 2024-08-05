using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MSAuth.Filters;
using MSAuth.Interfaces;
using MSAuth.Models;
using MSAuth.Services;
namespace MSAuth.Controllers;

[AuthorizeRoles("Admin")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IFileService _fileService;

    public EmployeeController(IEmployeeService employeeService, IFileService fileService)
    {
        _employeeService = employeeService;
        _fileService = fileService;
    }

    public IActionResult Index()
    {
        string? sort = HttpContext.Request.Query["sort"];
        List<Contact> contacts = [];

        contacts = sort switch
        {
            "eaz" => _employeeService.GetSortedEmployees("eaz"),
            "eza" => _employeeService.GetSortedEmployees("eza"),
            "hno" => _employeeService.GetSortedEmployees("hno"),
            "hon" => _employeeService.GetSortedEmployees("hon"),
            _ => _employeeService.GetAllEmployees(),
        };
        return View(contacts);
    }

    public IActionResult SearchEmployees(string employeeName)
    {
        if (string.IsNullOrEmpty(employeeName))
        {
            List<Contact> allContacts = _employeeService.GetAllEmployees();
            return Json(new { contacts = allContacts, sucess = false, message = "Searched employee is empty." });
        }

        List<Contact> contacts = _employeeService.SearchEmployees(employeeName);

        if (contacts.Count <= 0)
        {
            return Json(new { sucess = false, message = "No employees found for " + employeeName });
        }

        return Json(new { contacts, success = true });
    }

    public IActionResult ExportWord(string employeeId)
    {
        Contact employee = _employeeService.GetEmployeeById(employeeId);

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/words", "EmployeeDetails.docx");

        byte[] documentBytes = _fileService.PopulateWordTemplate(templatePath, employee);

        return File(documentBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", employee.FirstName + "-details.docx");
    }
}

