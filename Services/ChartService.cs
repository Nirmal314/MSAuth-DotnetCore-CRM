using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using MSAuth.Interfaces;
using Microsoft.PowerPlatform.Dataverse.Client;
using DocumentFormat.OpenXml.Bibliography;

namespace MSAuth.Services
{
    public class ChartService : IChartService
    {
        private readonly ServiceClient _service;
        public ChartService(ServiceClient service)
        {
            _service = service;
        }
        public int GetContactCount(string attribute, int value)
        {
            string fetchXml = $@"
                <fetch aggregate='true'>
                  <entity name='contact'>
                    <attribute name='contactid' aggregate='count' alias='contact_count' />
                    <filter type='and'>
                      <condition attribute='{attribute}' operator='eq' value='{value}' />
                    </filter>
                  </entity>
                </fetch>";

            var result = _service.RetrieveMultiple(new FetchExpression(fetchXml));

            if (result.Entities.Count > 0)
            {
                var count = result.Entities[0].GetAttributeValue<AliasedValue>("contact_count").Value;
                return (int)count;
            }
            else
            {
                return 0;
            }
        }
    }
}
