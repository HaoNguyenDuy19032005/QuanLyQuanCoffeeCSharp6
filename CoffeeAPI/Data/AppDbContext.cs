using Microsoft.EntityFrameworkCore;
using CoffeeAPI.Models;

namespace CoffeeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập quan hệ 1-n giữa Customer và Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa khách hàng nếu còn đơn hàng

            // Thiết lập quan hệ 1-n giữa PaymentMethod và Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa Payment và Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithMany()
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa Order và OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order sẽ xóa luôn OrderDetail

            // Thiết lập quan hệ 1-n giữa Product và OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa ProductCategory và Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa Employee và Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Products)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa Role và Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ 1-n giữa Product và Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Feedbacks)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Product sẽ xóa luôn Feedback
        }


        /*Giải thích ràng buộc quan hệ
Customer - Order (1-n)

Một khách hàng có thể có nhiều đơn hàng (Orders).
Nếu khách hàng bị xóa, các đơn hàng không bị xóa (DeleteBehavior.Restrict).
PaymentMethod - Payment (1-n)

Một phương thức thanh toán có thể có nhiều thanh toán.
Nếu phương thức thanh toán bị xóa, các thanh toán vẫn giữ nguyên (Restrict).
Payment - Order (1-n)

Một thanh toán có thể liên kết với nhiều đơn hàng.
Không cho phép xóa thanh toán nếu còn đơn hàng (Restrict).
Order - OrderDetail (1-n)

Một đơn hàng có thể có nhiều chi tiết đơn hàng.
Nếu đơn hàng bị xóa, tất cả chi tiết đơn hàng cũng bị xóa (Cascade).
Product - OrderDetail (1-n)

Một sản phẩm có thể xuất hiện trong nhiều đơn hàng.
Nếu sản phẩm bị xóa, chi tiết đơn hàng vẫn giữ nguyên (Restrict).
ProductCategory - Product (1-n)

Một danh mục có thể chứa nhiều sản phẩm.
Không cho phép xóa danh mục nếu còn sản phẩm (Restrict).
Employee - Product (1-n)

Một nhân viên có thể thêm nhiều sản phẩm.
Nếu nhân viên bị xóa, sản phẩm vẫn giữ nguyên (Restrict).
Role - Employee (1-n)

Một vai trò có thể có nhiều nhân viên.
Nếu vai trò bị xóa, nhân viên vẫn giữ nguyên (Restrict).
Product - Feedback (1-n)

Một sản phẩm có thể có nhiều đánh giá.
Nếu sản phẩm bị xóa, tất cả đánh giá cũng bị xóa (Cascade).*/

    }
}
