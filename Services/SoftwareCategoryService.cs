using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MSAuth.Interfaces;
using MSAuth.Models;

namespace MSAuth.Services;

public class SoftwareCategoryService : ISoftwareCategoryService
{
    private readonly ServiceClient _service;
    public SoftwareCategoryService(ServiceClient service)
    {
        _service = service;
    }

    public List<SoftwareCategory> GetAllSoftwareCategories()
    {
        QueryExpression query = new("pcf_softwarecategory")
        {
            ColumnSet = new ColumnSet(true),
        };

        EntityCollection result = _service.RetrieveMultiple(query);

        List<SoftwareCategory> softwareCategories = [];

        foreach (var softwareCategory in result.Entities)
        {
            softwareCategories.Add((SoftwareCategory)softwareCategory);
        }

        return softwareCategories;
    }
}
