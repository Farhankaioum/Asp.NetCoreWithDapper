using Dapper;
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
    public class AdvancedRepository : IAdvancedRepository
    {
        private IDbConnection db;

        public AdvancedRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public List<Company> GetAllCompanyWithEmployees()
        {
            var sql = "select C.*,E.* from Companies AS C Inner join Employees AS E On C.CompanyId = E.CompanyId";

            var companyDic = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (com, emp) =>
            {
                if (!companyDic.TryGetValue(com.CompanyId, out var currentCompany))
                {
                    currentCompany = com;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(emp);
                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id
            };

            var sql = "Select * From Companies Where CompanyId = @CompanyId;" +
                      " Select * From Employees Where CompanyId = @CompanyId;";
            Company company;

            using (var lists = db.QueryMultiple(sql, p))
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            }

            return company;
        }

        public List<Employee> GetEmployeeWithCompany(int id)
        {
            var sql = "select E.*,C.* from Employees AS E Inner join Companies AS C On E.CompanyId = C.CompanyId";

            if (id != 0)
            {
                sql += " Where E.CompanyId = @Id";
            }

            var employee = db.Query<Employee, Company, Employee>(sql, (emp, com) => // maping between two relational table
            {
                emp.Company = com;
                return emp;
            }, new { id }, splitOn: "CompanyId");

            return employee.ToList();
        }
    }
}
