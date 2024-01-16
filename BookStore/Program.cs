using BookStore.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

RouteGroupBuilder books = app.MapGroup("/books");

var bookList = AllBooks();

app.MapGet("/", () => "Hello World!");

books.MapGet("/", () => bookList);

books.MapGet("/{id}", (int id) =>
    bookList.Find(a => a.Id == id)
        is Book book
            ? Results.Ok(book)
            : Results.NotFound());

books.MapPost("/", (Book book) =>
{
    bookList.Add(book);
    return Results.Created($"/book/{book.Id}", book);
});

books.MapPut("/{id}", (int id, Book book) =>
{
    var selectedBook = bookList.Find(b => b.Id == id);

    if (selectedBook is null) return Results.NotFound();

    selectedBook.Title = book.Title;
    selectedBook.Description = book.Description;
    selectedBook.Author = book.Author;
    selectedBook.Price = book.Price;

    return Results.NoContent();
});

books.MapDelete("/{id}", (int id) =>
{
    if (bookList.Find(b => b.Id == id) is Book book)
    {
        bookList.Remove(book);
        return Results.NoContent();
    }
    return Results.NotFound();
});


app.Run();

List<Book> AllBooks()
{
    return [ new Book{ Id = 1, Title = "Book1", Author = "Author1", Description = "Description1", Price = 10 },
        new Book { Id = 2, Title = "Book2", Author = "Author2", Description = "Description2", Price = 40 },
            new Book { Id = 3, Title = "Book3", Author = "Author3", Description = "Description3", Price = 70 } ];
}


