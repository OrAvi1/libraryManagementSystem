using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace tryTask4
{
    class Book
    {
        public int BookID { get; private set; }
        public string BookName { get; private set; }
        public string BookWriter { get; private set; }
        public string Genre { get; private set; }
        public int AvailableCopies { get; set; }
        public bool IsAvailable { get; private set; }

        public Book(int bookID, string bookName, string bookWriter, string genre, int availableCopies, bool isAvailable)
        {
            BookID = bookID;
            BookName = bookName;
            BookWriter = bookWriter;
            Genre = genre;
            AvailableCopies = availableCopies;
            IsAvailable = isAvailable;
        }

        public bool BorrowBook()
        {
            if (AvailableCopies > 0 && IsAvailable)
            {
                AvailableCopies--;
                return true;
            }
            return false;
        }

        public void ReturnBook()
        {
            AvailableCopies++;
        }

        public string BookInfo()
        {
            return $"Book ID: {BookID}, Name: {BookName}, Writer: {BookWriter}, Genre: {Genre}, Available Copies: {AvailableCopies}, Available for Loan: {IsAvailable}";
        }
    }

    class Subscriber
    {
        public string SubName { get; private set; }
        public int SubID { get; private set; }
        public List<int> BorrowedBooks { get; private set; }

        public Subscriber(int subID, string subName)
        {
            SubID = subID;
            SubName = subName;
            BorrowedBooks = new List<int>();
        }

        public bool CanBorrow()
        {
            return BorrowedBooks.Count < 3;
        }

        public void BorrowBook(int bookID)
        {
            if (CanBorrow())
            {
                BorrowedBooks.Add(bookID);
            }
        }

        public void ReturnBook(int bookID)
        {
            BorrowedBooks.Remove(bookID);
        }

        public string SubscriberInfo()
        {
            return $"Subscriber ID: {SubID}, Name: {SubName}, Borrowed Books: {BorrowedBooks.Count}";
        }
    }

    class Library
    {
        private MySqlConnection conn;
        private Dictionary<int, Book> books;
        private Dictionary<int, Subscriber> subscribers;

        public Library(string connStr)
        {
            conn = new MySqlConnection(connStr);
            books = new Dictionary<int, Book>();
            subscribers = new Dictionary<int, Subscriber>();

            CreateDatabase();
            CreateTables();
            LoadBooks();
            LoadSubscribers();
        }

        private void CreateDatabase()
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS or_library";
                cmd.ExecuteNonQuery();
                conn.ChangeDatabase("or_library");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database: {ex.Message}");
            }
        }

        private void CreateTables()
        {
            try
            {
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Books (
                    BookID INT PRIMARY KEY AUTO_INCREMENT,
                    BookName VARCHAR(100),
                    BookWriter VARCHAR(100),
                    Genre VARCHAR(50),
                    AvailableCopies INT,
                    IsAvailable BOOLEAN
                )";
                cmd.ExecuteNonQuery();


                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Subscribers (
                    SubID INT PRIMARY KEY AUTO_INCREMENT,
                    SubName VARCHAR(100)
                )";
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }
        }

        private void LoadBooks()
        {
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Books";
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int bookID = reader.GetInt32("BookID");
                    string bookName = reader.GetString("BookName");
                    string bookWriter = reader.GetString("BookWriter");
                    string genre = reader.GetString("Genre");
                    int availableCopies = reader.GetInt32("AvailableCopies");
                    bool isAvailable = reader.GetBoolean("IsAvailable");

                    books.Add(bookID, new Book(bookID, bookName, bookWriter, genre, availableCopies, isAvailable));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        private void LoadSubscribers()
        {
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Subscribers";
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int subID = reader.GetInt32("SubID");
                    string subName = reader.GetString("SubName");

                    subscribers.Add(subID, new Subscriber(subID, subName));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading subscribers: {ex.Message}");
            }
        }

        public void AddBook()
        {
            Console.WriteLine("Enter Book ID (up to 5 digits):");
            int bookID = ValidInput(6);

            if (books.ContainsKey(bookID))
            {
                Console.WriteLine("A book with this ID already exists.");
                return;
            }

            Console.WriteLine("Enter Book Name:");
            string name = Console.ReadLine();
            
            Console.WriteLine("Enter Author:");
            string author = Console.ReadLine();
            
            Console.WriteLine("Enter Genre:");
            string genre = Console.ReadLine();

            Console.WriteLine("Enter Available Copies:");
            int copies = ValidInput();

            Console.WriteLine("Is Available for Loan? (y/n):");
            bool isAvailable = Console.ReadLine().ToLower() == "y";

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Books (BookID, BookName, BookWriter, Genre, AvailableCopies, IsAvailable) " +
                                  "VALUES (@bookID, @name, @author, @genre, @copies, @isAvailable)";

                cmd.Parameters.AddWithValue("@bookID", bookID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@author", author);
                cmd.Parameters.AddWithValue("@genre", genre);
                cmd.Parameters.AddWithValue("@copies", copies);
                cmd.Parameters.AddWithValue("@isAvailable", isAvailable);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Book added successfully.");
                books.Add(bookID, new Book(bookID, name, author, genre, copies, isAvailable));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
            }
        }

        public void AddSubscriber()
        {
            Console.WriteLine("Enter Subscriber ID (up to 5 digits):");
            int SubID = ValidInput(6);

            if(subscribers.ContainsKey(SubID) )
            {
                Console.WriteLine("Subscriber with this ID already exists.");
                return;
            }

            Console.WriteLine("Enter Subscriber Name:");
            string name = Console.ReadLine();

            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"INSERT INTO Subscribers (SubID, SubName) VALUES ({SubID}, '{name}')";

                cmd.ExecuteNonQuery();

                Subscriber newSubscriber = new Subscriber(SubID, name);
                subscribers.Add(SubID, newSubscriber);

                Console.WriteLine("Subscriber added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding subscriber: {ex.Message}");
            }
        }

        public void BorrowBook()
        {
            try
            {
                Console.WriteLine("Enter Subscriber ID:");
                int subID = ValidInput(6); 

                if (!subscribers.ContainsKey(subID))
                {
                    Console.WriteLine("Subscriber not found.");
                    return;
                }

                Subscriber subscriber = subscribers[subID];

                if (!subscriber.CanBorrow())
                {
                    Console.WriteLine("Subscriber cannot borrow more books.");
                    return;
                }

                Console.WriteLine("Enter Book ID to Borrow (or enter book name to search by name):");
                string input = Console.ReadLine();

                Book borrowBook = null;

                if (int.TryParse(input, out int bookID))
                {
                    if (books.ContainsKey(bookID))
                    {
                        borrowBook = books[bookID];
                    }
                    else
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Searching books by name...");
                    bool bookFound = false;

                    foreach (var book in books.Values)
                    {
                        string bookName = book.BookName.ToLower();
                        string temp = input.ToLower();

                        if (bookName.Contains(temp)) 
                        {
                            Console.WriteLine(book.BookInfo());
                            bookFound = true;
                        }
                    }

                    if (!bookFound)
                    {
                        Console.WriteLine("This book is not found.");
                        return;
                    }

                    Console.WriteLine("Enter the Book ID of the selected book:");
                    bookID = ValidInput(6); 

                    if (books.ContainsKey(bookID))
                    {
                        borrowBook = books[bookID];
                    }
                    else
                    {
                        Console.WriteLine("Invalid Book ID.");
                        return;
                    }
                }

                if (borrowBook != null && borrowBook.BorrowBook())
                {
                    subscriber.BorrowBook(borrowBook.BookID);
                    Console.WriteLine("Book borrowed successfully.");
                }
                else
                {
                    Console.WriteLine("No available copies of the book.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ReturnBook()
        {
            Console.WriteLine("Enter Subscriber ID:");
            int subID = int.Parse(Console.ReadLine());

            if (!subscribers.ContainsKey(subID))
            {
                Console.WriteLine("Subscriber not found.");
                return;
            }

            Console.WriteLine("Enter Book ID to Return:");
            int bookID = int.Parse(Console.ReadLine());

            if (!books.ContainsKey(bookID))
            {
                Console.WriteLine("Book not found.");
                return;
            }

            Book book = books[bookID];
            Subscriber subscriber = subscribers[subID];

            if (subscriber.BorrowedBooks.Contains(bookID))
            {
                subscriber.ReturnBook(bookID);
                book.ReturnBook();
                Console.WriteLine("Book returned successfully.");
            }
            else
            {
                Console.WriteLine("This subscriber did not borrow this book.");
            }
        }

        public void ShowBooks()
        {
            foreach (var book in books.Values)
            {
                Console.WriteLine(book.BookInfo());
            }
        }

        public void ShowBooksByGenre()
        {
            Console.WriteLine("Enter genre:");
            string genre = Console.ReadLine();

            foreach (var book in books.Values)
            {
                if (book.Genre.ToLower() == genre.ToLower())
                {
                    Console.WriteLine(book.BookInfo());
                }
            }
        }

        public void ShowSubscriberBooks()
        {
            Console.WriteLine("Enter Subscriber ID:");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int subID))
            {
                if (subscribers.ContainsKey(subID))
                {
                    Subscriber subscriber = subscribers[subID];

                    Console.WriteLine("Books borrowed by this subscriber:");
                    foreach (var bookID in subscriber.BorrowedBooks)
                    {
                        Console.WriteLine(books[bookID].BookInfo());
                    }
                }
                else
                {
                    Console.WriteLine("Subscriber not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid Subscriber ID.");
            }
        }


        public int ValidInput(int len = 0)
        {
            int temp;
            string input = Console.ReadLine();

            while (!int.TryParse(input, out temp) || temp <= 0 || input.Length < 1 || input.Length > 5)
            {
                Console.WriteLine("Invalid input. Please enter a valid integer with between 1 and 5 digits.");
                input = Console.ReadLine();
            }

            return temp;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            string connStr = Environment.GetEnvironmentVariable("LIBRARY_DB_CONN");

            if (string.IsNullOrWhiteSpace(connStr))
            {
                Console.WriteLine("Missing env var LIBRARY_DB_CONN.");
                Console.WriteLine(@"Example (Windows PowerShell):
                    setx LIBRARY_DB_CONN ""server=localhost;user=root;password=YOURPASS;port=3306;database=or_library""");
                Console.WriteLine(@"Example (macOS/Linux):
                    export LIBRARY_DB_CONN='server=localhost;user=root;password=YOURPASS;port=3306;database=or_library'");
                return;
            }

            Library library = new Library(connStr);

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1 - Add a new book");
                Console.WriteLine("2 - Add a new subscriber");
                Console.WriteLine("3 - Borrow a book");
                Console.WriteLine("4 - Return a book");
                Console.WriteLine("5 - Show all books");
                Console.WriteLine("6 - Show books by genre");
                Console.WriteLine("7 - Show subscriber's borrowed books");
                Console.WriteLine("8 - Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        library.AddBook();
                        break;
                    case 2:
                        library.AddSubscriber();
                        break;
                    case 3:
                        library.BorrowBook();
                        break;
                    case 4:
                        library.ReturnBook();
                        break;
                    case 5:
                        library.ShowBooks();
                        break;
                    case 6:
                        library.ShowBooksByGenre();
                        break;
                    case 7:
                        library.ShowSubscriberBooks();
                        break;
                    case 8:
                        Console.WriteLine("Goodbye!");
                        return;
                }
                Console.WriteLine("-----------------------------------");
            }
        }
    }
}
