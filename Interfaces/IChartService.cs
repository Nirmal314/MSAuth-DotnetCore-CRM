using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace MSAuth.Interfaces
{
    public interface IChartService
    {
        public int GetContactCount(string attribute, int value);
    }
}
