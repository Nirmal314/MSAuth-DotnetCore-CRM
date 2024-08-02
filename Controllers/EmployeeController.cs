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
        List<Contact> contacts = _employeeService.GetAllEmployees();

        return View(contacts);
    }

    public IActionResult ExportWord(string employeeId)
    {
        Contact employee = _employeeService.GetEmployeeById(employeeId);

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/words", "EmployeeDetails.docx");

        byte[] documentBytes = _fileService.PopulateWordTemplate(templatePath, employee);

        return File(documentBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", employee.FirstName + "-details.docx");
    }
}

