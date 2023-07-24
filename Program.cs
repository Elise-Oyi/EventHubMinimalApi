using EventHub.MinimalApi.Dal;
using EventHub.MinimalApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//--adding data context in service container
builder.Services.AddDbContext<EventHubDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("conn"))
);

//--adding common repository as a service
builder.Services.AddScoped<ICommonRepository<Employee>, CommonRepository<Employee>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//---a method to get data
app.MapGet("/api/employees", async (ICommonRepository<Employee> _repository)=>
{
    var employees = await _repository.GetAll();
    if(employees == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(employees);
}).WithName("GetAll")
   .Produces<IEnumerable<Employee>>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status404NotFound);


//--a method to get 1 data using Id
app.MapGet("/api/employees/{id}",async (int id, ICommonRepository<Employee> _repository) =>
{
    var employee = await _repository.GetDetails(id);
    return employee == null? Results.NotFound(): Results.Ok(employee);
}).WithName("GetDetails")
   .Produces<IEnumerable<Employee>>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status404NotFound);


//--a method to post data
app.MapPost("/api/employees", async (Employee employee, ICommonRepository<Employee> _repository) =>
{
    var output = await _repository.Insert(employee);
    if(output != null)
    {
        return Results.Created($"/api/employee/{output.EmployeeId}", output);
    }
    return Results.BadRequest();
}).WithName("Create")
   .Produces<IEnumerable<Employee>>(StatusCodes.Status201Created)
   .Produces(StatusCodes.Status400BadRequest);


//--a method to update data
app.MapPut("/api/employees", async (Employee employee, ICommonRepository<Employee> _repository) =>
{
    var output = await _repository.Update(employee);
     if (output != null)
    {
        return Results.NoContent();
    }
     return Results.BadRequest();
}).WithName("Update")
   .Produces<IEnumerable<Employee>>(StatusCodes.Status204NoContent)
     .Produces(StatusCodes.Status400BadRequest);


//--a method to delete data
app.MapDelete("/api/employees", async (int id, ICommonRepository<Employee> _repository) =>
{
    var output = _repository.Delete(id);
    if(output != null)
    {
        return Results.NoContent();
    }
    return Results.NotFound();
}).WithName("Delete")
   .Produces<IEnumerable<Employee>>(StatusCodes.Status204NoContent)
     .Produces(StatusCodes.Status400BadRequest);




app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}