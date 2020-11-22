using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly ICompanyRepository _companyRepo;
        private readonly IAdvancedRepository _adRepo;

        public EmployeeController(IEmployeeRepository repository, ICompanyRepository companyRepo, IAdvancedRepository adRepo)
        {
            _repository = repository;
            _companyRepo = companyRepo;
            _adRepo = adRepo;
        }

        public IActionResult Index(int companyId=0)
        {
            //var allEmployee = _repository.GetAll();
            //foreach (var emp in allEmployee) // n+1 way to iterate/mapping two relational table
            //{
            //    emp.Company = _companyRepo.Find(emp.CompanyId);
            //}
            //return View(allEmployee);

            var employee = _adRepo.GetAllEmployeeWithCompany(companyId);
            return View(employee);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString()
                });

            ViewBag.CompanyList = companyList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(employee);

                return RedirectToAction(nameof(Index));
            }

            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString()
                });

            ViewBag.CompanyList = companyList;

            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _repository.Find(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString()
                });

            ViewBag.CompanyList = companyList;

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repository.Update(employee);
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString()
                });

            ViewBag.CompanyList = companyList;

            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _repository.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
