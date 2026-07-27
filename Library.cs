namespace LibraryManager;

public class Library
{
    private const int MaxBorrowedBooks = 3;

    public List<Book> Books = new List<Book>();
    public int CurrentBorrowCount = 0;

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

    public void BorrowBook(int index) {
        if (IsValidIndex(index) == false) {
            Console.WriteLine("Invalid book number.");
            return;
        }

        Book book = Books[index];

        if (book.IsBorrowed) {
            Console.WriteLine("This book is already borrowed.");
            return;
        }

        if (CurrentBorrowCount >= MaxBorrowedBooks) {
            Console.WriteLine("You can't borrow more than " + MaxBorrowedBooks + " books at a time.");
            return;
        }

        book.Borrow();
        CurrentBorrowCount = CurrentBorrowCount + 1;
        Console.WriteLine("You borrowed \"" + book.Title + "\".");
    }

    public void ReturnBook(int index) {
        if (IsValidIndex(index) == false) {
            Console.WriteLine("Invalid book number.");
            return;
        }

        Book book = Books[index];

        if (book.IsBorrowed == false) {
            Console.WriteLine("This book isn't currently borrowed.");
            return;
        }

        if (book.CheckIfOverdue()) {
            Console.WriteLine("This book is overdue! Please return books on time.");
        }

        book.Return();
        CurrentBorrowCount = CurrentBorrowCount - 1;
        Console.WriteLine("You returned \"" + book.Title + "\".");
    }

    public void ExtendBook(int index) {
        if (IsValidIndex(index) == false) {
            Console.WriteLine("Invalid book number.");
            return;
        }

        Book book = Books[index];

        if (book.IsBorrowed == false) {
            Console.WriteLine("This book isn't currently borrowed.");
            return;
        }

        bool success = book.ExtendBorrow();
        if (success) {
            Console.WriteLine("Due date for \"" + book.Title + "\" extended by 7 days.");
        } else {
            Console.WriteLine("This book's borrow period has already been extended once.");
        }
    }

   
    public void ListBooks(int filter) {
        int shownCount = 0;

        for (int i = 0; i < Books.Count; i++) {
            Book book = Books[i];

            if (filter == 1 && book.IsBorrowed) {
                continue;
            }
            if (filter == 2 && book.IsBorrowed == false) {
                continue;
            }

            Console.WriteLine(FormatBook(i, book));
            shownCount = shownCount + 1;
        }

        if (shownCount == 0) {
            Console.WriteLine("No books to show.");
        }
    }

    public void ListOverdueBooks() {
        int shownCount = 0;

        for (int i = 0; i < Books.Count; i++) {
            Book book = Books[i];

            if (book.CheckIfOverdue()) {
                Console.WriteLine(FormatBook(i, book));
                shownCount = shownCount + 1;
            }
        }

        if (shownCount == 0) {
            Console.WriteLine("No overdue books.");
        }
    }

    public void PrintStats() {
        int total = Books.Count;
        int borrowedCount = 0;
        int overdueCount = 0;

        for (int i = 0; i < Books.Count; i++) {
            Book book = Books[i];

            if (book.IsBorrowed) {
                borrowedCount = borrowedCount + 1;
            }
            if (book.CheckIfOverdue()) {
                overdueCount = overdueCount + 1;
            }
        }

        Console.WriteLine("Total books: " + total);
        Console.WriteLine("Borrowed: " + borrowedCount);
        Console.WriteLine("Overdue: " + overdueCount);
    }

    private bool IsValidIndex(int index) {
        if (index < 0) {
            return false;
        }
        if (index >= Books.Count) {
            return false;
        }
        return true;
    }

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
}
