using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using MSAuth.Filters;
using MSAuth.Interfaces;
using MSAuth.Models;
using MSAuth.Services;
using System.Security.Claims;
using System.Text;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace MSAuth.Controllers;

[AuthorizeRoles("Employee", "Admin")]
public class TaskController : Controller
{
    private readonly ServiceClient _service;
    private readonly ITaskService _taskService;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;


    public TaskController(ServiceClient service, ITaskService taskService, IFileService fileService, IEmailService emailService)
    {
        _service = service;
        _taskService = taskService;
        _fileService = fileService;
        _emailService = emailService;
        //employeeId = HttpContext.Session.GetString("UserId")!;
    }
    public IActionResult Index()
    {
        List<CrmTask> tasks = _taskService.GetTasksByEmployeeId(HttpContext.Session.GetString("UserId")!);
        return View(tasks);
    }
    public IActionResult ExportTasksToPdf()
    {
        List<CrmTask> tasks = _taskService.GetTasksByEmployeeId(HttpContext.Session.GetString("UserId")!);

        byte[] tasksPdf = _fileService.GenerateTasksPdf(tasks);

        return File(tasksPdf, "application/pdf", "Tasks.pdf");
    }
}
