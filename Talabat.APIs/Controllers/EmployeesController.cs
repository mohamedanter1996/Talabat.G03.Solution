using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Employee_Specs;

namespace Talabat.APIs.Controllers
{

	public class EmployeesController : BaseApiController
	{
		private readonly IGenericRepository<Employee> _employeeRepo;

		public EmployeesController(IGenericRepository<Employee> employeeRepo)
		{
			_employeeRepo = employeeRepo;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>>GetEmployees()
		{
			var Spec=new EmployeeWithDepartmentSpecifications();
			var Employees = await _employeeRepo.GetAllWithSpecAsync(Spec);

			return Ok(Employees);
		}

		[HttpGet("{id}")]

		public async Task<ActionResult<Employee>> GetEmployee(int Id)
		{
			var Spec=new EmployeeWithDepartmentSpecifications(Id);
			var Employee = await _employeeRepo.GetWithSpecAsync(Spec);

			if(Employee == null)
			{
				return NotFound(new ApiResponse(404));
			}

			return Ok(Employee);
		}
	}
}
