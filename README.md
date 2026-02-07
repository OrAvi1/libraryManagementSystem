# Library Management System (C# + MySQL)

A console-based **Library Management System** built with **C#** and **MySQL**.  
The system manages books and subscribers, supports borrowing and returning books, and persists data in a relational database.

This project was developed as part of an academic assignment and demonstrates backend logic, database integration, and basic system design.

---

## Features
- Create and initialize a MySQL database and tables automatically
- Add new books and subscribers
- Borrow and return books (maximum 3 books per subscriber)
- Track available copies of each book
- View all books or filter books by genre
- View books borrowed by a specific subscriber

---

## Data Model

### Book
Each book includes:
- Book ID
- Name
- Author
- Genre
- Available copies
- Availability status

### Subscriber
Each subscriber includes:
- Subscriber ID
- Name
- List of borrowed books (up to 3)

---

## Tech Stack
- **C#** (.NET Console Application)
- **MySQL**
- **MySql.Data** connector
- Object-Oriented Programming (OOP)
- SQL (CREATE, INSERT, SELECT, UPDATE)

---

## How to Run

### Prerequisites
- MySQL installed and running
- .NET SDK / Visual Studio

### Database Configuration
The application uses a **MySQL connection string via environment variable**  
(to avoid hardcoding credentials).

#### Windows (PowerShell)
```powershell
setx LIBRARY_DB_CONN "server=localhost;user=root;password=YOUR_PASSWORD;port=3306;database=or_library"
