using Dapper;
using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

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

        public List<Employee> GetAllEmployeeWithCompany(int id)
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

        public void AddTestCompanyWithEmployee(Company objComp)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int); ";

            var id = db.Query<int>(sql, objComp).Single();

            objComp.CompanyId = id;

            //foreach (var employee in objComp.Employees)
            //{
            //    employee.CompanyId = objComp.CompanyId;
            //    var sql1 = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
            //            "SELECT CAST(SCOPE_IDENTITY() as int);";

            //    db.Query<int>(sql1, employee).Single();
            //} // one way to insert navigation property

            // another way to insert navigation property
            objComp.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();
            var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
                      "SELECT CAST(SCOPE_IDENTITY() as int);";
            db.Execute(sqlEmp, objComp.Employees);
        }

        public void AddTestCompanyWithEmployeesWithTransaction(Company objComp)
        {
            using (var tranction = new TransactionScope())
            {
                try
                {
                    var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int); ";

                    var id = db.Query<int>(sql, objComp).Single();
                    objComp.CompanyId = id;

                    objComp.Employees.Select(c => { c.CompanyId = id; return c; }).ToList(); // set CompanyId in each employee

                    var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
                              "SELECT CAST(SCOPE_IDENTITY() as int);";

                    db.Execute(sqlEmp, objComp.Employees);

                    tranction.Complete();
                }
                catch
                {
                    tranction.Dispose();
                }
            }

            
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("Delete From Companies Where CompanyId In @companyId", new { companyId });
        }

        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("Select * From Companies Where Name like '%' + @name + '%'", new { name = name}).ToList();
        }
    }
}
