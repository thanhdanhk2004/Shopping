using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Areas.Admin.Reponsitory;
using Shopping.Models;
using Shopping.Models.Momo;
using Shopping.Reponitory;
using Shopping.Services.Momo;
using Shopping.Services.VnPay;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load file .env
Env.Load("Key.env");

// Gán giá trị từ file .env vào Configuration
builder.Configuration["MomoAPI:MomoApiUrl"] = Env.GetString("MomoApiUrl");
builder.Configuration["MomoAPI:SecretKey"] = Env.GetString("SecretKey");
builder.Configuration["MomoAPI:AccessKey"] = Env.GetString("AccessKey");
builder.Configuration["MomoAPI:ReturnUrl"] = Env.GetString("ReturnUrl");
builder.Configuration["MomoAPI:NotifyUrl"] = Env.GetString("NotifyUrl");
builder.Configuration["MomoAPI:PartnerCode"] = Env.GetString("PartnerCode");
builder.Configuration["MomoAPI:RequestType"] = Env.GetString("RequestType");

builder.Configuration["GoogleKeys:ClientID"] = Env.GetString("ClientID");
builder.Configuration["GoogleKeys:ClientSecret"] = Env.GetString("ClientSecret");

builder.Configuration["Vnpay:TmnCode"] = Env.GetString("TmnCode");
builder.Configuration["Vnpay:HashSecret"] = Env.GetString("HashSecret");
builder.Configuration["Vnpay:BaseUrl"] = Env.GetString("BaseUrl");
builder.Configuration["Vnpay:Command"] = Env.GetString("Command");
builder.Configuration["Vnpay:CurrCode"] = Env.GetString("CurrCode");
builder.Configuration["Vnpay:Version"] = Env.GetString("Version");
builder.Configuration["Vnpay:Locale"] = Env.GetString("Locale");
builder.Configuration["Vnpay:PaymentBackReturnUrl"] = Env.GetString("PaymentBackReturnUrl");
builder.Configuration["TimeZoneId"] = Env.GetString("TimeZoneId");


//Dang ky thanh toan momo
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

//Dang ky thanh toan VnPay
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Add services to the container.
builder.Services.AddControllersWithViews();



//Ket noi den database
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb"));
});

// Dang ky gui mail
builder.Services.AddTransient<IEmailSender, EmailSender>();

//Cau hinh Identity
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
});


//Dang ky session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true;
});

//Dang ky GG
builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientID").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});


var app = builder.Build();

//Dang ky trang 404
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");


//su dung Session
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();//Xác thực
app.UseAuthorization();//Phân quyền


app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new {controller = "Category", action = "index" }
);

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



//Seeding data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<Context>();
SeedData.SeedingData(context);

app.Run();
