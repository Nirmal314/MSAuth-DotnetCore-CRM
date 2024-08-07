using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Rest;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MSAuth.Interfaces;
using MSAuth.Models;

namespace MSAuth.Services;

public class SoftwareService : ISoftwareService
{
    private readonly ServiceClient _service;
    public SoftwareService(ServiceClient service)
    {
        _service = service;
    }
    public List<Software> GetAllSoftwares()
    {
        List<Software> softwares = [];

        QueryExpression query = new("pcf_software")
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression()
        };

        EntityCollection result = _service.RetrieveMultiple(query);

        foreach (var software in result.Entities)
        {
            softwares.Add((Software)software);
        }

        return softwares;
    }
    public Software GetSoftwareById(string softwareId)
    {
        Entity software = _service.Retrieve("pcf_software", new Guid(softwareId), new ColumnSet(true));

        return (Software)software;
    }
    public byte[] GetIconBytes(string softwareId)
    {
        Entity result = _service.Retrieve("pcf_software", new Guid(softwareId), new ColumnSet("pcf_icon"));

        byte[] iconBytes = result.GetAttributeValue<byte[]>("pcf_icon");

        return iconBytes;
    }

    public List<Software> GetSoftwaresBySoftwareCategory(string scId, int? softwareType = null)
    {
        List<Software> softwares = [];



        QueryExpression query = new("pcf_software")
        {
            ColumnSet = new ColumnSet("pcf_softwareid", "pcf_icon", "pcf_name", "pcf_softwarecategory"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions = {
                    new ConditionExpression("pcf_softwarecategory", ConditionOperator.Equal, new Guid(scId))
                }
            }
        };

        if (softwareType != null)
        {
            query.Criteria.AddCondition("pcf_softwaretype", ConditionOperator.ContainValues, softwareType);
        }

        EntityCollection result = _service.RetrieveMultiple(query);

        foreach (var software in result.Entities)
        {
            softwares.Add((Software)software);
        }

        return softwares;
    }
}

//https://localhost:7071/SoftwareInventory/GetSoftwaresBySoftwareCategory?scId=4a45ec6e-f053-ef11-bfe3-6045bd73754e