using ErronkaApi;
using ErronkaApi.Interfaces;
using ErronkaApi.Repositorioak;
using ErronkaApi.NHibernate;
using Microsoft.Extensions.FileProviders;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()   
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<NHibernate.ISessionFactory>(_ => NHibernateHelper.SessionFactory);
builder.Services.AddTransient<IErabiltzaileaRepository, ErabiltzaileaRepository>();
builder.Services.AddTransient<IKategoriaRepository, KategoriaRepository>();
builder.Services.AddTransient<IProduktuaRepository, ProduktuaRepository>();
builder.Services.AddTransient<IEskaeraRepository, EskaeraRepository>();
builder.Services.AddTransient<IMahaiaRepository, MahaiaRepository>();
builder.Services.AddTransient<IEskaeraMahaiakRepository, EskaeraMahaiakRepository>();
builder.Services.AddTransient<IEskaeraProduktuakRepository, EskaeraProduktuakRepository>();
builder.Services.AddTransient<IZerbitzuaRepository, ZerbitzuaRepository>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "irudiak"))),
    RequestPath = "/irudiak"
});

app.UseCors(); 

app.UseAuthorization();

app.UseMiddleware<NHibernateSessionMiddleware>();
app.MapControllers();

app.Run();
