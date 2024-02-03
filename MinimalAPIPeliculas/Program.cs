var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration
    .GetValue<string>("origenesPermitidos")!;

// Inicio �rea de servicios *****************************************

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(config =>
        config.WithOrigins(origenesPermitidos)
        .AllowAnyHeader()
        .AllowAnyMethod());

    options.AddPolicy("libre", config =>
        config.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddOutputCache();

// Fin �rea de servicios ********************************************

var app = builder.Build();

// Inicio �rea de middlewares

/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

// se coloca antes normalmente de primero para evitar CORS
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseOutputCache();

app.MapGet("/", () => "Hello World!")
    .RequireCors("libre");
app.MapGet("/generos", () =>
{
    var generos = new List<Genero> {
        new Genero { Id = 1, Nombre = "Drama" },
        new Genero { Id = 2, Nombre = "Acci�n" },
        new Genero { Id = 3, Nombre = "Comedia" },
    };

    return generos;
})
    .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)));

// Fin �rea de middlewares

app.Run();
