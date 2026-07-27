namespace LibraryManager;

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

    public bool CheckIfOverdue() {
        if (IsBorrowed == false) {
            return false;
        }
        if (BorrowedAt == null) {
            return false;
        }

        double daysBorrowed = (DateTime.Now - BorrowedAt.Value).TotalDays;
        if (daysBorrowed > 14) {
            return true;
        }
        return false;
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

    public bool ExtendBorrow() {
        if (IsBorrowed == false) {
            return false;
        }
        if (HasExtended == true) {
            return false;
        }
        if (BorrowedAt.HasValue == false) {
            return false;
        }

        DateTime currentDate = BorrowedAt.Value;
        BorrowedAt = currentDate.AddDays(7);
        HasExtended = true;
        return true;
    }
}
