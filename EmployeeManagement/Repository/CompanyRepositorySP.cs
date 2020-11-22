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
    public class CompanyRepositorySP : ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepositorySP(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", 0, DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Name", company.Name);
            parameters.Add("@Address", company.Address);
            parameters.Add("@City", company.City);
            parameters.Add("@State", company.State);
            parameters.Add("@PostalCode", company.PostalCode);

            db.Execute("usp_AddCompany", parameters, commandType: CommandType.StoredProcedure);

            company.CompanyId = parameters.Get<int>("CompanyId");

            return company;
        }

        public Company Find(int id)
        {
            var sql = "usp_GetCompany";
            var company = db.Query<Company>(sql, new { CompanyId = id }, commandType: CommandType.StoredProcedure).Single();

            return company;
        }

        public List<Company> GetAll()
        {
            var sql = "usp_GetALLCompany";
            var allCompanies = db.Query<Company>(sql, commandType: CommandType.StoredProcedure).ToList();

            return allCompanies;
        }

        public void Remove(int id)
        {
            db.Execute("usp_RemoveCompany", new { @CompanyId = id}, commandType: CommandType.StoredProcedure);
        }

        public Company Update(Company company)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", company.CompanyId, DbType.Int32);
            parameters.Add("@Name", company.Name);
            parameters.Add("@Address", company.Address);
            parameters.Add("@City", company.City);
            parameters.Add("@State", company.State);
            parameters.Add("@PostalCode", company.PostalCode);

            db.Execute("usp_UpdateCompany", parameters, commandType: CommandType.StoredProcedure);

            return company;
        }
    }
}
