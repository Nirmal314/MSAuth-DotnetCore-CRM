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

    public bool UpdateSoftwareType(string softwareId, int prevSoftwareType, int targetSoftwareType)
    {
        try
        {
            Entity record = _service.Retrieve("pcf_software", new Guid(softwareId), new ColumnSet("pcf_softwaretype"));

            if (record.Contains("pcf_softwaretype"))
            {
                OptionSetValueCollection prevSoftwareTypes = ((OptionSetValueCollection)record["pcf_softwaretype"]);

                if (!prevSoftwareTypes.Any(t => t.Value.Equals(targetSoftwareType)) && prevSoftwareType != targetSoftwareType)
                {
                    prevSoftwareTypes.Remove(new OptionSetValue(prevSoftwareType));
                    prevSoftwareTypes.Add(new OptionSetValue(targetSoftwareType));

                    record["pcf_softwaretype"] = prevSoftwareTypes;
                    _service.Update(record);

                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

}