using KaiWeb.DataAccess.Data;
using KaiWeb.DataAccess.Repository;
using KaiWeb.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using KaiWeb.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(option=>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();

// 當有人沒有身分認證又想透過網址進入限制頁面時，會將其導向以下頁面
builder.Services.ConfigureApplicationCookie(option =>
{
    // 如果該使用這沒有登入就導向登入頁面
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = "/Identity/Account/Logout";
    // 如果該使用者權限不足就導向警告通知頁面
    option.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 由於預設的使用者登入介面適用RazorPages的方式建立的，所以要先開通有關RazorPages的功能
builder.Services.AddRazorPages();
// 這邊是用來註冊我們事先建好的功能
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

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
// 要先驗證使用者
app.UseAuthentication();
// 才可以授權使用網站
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    // 這邊就是我們的首頁網址設定
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
