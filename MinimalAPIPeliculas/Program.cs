using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;
using MinimalAPIPeliculas.Servicios;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//Inicio de área de servicios
builder.Services.AddCors(   opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        //Permitir Todo
        //configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

        //Permitir sólo origenes del archivo .appsetting
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();

    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();

//Guardado de imagen en azure
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

//Guardado de imagen localmente en la carpeta wwwroot
//builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(typeof(Program));

//Fin de área de los servicios

var app = builder.Build();

//if (builder.Environment.IsDevelopment())
//{

//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

//Inicio de área de los middleware
app.MapGet("/", [EnableCors(policyName:"libre")]() => "Hello World!");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios(); 

//se cambió el prefijo app de aqui para abajo por endpointGeneros para agrupar los endpoint
//app.MapGet("/", async (IRepositorioGeneros repositorio)....

//fin de área de los middleware
app.Run();

