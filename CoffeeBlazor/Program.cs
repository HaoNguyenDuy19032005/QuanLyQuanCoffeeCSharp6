using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CoffeeBlazor;
using CoffeeBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Chỉ cần một HttpClient với base address mặc định (Blazor WebAssembly URL)
builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });

// Đăng ký các dịch vụ cần thiết
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<EmployeeService>();


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");




await builder.Build().RunAsync();
