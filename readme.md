# Library Manager

.NET console app for managing a library's books (add, view, borrow, return), with business rules layered on top. For each requirement below, the exact code that implements it, followed by a short note on why.

---

## 1. Create a .NET Console Project

> Name it LibraryManager.

Created with `dotnet new console -n LibraryManager`, which generated `LibraryManager.csproj` and `Program.cs`.

**`LibraryManager.csproj`:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

Uses .NET 10, the latest SDK installed locally. `Nullable enable` so the compiler flags possible-null values (e.g. `Console.ReadLine()` results).

---

## 2. The `Book` class

> Properties: Title, Author, IsBorrowed, BorrowedAt. Methods: Borrow(), Return().

**`Book.cs`:**

```csharp
public class Book
{
    public string Title;
    public string Author;
    public bool IsBorrowed;
    public DateTime? BorrowedAt;
    public bool HasExtended;

    public Book(string title, string author) {
        Title = title;
        Author = author;
        IsBorrowed = false;
        BorrowedAt = null;
        HasExtended = false;
    }

    public void Borrow() {
        IsBorrowed = true;
        BorrowedAt = DateTime.Now;
        HasExtended = false;
    }

    public void Return() {
        IsBorrowed = false;
        BorrowedAt = null;
    }
}
```

A `Book` only knows how to manage its own state — it has no idea a `Library` exists. `BorrowedAt` is `DateTime?` (nullable) because a book that's never been borrowed has no borrow date. `Borrow()` always overwrites `BorrowedAt` with `DateTime.Now`, so a re-borrowed book automatically resets its previous date.

---

## 3. The `Library` class

> Holds a `List<Book>` Books. Methods: AddBook, BorrowBook, ReturnBook, ListOverdueBooks.

**`Library.cs`:**

```csharp
public class Library
{
    public List<Book> Books = new List<Book>();
    public int CurrentBorrowCount = 0;

    public void AddBook(Book book) { ... }
    public void BorrowBook(int index) { ... }
    public void ReturnBook(int index) { ... }
    public void ListOverdueBooks() { ... }
}
```

All the business rules (duplicates, borrow limit, overdue checks) live here, not in `Program.cs` and not in `Book`. `Book` stores state; `Library` decides what's *allowed* to happen to that state.

---

## 4. Console Menu

> Show a menu, let the user pick an option by number.

**`Program.cs`:**

```csharp
while (running){
    Console.Clear();
    Console.WriteLine("Library Manager");
    Console.WriteLine("---------------");
    Console.WriteLine("1. Add a new book");
    Console.WriteLine("2. View all books");
    Console.WriteLine("3. Borrow a book");
    Console.WriteLine("4. Return a book");
    Console.WriteLine("5. View overdue books");
    Console.WriteLine("6. View filtered books");
    Console.WriteLine("7. Extend a borrowed book");
    Console.WriteLine("8. View stats");
    Console.WriteLine("9. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    if (choice == "1") { AddBookMenu(); }
    else if (choice == "2") { ViewBooksMenu(); }
    else if (choice == "3") { BorrowBookMenu(); }
    ...
    else if (choice == "9") { running = false; Console.WriteLine("Goodbye!"); }
    else { Console.WriteLine("Invalid option, try again."); }

    if (running){
        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
}
```

`bool running` drives the loop, flips to `false` only on Exit. `if/else if` instead of `switch` since `choice` is a nullable string. Each menu option calls its own local function (`AddBookMenu`, `ViewBooksMenu`, ...) which just reads input and calls into `library` — the menu doesn't contain business logic itself. `Console.Clear()` keeps the menu tidy; `Console.ReadKey()` pauses after each action so results aren't wiped before being read.

---

## 5. Business Rules / Logic

### No duplicate books

```csharp
public void AddBook(Book book) {
    for (int i = 0; i < Books.Count; i++) {
        Book existingBook = Books[i];
        bool sameTitle = existingBook.Title.ToLower() == book.Title.ToLower();
        bool sameAuthor = existingBook.Author.ToLower() == book.Author.ToLower();

        if (sameTitle && sameAuthor) {
            Console.WriteLine("This book already exists in the library.");
            return;
        }
    }

    Books.Add(book);
    Console.WriteLine("Added \"" + book.Title + "\" by " + book.Author + ".");
}
```

Loops through existing books comparing title + author, case-insensitively (`.ToLower()`), before allowing the add.

### Borrow limit (max 3 books)

```csharp
public void BorrowBook(int index) {
    ...
    if (CurrentBorrowCount >= MaxBorrowedBooks) {
        Console.WriteLine("You can't borrow more than " + MaxBorrowedBooks + " books at a time.");
        return;
    }

    book.Borrow();
    CurrentBorrowCount = CurrentBorrowCount + 1;
    ...
}
```

`CurrentBorrowCount` is a simple counter on `Library`, incremented on borrow and decremented on return. `MaxBorrowedBooks` is a `const` so the limit is defined once and can't drift.

### Overdue after 14 days

```csharp
public bool CheckIfOverdue() {
    if (IsBorrowed == false) { return false; }
    if (BorrowedAt == null) { return false; }

    double daysBorrowed = (DateTime.Now - BorrowedAt.Value).TotalDays;
    if (daysBorrowed > 14) { return true; }
    return false;
}
```

Lives on `Book` itself, since "is this book overdue" only needs the book's own data. Compares `DateTime.Now` against `BorrowedAt` and checks if more than 14 days have passed.

### Message on overdue return

```csharp
public void ReturnBook(int index) {
    ...
    if (book.CheckIfOverdue()) {
        Console.WriteLine("This book is overdue! Please return books on time.");
    }

    book.Return();
    ...
}
```

The overdue check happens *before* `book.Return()` clears `BorrowedAt` — otherwise there'd be no date left to check against.

---

## 6. Output Formatting

> [0] Clean Code by Robert C. Martin - Borrowed on 2025-03-27 / [1] Atomic Habits by James Clear - Available

```csharp
private string FormatBook(int index, Book book) {
    if (book.IsBorrowed == false || book.BorrowedAt.HasValue == false) {
        return "[" + index + "] " + book.Title + " by " + book.Author + " - Available";
    }

    DateTime borrowedDate = book.BorrowedAt.Value;
    string dateText = borrowedDate.ToString("yyyy-MM-dd");
    string status = "[" + index + "] " + book.Title + " by " + book.Author + " - Borrowed on " + dateText;

    if (book.CheckIfOverdue()) {
        status = status + " (OVERDUE)";
    }

    return status;
}
```

One shared formatter used by `ListBooks`, `ListOverdueBooks`, and stats, so every list looks the same everywhere. Appends `(OVERDUE)` when relevant instead of duplicating the overdue text elsewhere.

---

## Bonus

### Filter books (all / available / borrowed)

```csharp
public void ListBooks(int filter) {
    for (int i = 0; i < Books.Count; i++) {
        Book book = Books[i];
        if (filter == 1 && book.IsBorrowed) { continue; }
        if (filter == 2 && book.IsBorrowed == false) { continue; }
        Console.WriteLine(FormatBook(i, book));
    }
}
```

An `int` flag instead of an enum, to keep things simple. `0` also covers plain "view all books" so one method serves both requirement 4 and the bonus.

### Library stats

```csharp
public void PrintStats() {
    int total = Books.Count;
    int borrowedCount = 0;
    int overdueCount = 0;

    for (int i = 0; i < Books.Count; i++) {
        Book book = Books[i];
        if (book.IsBorrowed) { borrowedCount = borrowedCount + 1; }
        if (book.CheckIfOverdue()) { overdueCount = overdueCount + 1; }
    }

    Console.WriteLine("Total books: " + total);
    Console.WriteLine("Borrowed: " + borrowedCount);
    Console.WriteLine("Overdue: " + overdueCount);
}
```

Counts totals in one pass over `Books` rather than three separate loops.

### Extend borrow period once

```csharp
public bool ExtendBorrow() {
    if (IsBorrowed == false) { return false; }
    if (HasExtended == true) { return false; }
    if (BorrowedAt.HasValue == false) { return false; }

    DateTime currentDate = BorrowedAt.Value;
    BorrowedAt = currentDate.AddDays(7);
    HasExtended = true;
    return true;
}
```

`HasExtended` is the guard that makes this a one-time action per borrow; it gets reset back to `false` inside `Borrow()`, so the book earns a fresh extension each time it's borrowed again.

---

## OOP principles used

This project leans on core OOP ideas to keep the code organized and easy to reason about:

### Encapsulation

Each `Book` manages its own state through its own methods (`Borrow()`, `Return()`, `ExtendBorrow()`, `CheckIfOverdue()`) instead of letting outside code poke at `IsBorrowed`/`BorrowedAt` directly. For example, `Library.BorrowBook()` never writes `book.IsBorrowed = true` itself — it calls `book.Borrow()` and trusts the `Book` to update itself consistently (setting both `IsBorrowed` and `BorrowedAt` together, and resetting `HasExtended`). This means there's only one place that can put a `Book` into an inconsistent state (like `IsBorrowed = true` but `BorrowedAt = null`), and that place is the `Book` class itself.

### Abstraction

`Program.cs` never touches `Books[index].IsBorrowed` or does date math — it just calls `library.BorrowBook(3)` or `library.ReturnBook(1)`. The menu code doesn't need to know *how* borrowing works (checking the limit, updating the counter, calling `Borrow()`); it only needs to know *that* it works. All of that detail is hidden behind `Library`'s public methods.

### Single Responsibility (separation of concerns)

Three classes, three jobs:

- **`Book`** — represents one book and its own borrow state.
- **`Library`** — owns the collection of books and enforces business rules (duplicates, borrow limit, overdue logic, formatting).
- **`Program.cs`** — only handles console input/output and wiring menu choices to `Library` calls.

This is why, for example, `CheckIfOverdue()` exists on `Book` (it only needs that one book's data) while `AddBook()` exists on `Library` (it needs to compare a new book against *every other* book in the collection).

### Composition (objects made of other objects)

`Library` doesn't inherit from `Book` — it *has* a `List<Book>`. This is composition ("a Library has Books") rather than inheritance ("a Library is a Book"), which fits the real relationship between the two: a library manages many books, it isn't a special kind of book.

**Why no inheritance here:** there's only one kind of `Book` in this project (no `EBook`/`AudioBook` subclasses with different behavior), so introducing a class hierarchy would add structure without solving any real problem — one of the reasons this design sticks to plain classes and composition instead.

---

## CI/CD Pipeline

> Build the project and publish a downloadable release automatically, only when a version tag is pushed.

**`.github/workflows/ci.yml`:**

```yaml
name: CI/CD

on:
  push:
    tags: [ "v*" ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "10.0.x"
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release

  release:
    needs: build
    runs-on: ubuntu-latest
    if: startsWith(github.ref, 'refs/tags/v')
    permissions:
      contents: write
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "10.0.x"
      - run: dotnet publish LibraryManager.csproj --configuration Release --output ./publish-win-x64 -r win-x64 --self-contained true -p:PublishSingleFile=true
      - run: dotnet publish LibraryManager.csproj --configuration Release --output ./publish-linux-x64 -r linux-x64 --self-contained true -p:PublishSingleFile=true
      - run: |
          cp publish-win-x64/LibraryManager.exe LibraryManager-${{ github.ref_name }}-win-x64.exe
          cp publish-linux-x64/LibraryManager LibraryManager-${{ github.ref_name }}-linux-x64
      - uses: softprops/action-gh-release@v2
        with:
          files: |
            LibraryManager-${{ github.ref_name }}-win-x64.exe
            LibraryManager-${{ github.ref_name }}-linux-x64
```