using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<BookService>();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}");
app.Run();
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
}
public class BookViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
}
public class BookService
{
    private readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", Genre = "Поезія", Year = 1840 },
        new Book { Id = 2, Title = "Тіні забутих предків", Author = "Михайло Коцюбинський", Genre = "Повість", Year = 1911 }
    };

    public IEnumerable<Book> GetAll() => _books;
    public void Add(BookViewModel model)
    {
        int newId = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
        _books.Add(new Book
        {
            Id = newId,
            Title = model.Title,
            Author = model.Author,
            Genre = model.Genre,
            Year = model.Year
        });
    }
}
public class BookController : Controller
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var books = _bookService.GetAll();

        var rows = new StringBuilder();
        foreach (var b in books)
        {
            rows.Append($@"
                <tr>
                    <td>{b.Id}</td>
                    <td>{b.Title}</td>
                    <td>{b.Author}</td>
                    <td>{b.Genre}</td>
                    <td>{b.Year}</td>
                </tr>");
        }

        var html = $@"
            <!DOCTYPE html>
            <html lang='uk'>
            <head>
                <meta charset='utf-8'>
                <title>Список книг (Index)</title>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
            </head>
            <body class='bg-light py-4'>
                <div class='container' style='max-width: 800px;'>
                    <div class='d-flex justify-content-between align-items-center mb-4'>
                        <h2>Список книг</h2>
                        <a href='/Book/Add' class='btn btn-success'>Додати нову книгу</a>
                    </div>
                    <table class='table table-striped table-bordered shadow-sm bg-white'>
                        <thead class='table-dark'>
                            <tr><th>ID</th><th>Назва</th><th>Автор</th><th>Жанр</th><th>Рік</th></tr>
                        </thead>
                        <tbody>
                            {rows}
                        </tbody>
                    </table>
                </div>
            </body>
            </html>";

        return Content(html, "text/html", Encoding.UTF8);
    }

    [HttpGet]
    public IActionResult Add()
    {
        var html = @"
            <!DOCTYPE html>
            <html lang='uk'>
            <head>
                <meta charset='utf-8'>
                <title>Додати книгу (Add)</title>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
            </head>
            <body class='bg-light py-5'>
                <div class='container' style='max-width: 500px;'>
                    <div class='card shadow p-4'>
                        <h2 class='mb-3 text-center'>Додати нову книгу</h2>
                        <form action='/Book/Add' method='post'>
                            <div class='mb-3'>
                                <label class='form-label'>Назва</label>
                                <input type='text' name='Title' class='form-control' required />
                            </div>
                            <div class='mb-3'>
                                <label class='form-label'>Автор</label>
                                <input type='text' name='Author' class='form-control' required />
                            </div>
                            <div class='mb-3'>
                                <label class='form-label'>Жанр</label>
                                <input type='text' name='Genre' class='form-control' required />
                            </div>
                            <div class='mb-3'>
                                <label class='form-label'>Рік</label>
                                <input type='number' name='Year' class='form-control' required />
                            </div>
                            <button type='submit' class='btn btn-primary w-100 mb-2'>Зберегти</button>
                            <a href='/Book/Index' class='btn btn-outline-secondary w-100'>Назад до списку</a>
                        </form>
                    </div>
                </div>
            </body>
            </html>";

        return Content(html, "text/html", Encoding.UTF8);
    }

    [HttpPost]
    public IActionResult Add([FromForm] BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            _bookService.Add(model);
        }
        return RedirectToAction("Index");
    }
}
