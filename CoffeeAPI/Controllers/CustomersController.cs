using CoffeeAPI.Data;
using CoffeeAPI.HashHelpers;
using CoffeeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-customer-role/{phoneNumber}")]
        public async Task<IActionResult> GetCustomerRoleByPhoneNumber(string phoneNumber)
        {
            var exists = await _context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber);

            if (!exists)
            {
                return NotFound();
            }

            return Ok("Customer"); // 🔥 Nếu tìm thấy khách hàng, gán vai trò "Customer"
        }

        /*[HttpGet("GetByPhoneNumber/{phoneNumber}")]
        public ActionResult<Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (customer == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(customer); // Return the customer object
        }*/

        [HttpGet("GetByPhoneNumber/{phoneNumber}")]
        public ActionResult<Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var customer = _context.Customers
                .FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);  // Return customer data
        }


        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            if (!customers.Any())
            {
                return NotFound("Không có khách hàng nào");
            }
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound($"Không tìm thấy khách hàng với ID = {id}");
            }
            return Ok(customer);
        }

        /* // POST: api/Customers
         [HttpPost]
         public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
         {
             if (string.IsNullOrEmpty(customer.FullName))
             {
                 return BadRequest("Tên khách hàng không được để trống.");
             }

             _context.Customers.Add(customer);
             await _context.SaveChangesAsync();

             return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
         }*/

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            // Kiểm tra nếu email đã tồn tại
            if (_context.Customers.Any(c => c.Email == customer.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            // Kiểm tra nếu số điện thoại đã tồn tại
            if (_context.Customers.Any(c => c.PhoneNumber == customer.PhoneNumber))
            {
                return BadRequest("Số điện thoại đã tồn tại.");
            }

            // Mã hóa mật khẩu trước khi lưu
            customer.PassWord = HashHelper.ComputeSha256Hash(customer.PassWord);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }


        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("ID không khớp.");
            }

            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound($"Không tìm thấy khách hàng với ID = {id}");
            }

            existingCustomer.FullName = customer.FullName;
            existingCustomer.Email = customer.Email;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.PassWord = customer.PassWord;
            existingCustomer.Address = customer.Address;
            existingCustomer.DateOfBirth = customer.DateOfBirth;
            existingCustomer.IsActive = customer.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound($"Không tìm thấy khách hàng với ID = {id}");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
