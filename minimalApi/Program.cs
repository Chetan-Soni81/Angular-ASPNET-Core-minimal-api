using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todo", async (AppDbContext db) => {
    return await db.Todos.ToListAsync();
});

app.MapGet("/todo/{id}", async (int id, AppDbContext db) => {
    var todo = await db.Todos.FindAsync(id);

    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todo", async (TodoItem todoItem, AppDbContext db) => {
    db.Todos.Add(todoItem);

    await db.SaveChangesAsync();

    return Results.Created($"/todo/{todoItem.Id}", todoItem);
});

app.MapPut("/todo/{id}", async (int id, TodoItem todoItem, AppDbContext db) => {
    var item = await db.Todos.FindAsync(id);

    if(item is null) {
        return Results.NotFound();
    }

    item.Task = todoItem.Task;
    item.isCompleted = todoItem.isCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todo/{id}", async ( int id, AppDbContext db) => {
    var item = await db.Todos.FindAsync(id);

    if (item is null) {
        return Results.NotFound();
    }

    db.Todos.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
