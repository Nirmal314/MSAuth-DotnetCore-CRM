using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using MSAuth.Interfaces;
using MSAuth.Models;
using MSAuth.ViewModels;

namespace MSAuth.Controllers;

public class SoftwareInventoryController : Controller
{
    private readonly ISoftwareService _softwareService;
    private readonly ISoftwareCategoryService _softwareCategoryService;
    public SoftwareInventoryController(ISoftwareService softwareService, ISoftwareCategoryService softwareCategoryService)
    {
        _softwareService = softwareService;
        _softwareCategoryService = softwareCategoryService;
    }

    public IActionResult Index()
    {
        List<SoftwareCategory> softwareCategories = _softwareCategoryService.GetAllSoftwareCategories();

        List<SoftwareInventoryViewModel> lvm = [];

        foreach (var item in softwareCategories)
        {
            SoftwareInventoryViewModel vm = new()
            {
                SoftwareCategory = item,
                CurrentSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString()!, ((int)pcf_software_pcf_softwaretype.Current)),
                FutureSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString()!, ((int)pcf_software_pcf_softwaretype.Future)),
                OtherSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString()!, ((int)pcf_software_pcf_softwaretype.Others))
            };

            lvm.Add(vm);
        }
        return View(lvm);
    }

    public IActionResult GetIcon(string softwareId)
    {
        byte[] iconBytes = _softwareService.GetIconBytes(softwareId);

        iconBytes ??= [];

        return File(iconBytes, "image/png");
    }

    [HttpPost]
    public IActionResult UpdateSoftwareType(string softwareId, int prevSoftwareType, int targetSoftwareType)
    {
        bool isUpdated = _softwareService.UpdateSoftwareType(softwareId, prevSoftwareType, targetSoftwareType);
        return Json(new { success = isUpdated });
    }
    [HttpPost]
    public IActionResult CreateSoftware(string category, string softwareName, int softwareType, IFormFile iconFile)
    {
        bool isCreated = _softwareService.CreateSoftware(category, softwareName, softwareType, iconFile);
        return Json(new { success = isCreated });
    }

    [HttpPost]
    public IActionResult AddSoftware(string category, string software, int softwareType)
    {
        bool isAdded = _softwareService.AddSoftware(category, software, softwareType);

        if (isAdded)
        {
            var updatedSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(category, softwareType);
            return Json(new { success = true, softwares = updatedSoftwares });
        }

        return Json(new { success = false });
    }

    public IActionResult GetSoftwaresBySoftwareCategory(string softwareCategoryId)
    {
        List<Software> softwares = _softwareService.GetSoftwaresBySoftwareCategory(softwareCategoryId);
        return Json(new { softwares, success = true });
    }
}
