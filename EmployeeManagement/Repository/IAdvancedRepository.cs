using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public interface IAdvancedRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);
        Company GetCompanyWithAddress(int id);
    }
}
