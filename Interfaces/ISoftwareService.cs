using MSAuth.Models;

namespace MSAuth.Interfaces;

public interface ISoftwareService
{
    public List<Software> GetAllSoftwares();
    public Software GetSoftwareById(string softwareId);
    public byte[] GetIconBytes(string softwareId);
    public List<Software> GetSoftwaresBySoftwareCategory(string scId, int? softwareType = null);
    public bool UpdateSoftwareType(string softwareId, int prevSoftwareType, int targetSoftwareType);
}
