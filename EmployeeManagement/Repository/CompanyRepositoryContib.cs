using Dapper;
using Dapper.Contrib.Extensions;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public class CompanyRepositoryContib : ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepositoryContib(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var id = db.Insert(company);

            company.CompanyId = (int)id;
            return company;
        }

        public Company Find(int id)
        {
            var company = db.Get<Company>(id);
            return company;
        }

        public List<Company> GetAll()
        {
            var allCompanies = db.GetAll<Company>().ToList();

            return allCompanies;
        }

        public void Remove(int id)
        {
            db.Delete(new Company { CompanyId = id}) ;
        }

        public Company Update(Company company)
        {
             db.Update(company);

            return company;
        }
    }
}
