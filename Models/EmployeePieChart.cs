using System.ComponentModel;

namespace MSAuth.Models
{
    public class EmployeePieChart
    {
        [DisplayName("Departments")]
        public string[] Departments { get; set; }
        [DisplayName("Employement Status")]
        public string[] EmployementStatus { get; set; }
        public int[] DepartmentsData { get; set; }
        public int[] EmployementStatusData { get; set; }
    }
}
