using EmployeeManagement.Data;
using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public class CompanyRepositoryEF : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepositoryEF(AppDbContext context)
        {
            _context = context;
        }

        public Company Add(Company company)
        {
            _context.Companies.Add(company);
            _context.SaveChanges();

            return company;
        }

        public Company Find(int id)
        {
            var existingCompany = _context.Companies.FirstOrDefault(c => c.CompanyId == id);
            return existingCompany;
        }

        public List<Company> GetAll()
        {
            var companies = _context.Companies.ToList();
            return companies;
        }

        public void Remove(int id)
        {
            var existingCompany = Find(id);

            _context.Remove(existingCompany);
            _context.SaveChanges();
        }

        public Company Update(Company company)
        {
            var existingCompany = Find(company.CompanyId);
            existingCompany.Name = company.Name;
            existingCompany.Address = company.Address;
            existingCompany.City = company.City;
            existingCompany.State = company.State;
            existingCompany.PostalCode = company.PostalCode;

            _context.SaveChanges();

            return company;
        }
    }
}
