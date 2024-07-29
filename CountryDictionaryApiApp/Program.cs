using CountryDictionaryApiApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();

app.MapGet("/", () => new{ Message = "server is running" });
app.MapGet("/ping", () => new { Message = "pong" });
app.MapGet("/country", async (ApplicationDbContext db) => await db.Countries.ToListAsync());

app.MapPost("/country", async (Country country, ApplicationDbContext db) =>
{
    await db.Countries.AddAsync(country);
    await db.SaveChangesAsync();
    return country;
});

app.MapGet("/country/{id:int}", async (int id, ApplicationDbContext db) =>
{
    return await db.Countries.FirstOrDefaultAsync(c => c.Id == id);
});

app.MapGet("/country/alpha2/{alpha2:alpha}", async (string alpha2, ApplicationDbContext db) =>
{
    return await db.Countries.FirstOrDefaultAsync(c => c.ISO31661Alpha2Code == alpha2);
});

app.MapGet("/country/alpha3/{alpha3:alpha}", async (string alpha3, ApplicationDbContext db) =>
{
    return await db.Countries.FirstOrDefaultAsync(c => c.ISO31661Alpha3Code == alpha3);
});

app.MapGet("/country/numeric/{num:int}", async (string num, ApplicationDbContext db) =>
{
    return await db.Countries.FirstOrDefaultAsync(c => c.ISO31661NumericCode == num);
});

app.MapPatch("/country/{id:int}", async (int id, Country country, ApplicationDbContext db) =>
{
    Country? result = await db.Countries.FirstOrDefaultAsync(c => c.Id == id);
    if(result != null)
    {
        result.Name = country.Name;
        result.ISO31661Alpha2Code = country.ISO31661Alpha2Code;
        result.ISO31661Alpha3Code = country.ISO31661Alpha3Code;
        result.ISO31661NumericCode = country.ISO31661NumericCode;
        await db.SaveChangesAsync();
    }
    return result;
});


app.MapGet("/country/search", Country? (HttpContext context, ApplicationDbContext db) =>
{
    KeyValuePair<string, StringValues>? nullableParam = context.Request.Query.FirstOrDefault();
    if (nullableParam == null)
    {
        return null;
    }
    KeyValuePair<string, StringValues> param = nullableParam.Value;
    if (param.Key == "id")
    {
        int id = Convert.ToInt32(param.Value[0]);
        return db.Countries.FirstOrDefault(c => c.Id == id);
    }
    if (param.Key == "name")
    {
        string name = param.Value[0] ?? "";
        return db.Countries.FirstOrDefault(c => c.Name.Trim().ToLower() == name.Trim().ToLower());
    }
    if (param.Key == "code")
    {
        string code = param.Value[0] ?? "";
        return db.Countries.FirstOrDefault(c => 
            c.ISO31661Alpha2Code.Trim().ToLower() == code.Trim().ToLower() ||
            c.ISO31661Alpha3Code.Trim().ToLower() == code.Trim().ToLower() ||
            c.ISO31661NumericCode.Trim().ToLower() == code.Trim().ToLower()
        );
    }
    return null;
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();
