using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<CustomerDb>(options =>
options.UseInMemoryDatabase("CustomerDb"));
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.Json", "Minimal API V1");
});

app.MapGet("/", () => "Hello Bro");
app.MapGet("/customers", async (CustomerDb db) => db.Customers.ToListAsync());
app.MapPost("/customers", async (CustomerDb db, Customer cus) =>
{
    await db.Customers.AddAsync(cus);
    await db.SaveChangesAsync();
    return Results.Created($"/customer/{cus.Id}", cus);
});
app.MapGet("/customers/{id}", async (CustomerDb db, int id) => await
    db.Customers.FindAsync(id));

app.MapPut("/customers/{id}", async (CustomerDb db, Customer newCustomer, int id) =>
{
    var current = await db.Customers.FindAsync(id);

    if (current == null) return Results.NotFound();

    current.Name = newCustomer.Name;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/customers{id}", async (CustomerDb db, int id) =>
{
    var current = await db.Customers.FindAsync(id);

    if (current == null) return Results.NotFound();

    db.Customers.Remove(current);
    await db.SaveChangesAsync();

    return Results.Ok();
});



app.Run();


class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

/* record Customer(int Id, string Name); */

class CustomerDb : DbContext
{
    public CustomerDb(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseInMemoryDatabase("CustomerDb");
    }
}