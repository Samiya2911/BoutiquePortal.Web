using BoutiquePortal.Repositories.Interfaces;
using BoutiquePortal.Repositories.Repository;
using BoutiquePortal.Services.Interfaces;
using BoutiquePortal.Services.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session (Login Handling)System.IO.InvalidDataException: 'Failed to load configuration from file 'D:\MCA Project\4 10 26 new start\BoutiquePortal.Web\BoutiquePortal.Web\appsettings.json'.'
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//// Dependency Injection (Repositories)
//builder.Services.AddScoped<IVendorRepository, VendorRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();  //System.IO.InvalidDataException: 'Failed to load configuration from file 'D:\MCA Project\4 10 26 new start\BoutiquePortal.Web\BoutiquePortal.Web\appsettings.json'.'

//builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//// builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();


//// Dependency Injection (Services)
//builder.Services.AddScoped<IVendorService, VendorService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

//builder.Services.AddScoped<IAuthService, AuthService>();
//// builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session (IMPORTANT)  
app.UseSession();

app.UseAuthorization();


// AREA Routing (Admin / Vendor)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// AREA Routing (User)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
