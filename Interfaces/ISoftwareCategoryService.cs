using MSAuth.Models;

namespace MSAuth.Interfaces;

public interface ISoftwareCategoryService
{
    public List<SoftwareCategory> GetAllSoftwareCategories();
}
