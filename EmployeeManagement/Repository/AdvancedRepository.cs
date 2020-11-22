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
