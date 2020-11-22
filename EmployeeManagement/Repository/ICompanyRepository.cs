using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public interface ICompanyRepository
    {
        Company Find(int id);
        List<Company> GetAll();

        Company Add(Company company);
        Company Update(Company company);

        void Remove(int id);
    }
}
