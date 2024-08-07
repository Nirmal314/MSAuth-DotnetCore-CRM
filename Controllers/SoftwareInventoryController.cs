﻿using DocumentFormat.OpenXml.Bibliography;
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
                CurrentSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString(), ((int)pcf_software_pcf_softwaretype.Current)),
                FutureSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString(), ((int)pcf_software_pcf_softwaretype.Future)),
                OtherSoftwares = _softwareService.GetSoftwaresBySoftwareCategory(item.pcf_SoftwareCategoryId.ToString(), ((int)pcf_software_pcf_softwaretype.Others))
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

    public IActionResult GetSoftwaresBySoftwareCategory(string scId)
    {
        List<Software> softwares = _softwareService.GetSoftwaresBySoftwareCategory(scId, ((int)pcf_software_pcf_softwaretype.Current));
        List<Software> softwares1 = _softwareService.GetSoftwaresBySoftwareCategory(scId, ((int)pcf_software_pcf_softwaretype.Future));
        List<Software> softwares2 = _softwareService.GetSoftwaresBySoftwareCategory(scId, ((int)pcf_software_pcf_softwaretype.Others));
        List<Software> softwares3 = _softwareService.GetSoftwaresBySoftwareCategory(scId);
        return Json(new { softwares });
    }
}