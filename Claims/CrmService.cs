using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MSAuth.Claims
{
    public interface ICrmService
    {
        Task<List<string>> GetUserRolesFromCrm(string userId);
    }
    public class CrmService(ServiceClient serviceClient) : ICrmService
    {
        private readonly ServiceClient _serviceClient = serviceClient;

        public Task<List<string>> GetUserRolesFromCrm(string userId)
        {
            var roles = new List<string>();

            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("systemuserid")
            };
            query.Criteria.AddCondition("azureactivedirectoryobjectid", ConditionOperator.Equal, userId);

            Guid systemUserId = _serviceClient.RetrieveMultiple(query)[0].Id;

            var roleQuery = new QueryExpression("role")
            {
                ColumnSet = new ColumnSet("name"),
                LinkEntities =
            {
                new LinkEntity
                {
                    LinkFromEntityName = "role",
                    LinkFromAttributeName = "roleid",
                    LinkToEntityName = "systemuserroles",
                    LinkToAttributeName = "roleid",
                    LinkCriteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.Equal, systemUserId)
                        }
                    }
                }
            }
            };

            var roleEntities = _serviceClient.RetrieveMultiple(roleQuery).Entities;
            roles.AddRange(roleEntities.Select(r => r.GetAttributeValue<string>("name")));


            return Task.FromResult(roles);
        }
    }
}
