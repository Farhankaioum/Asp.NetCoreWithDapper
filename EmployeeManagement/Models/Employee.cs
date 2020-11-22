using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    [Dapper.Contrib.Extensions.Table("Employees")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Title { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        [Write(false)]
        public virtual Company Company { get; set; }
    }
}
