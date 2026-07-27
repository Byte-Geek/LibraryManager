using LibraryManager;

Library library = new Library();
library.AddBook(new Book("Clean Code", "Robert C. Martin"));
library.AddBook(new Book("Atomic Habits", "James Clear"));

bool running = true;

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

    if (choice == "1"){
        AddBookMenu();
    }
    else if (choice == "2"){
        ViewBooksMenu();
    }
    else if (choice == "3"){
        BorrowBookMenu();
    }
    else if (choice == "4"){
        ReturnBookMenu();
    }
    else if (choice == "5"){
        OverdueBooksMenu();
    }
    else if (choice == "6"){
        FilteredBooksMenu();
    }
    else if (choice == "7"){
        ExtendBookMenu();
    }
    else if (choice == "8"){
        StatsMenu();
    }
    else if (choice == "9"){
        running = false;
        Console.WriteLine("Goodbye!");
    }
    else{
        Console.WriteLine("Invalid option, try again.");
    }

    if (running){
        Console.WriteLine();
        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
}

void AddBookMenu(){
    Console.Write("Title: ");
    string title = Console.ReadLine() ?? "";
    Console.Write("Author: ");
    string author = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author)){
        Console.WriteLine("Title and author cannot be empty.");
        return;
    }

    library.AddBook(new Book(title.Trim(), author.Trim()));
}

void ViewBooksMenu(){
    library.ListBooks(0);
}

void BorrowBookMenu(){
    library.ListBooks(0);
    Console.Write("Book number to borrow: ");
    string? input = Console.ReadLine();
    int index;
    bool isNumber = int.TryParse(input, out index);

    if (!isNumber){
        Console.WriteLine("Invalid input.");
        return;
    }

    library.BorrowBook(index);
}

void ReturnBookMenu(){
    library.ListBooks(0);
    Console.Write("Book number to return: ");
    string? input = Console.ReadLine();
    int index;
    bool isNumber = int.TryParse(input, out index);

    if (!isNumber){
        Console.WriteLine("Invalid input.");
        return;
    }

    library.ReturnBook(index);
}

void OverdueBooksMenu(){
    library.ListOverdueBooks();
}

void FilteredBooksMenu(){
    Console.Write("Filter (1 = Available, 2 = Borrowed, 3 = All): ");
    string? filterChoice = Console.ReadLine();
    int filter;

    if (filterChoice == "1"){
        filter = 1;
    }
    else if (filterChoice == "2"){
        filter = 2;
    }
    else{
        filter = 0;
    }

    library.ListBooks(filter);
}

void ExtendBookMenu(){
    library.ListBooks(0);
    Console.Write("Book number to extend: ");
    string? input = Console.ReadLine();
    int index;
    bool isNumber = int.TryParse(input, out index);

    if (!isNumber){
        Console.WriteLine("Invalid input.");
        return;
    }

    library.ExtendBook(index);
}

void StatsMenu(){
    library.PrintStats();
}
