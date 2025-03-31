using CoffeeAPI.Data;
using CoffeeAPI.HashHelpers;
using CoffeeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CoffeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ API đổi mật khẩu
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest model)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên." });
            }

            // Kiểm tra mật khẩu cũ
            string hashedOldPassword = HashHelper.ComputeSha256Hash(model.OldPassword);
            if (employee.PassWord != hashedOldPassword)
            {
                return BadRequest(new { message = "Mật khẩu cũ không chính xác." });
            }

            // Cập nhật mật khẩu mới
            employee.PassWord = HashHelper.ComputeSha256Hash(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            var Employees = await _context.Employees.ToListAsync();
            if (!Employees.Any())
            {
                return NotFound("Không có nhân viên nào");
            }
            return Ok(Employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var Employees = await _context.Employees.FindAsync(id);
            if (Employees == null)
            {
                return NotFound($"Không tìm thấy nhân viên với ID = {id}");
            }
            return Ok(Employees);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.FullName))
            {
                return BadRequest("Tên nhân viên không được để trống.");
            }

            if (string.IsNullOrEmpty(employee.PassWord))
            {
                return BadRequest("Mật khẩu không được để trống.");
            }

            // Mã hóa mật khẩu trước khi lưu
            employee.PassWord = HashHelper.ComputeSha256Hash(employee.PassWord);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("ID không khớp.");
            }

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound($"Không tìm thấy nhân viên với ID = {id}");
            }

            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.PhoneNumber = employee.PhoneNumber;

            // Kiểm tra nếu người dùng muốn thay đổi mật khẩu thì mới mã hóa lại
            if (!string.IsNullOrEmpty(employee.PassWord))
            {
                existingEmployee.PassWord = HashHelper.ComputeSha256Hash(employee.PassWord);
            }

            existingEmployee.Address = employee.Address;
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            existingEmployee.IsActive = employee.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/ProductCategories/5 (Xóa danh mục)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var Customers = await _context.Customers.FindAsync(id);
            if (Customers == null)
            {
                return NotFound($"Không tìm thấy danh mục với ID = {id}");
            }

            _context.Customers.Remove(Customers);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        /*
                [HttpGet("GetByPhoneNumber/{phoneNumber}")]
                public ActionResult<Employee> GetEmployeeByPhoneNumber(string phoneNumber)
                {
                    var employee = _context.Employees
                        .Include(e => e.Role) // Đảm bảo JOIN bảng Role để lấy tên vai trò
                        .FirstOrDefault(e => e.PhoneNumber == phoneNumber);

                    if (employee == null)
                    {
                        return NotFound();
                    }

                    Console.WriteLine($"✅ API trả về Employee - ID: {employee.Id}, Role: {employee.Role?.Name}");
                    return Ok(employee);
                }*/

        [HttpGet("GetByPhoneNumber/{phoneNumber}")]
        public ActionResult<Employee> GetEmployeeByPhoneNumber(string phoneNumber)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.PhoneNumber == phoneNumber);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);  // Return employee data
        }


    }
}
