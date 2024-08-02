using MSAuth.Models;

namespace MSAuth.Interfaces
{
    public interface IFileService
    {
        public byte[] PopulateWordTemplate(string templatePath, Contact contact);
        public byte[] GenerateTasksPdf(List<CrmTask> tasks);
    }
}
