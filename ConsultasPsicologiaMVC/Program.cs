using ConsultasPsicologiaMVC.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<ConsultasPsicologiaMVC.DAO.Interfaces.IAdminDao, ConsultasPsicologiaMVC.DAO.Implementations.AdminDao>(); // Added
builder.Services.AddScoped<ConsultasPsicologiaMVC.DAO.Interfaces.IAgendamentoDao, ConsultasPsicologiaMVC.DAO.Implementations.AgendamentoDao>();
builder.Services.AddScoped<ConsultasPsicologiaMVC.DAO.Interfaces.ICadastroDao, ConsultasPsicologiaMVC.DAO.Implementations.CadastroDao>();
builder.Services.AddScoped<ConsultasPsicologiaMVC.BLL.Interfaces.ICadastroBll, ConsultasPsicologiaMVC.BLL.CadastroBLL>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redireciona para a página de login se não autenticado
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redireciona se não tiver permissão
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Tempo de expiração do cookie
        options.SlidingExpiration = true; // Renova o cookie a cada requisição
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de inatividade da sessão
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));//conexao com banco

builder.Services.AddScoped<System.Data.IDbConnection>(sp => new Npgsql.NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))); // Added

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value of 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Adicionar antes de UseAuthorization
app.UseSession(); // Adicionar antes de UseAuthorization

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
