using Dapper;
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
    public class CompanyRepository : ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);SELECT CAST(SCOPE_IDENTITY() as int); ";
            var insertedId = db.Query<int>(sql, new 
              { 
                @Name = company.Name,
                @Address = company.Address,
                @City = company.City,
                @State = company.State,
                @PostalCode = company.PostalCode
            }).FirstOrDefault();

            //db.Query<int>(sql, company); // another way to execute query

            company.CompanyId = insertedId;

            return company;
        }

        public Company Find(int id)
        {
            var sql = "select * from Companies where CompanyId = @CompanyId";
            var company = db.Query<Company>(sql, new { @CompanyId =id}).FirstOrDefault();

            return company;
        }

        public List<Company> GetAll()
        {
            var sql = "select * from Companies";
            var allCompanies = db.Query<Company>(sql).ToList();

            return allCompanies;
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            db.Execute(sql, new { @Id = id});
        }

        public Company Update(Company company)
        {
            var sql = "UPDATE Companies SET Name = @Name, Address = @Address, City = @City, State = @State, PostalCode = @PostalCode WHERE CompanyId = @CompanyId";
            db.Execute(sql, company);

            return company;
        }
    }
}
