using Microsoft.EntityFrameworkCore;
using Module16_WebApplication.Configuration;
using Module16_WebApplication.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure NorthwindSettings
builder.Services.Configure<NorthwindSettings>(
    builder.Configuration.GetSection(NorthwindSettings.SectionName));

// Add DbContext with In-Memory database
// Note: Connection string is configured but currently using In-Memory for simplicity
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseInMemoryDatabase("NorthwindDb"));

var app = builder.Build();

// Initialize the database with sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NorthwindContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
