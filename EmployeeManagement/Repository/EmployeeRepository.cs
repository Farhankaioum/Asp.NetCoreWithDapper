using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace EmployeeManagement.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private IDbConnection db;

        public EmployeeRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }


        public Employee Add(Employee employee)
        {
            var sql = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int);";

            var insertedId = db.Query<int>(sql, employee).Single();
            employee.EmployeeId = insertedId;

            return employee;
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Employees WHERE EmployeeId = @Id";
            db.Query(sql, new { id });
        }

        public Employee Update(Employee employee)
        {
            var sql = "UPDATE Employees SET Name = @Name, Title = @Title, Email = @Email, Phone = @Phone, CompanyId = @CompanyId WHERE EmployeeId = @EmployeeId";
            db.Execute(sql, employee);

            return employee;
        }

        public List<Employee> GetAll()
        {
            var sql = "SELECT * FROM Employees";
            var listOfEmployee = db.Query<Employee>(sql).ToList();

            return listOfEmployee;
        }

        public Employee Find(int id)
        {
            var sql = "SELECT * FROM Employees WHERE EmployeeId = @EmployeeId";
            var employee = db.Query<Employee>(sql, new{ @EmployeeId = id }).Single();

            return employee;
        }

    }
}
