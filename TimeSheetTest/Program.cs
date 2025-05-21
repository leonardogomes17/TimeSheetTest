using TimeSheetTest.Data;
using TimeSheetTest.Data.Interfaces;
using TimeSheetTest.Service;
using TimeSheetTest.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddScoped(typeof(IRepository<Product>), typeof(RepositoryProduct));
builder.Services.AddSingleton(typeof(DatabaseConnection));
builder.Services.AddSingleton(typeof(ITimeSheetRepository), typeof(TimeSheetRepository));
builder.Services.AddScoped(typeof(ITimeSheetService), typeof(TimeSheetService));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
