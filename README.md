# Employee Leave Management System (ELMS)

A modern, secure, and fully responsive Web Application built using **ASP.NET Core MVC** and **Entity Framework Core**. This system is designed to streamline the process of applying for, managing, and tracking employee leave requests within an organization. 

It features a dual-role architecture (Administrator and Employee) with tailored dashboards, interactive visualizations, and robust security.

## 🚀 Key Features

### 👨‍💼 Administrator Portal
* **Interactive Dashboard:** Dynamic Chart.js visualizations showing leave application trends and real-time statistics.
* **Employee Management:** Full CRUD (Create, Read, Update, Delete) operations for employee profiles, with automatic assignment of login credentials.
* **Department & Leave Type Management:** Define organization structure and categorize different types of leaves (e.g., Sick, Casual, Earned) with strict quota allocations.
* **Leave Request Handling:** Review, approve, or reject employee leave requests with a single click.

### 🧑‍💻 Employee Portal
* **Personalized Dashboard:** Track total leaves taken, pending requests, and view real-time quota balances via visual progress bars.
* **Leave Application:** A streamlined, validated form to submit new leave requests (calculates total days automatically).
* **Leave History:** A comprehensive history table detailing past requests and their approval statuses.

### 🔒 Security & Architecture
* **Authentication:** Implemented using ASP.NET Core Identity with Encrypted `HttpOnly` Cookies (blocking XSS vulnerabilities).
* **Role-Based Access Control (RBAC):** Strict isolation between Admin and Employee routes using `[Authorize(Roles="...")]` attributes.
* **CSRF Protection:** Anti-forgery tokens implemented on all POST requests to prevent cross-site request forgery.
* **Data Integrity:** Entity Framework Core handles all relational mapping, foreign keys, and cascading rules between Employees, Departments, and Leave Balances.

## 🛠️ Technology Stack
* **Backend:** C#, ASP.NET Core 9.0 MVC
* **Database:** SQL Server, Entity Framework Core (Code-First Migrations)
* **Frontend:** HTML5, Vanilla CSS, Bootstrap 5 (Layout & Modals)
* **Visualizations:** Chart.js (Interactive line graphs and trend analysis)
* **Authentication:** ASP.NET Core Identity

## ⚙️ Getting Started

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* SQL Server (LocalDB or standard instance)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/ELMS.git
   ```
2. Navigate to the project directory:
   ```bash
   cd ELMS
   ```
3. Update the database connection string in `appsettings.json` if necessary.
4. Apply Entity Framework migrations to build the database:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

*(Note: The system automatically seeds a default Administrator account and initial testing data upon the first run).*
