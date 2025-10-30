# Task Management Application

A professional WPF desktop application for managing tasks with full CRUD operations, filtering, searching, and data export capabilities.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![WPF](https://img.shields.io/badge/WPF-Windows-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

### Core Functionality
- **CRUD Operations** - Create, Read, Update, Delete tasks
- **Task Properties** - Title, Description, Status, Priority, Due Date, Estimated Hours, Tags
- **Person Assignment** - Assign tasks to people with inline person creation
- **Advanced Filtering** - Filter by Status and Priority with auto-refresh
- **Search** - Full-text search in task titles and descriptions
- **Data Export** - Export task list to CSV or Excel format
- **Data Validation** - FluentValidation with business rules enforcement
- **Sample Data** - Auto-seeded with 10 example tasks on first run

### Technical Features
- **Clean Architecture** - Separated layers (Core, Infrastructure, UI)
- **MVVM Pattern** - Full implementation with Commands and Data Binding
- **Entity Framework Core** - SQLite database with Code-First approach
- **Dependency Injection** - Using Microsoft.Extensions.DependencyInjection
- **Unit Tests** - 43 tests covering validation, services, and entities
- **Result Pattern** - Unified error handling and success responses

## Architecture

```
TaskManager/
├── TaskManager.Core/          # Domain layer (entities, interfaces, validation)
│   ├── Entities/             # WorkTask, Person
│   ├── Enums/               # Status, Priority
│   ├── Interfaces/          # ITaskRepository, ITaskService
│   ├── Services/            # TaskService (business logic)
│   ├── Validation/          # FluentValidation rules
│   └── Common/              # Result pattern
│
├── TaskManager.Infrastructure/ # Data access layer
│   ├── TaskDbContext.cs     # EF Core DbContext
│   ├── TaskRepository.cs    # Repository implementation
│   ├── Configurations/      # EF entity configurations
│   └── Data/               # DataSeeder for sample data
│
├── TaskManager.UI/           # Presentation layer (WPF)
│   ├── ViewModels/          # MainViewModel, TaskViewModel
│   ├── Views/               # MainWindow, TaskDialog
│   ├── Commands/            # RelayCommand, AsyncRelayCommand
│   ├── Converters/          # Value converters for UI
│   └── Styles/              # XAML styling resources
│
└── TaskManager.Tests/        # Unit tests (xUnit + Moq)
    ├── TaskValidatorTests   # Validation rules tests
    ├── TaskServiceTests     # Business logic tests
    ├── EntityTests          # Entity model tests
    └── ResultTests          # Result pattern tests
```

## 🚀 Prerequisites

- **Windows OS** (Windows 10 or later)
- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recommended) or **Visual Studio Code**

## Installation & Running

### Option 1: Using Visual Studio 2022

1. **Clone the repository**
   ```bash
   git clone https://github.com/Rotkiw00/WPF-Task-Management-App.git
   cd WPF-Task-Management-App
   ```

2. **Open the solution**
   - Double-click `TaskManager.sln`
   - Or open in Visual Studio: File → Open → Project/Solution

3. **Restore NuGet packages**
   - Visual Studio will automatically restore packages
   - Or manually: Right-click solution → Restore NuGet Packages

4. **Build the solution**
   - Press `Ctrl + Shift + B`
   - Or: Build → Build Solution

5. **Run the application**
   - Press `F5` (Debug mode)
   - Or `Ctrl + F5` (Release mode)
   - Or: Debug → Start Debugging

### Option 2: Using Command Line

1. **Clone the repository**
   ```bash
   git clone https://github.com/Rotkiw00/WPF-Task-Management-App.git
   cd WPF-Task-Management-App
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the application**
   ```bash
   dotnet build --configuration Release
   ```

4. **Run the application**
   ```bash
   cd TaskManager
   dotnet run
   ```

## Running Tests

### Using Visual Studio
1. Open **Test Explorer** (Test → Test Explorer)
2. Click **Run All Tests**
3. View results in Test Explorer window

### Using Command Line
```bash
cd TaskManager.Tests
dotnet test
```

**Expected Output:**
```
Passed!  - Failed:     0, Passed:    43, Skipped:     0, Total:    43
```

## Database

- **Type**: SQLite
- **Location**: `%LocalAppData%\TaskManager\tasks.db`
  - Windows: `C:\Users\{YourUsername}\AppData\Local\TaskManager\tasks.db`
- **Auto-created**: On first application run
- **Sample Data**: 10 tasks + 4 people automatically seeded

### Accessing the Database

To view or modify the database manually:
1. Navigate to: `%LocalAppData%\TaskManager\`
2. Open `tasks.db` with SQLite browser or DB Browser for SQLite
3. Or delete the file to reset with fresh sample data

### Database Schema

**WorkTask**
- Id (Guid, PK)
- Title (string, required, max 200)
- Description (string, max 1000)
- Status (enum: Draft, Assigned, InProgress, UnderReview, Completed, Cancelled)
- Priority (enum: Low, Medium, High, Critical)
- CreatedDateTime (DateTime)
- DueDate (DateTime?, nullable)
- EstimatedHours (int?, nullable)
- AssignedToId (Guid?, FK to Person)
- Tags (JSON array of strings)

**Person**
- Id (Guid, PK)
- Name (string, required, max 100)
- Email (string, unique, max 100)

## How to Use

### Managing Tasks

1. **View Tasks**
   - All tasks are displayed in the DataGrid
   - Click on any task to select it

2. **Add New Task**
   - Click "Add Task" button
   - Fill in the form fields
   - Click "Save" to create

3. **Edit Task**
   - Select a task from the list
   - Click "Edit Task"
   - Modify fields
   - Click "Save" to update

4. **Delete Task**
   - Select a task
   - Click "Delete Task"
   - Confirm deletion

### Filtering & Search

1. **Filter by Status**
   - Select status from dropdown (Draft, InProgress, etc.)
   - List updates automatically

2. **Filter by Priority**
   - Select priority from dropdown (Low, Medium, High, Critical)
   - Works together with Status filter

3. **Search**
   - Type keywords in search box
   - Click "Search"
   - Searches in Title and Description

4. **Clear Filters**
   - Click "Clear" to reset all filters

### Export Data

1. **Export to CSV**
   - Click "Export CSV"
   - Choose location and filename
   - Open in Excel or any text editor

2. **Export to Excel**
   - Click "Export Excel"
   - Choose location and filename
   - Open directly in Microsoft Excel

### Adding People

1. In Task dialog, click "**+**" button next to Assigned To
2. Enter person's name
3. Click "**✓**" to add or "**✗**" to cancel

## 🔧 Technologies Used

### Frameworks & Libraries
- **.NET 8.0** - Latest LTS version
- **WPF** - Windows Presentation Foundation
- **Entity Framework Core 9.0** - ORM for database operations
- **SQLite** - Lightweight database
- **FluentValidation 12.0** - Business rules validation
- **Microsoft.Extensions.DependencyInjection** - DI container

### Testing
- **xUnit 2.5.3** - Testing framework
- **Moq 4.20.70** - Mocking library
- **Microsoft.NET.Test.Sdk** - Test SDK

## Business Rules (Validation)

1. **Title** - Required, cannot be empty
2. **Due Date** - Must be after Created Date (if set)
3. **Estimated Hours** - Required for InProgress, UnderReview, and Completed statuses
4. **Assigned Person** - Required for Assigned, InProgress, and UnderReview statuses
5. **Tags** - Cannot contain empty values

## Troubleshooting

### Application won't start
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Try cleaning and rebuilding: `dotnet clean` then `dotnet build`

### Database errors
- Delete database file: `%LocalAppData%\TaskManager\tasks.db`
- Restart application - database will be recreated with sample data

### Tests failing
- Restore NuGet packages: `dotnet restore`
- Rebuild solution: `dotnet build`
- Run tests: `dotnet test`

### Can't find database file
- Press `Win + R`
- Type: `shell:Local AppData\TaskManager`
- Or alternatively: paste this in File Explorer address bar: `%LocalAppData%\TaskManager`

## Author

**Wiktor Kalaga**
- GitHub: [@Rotkiw00](https://github.com/Rotkiw00)

## Acknowledgments

Built as a technical assessment project demonstrating:
- Clean Architecture principles
- SOLID design principles
- MVVM pattern in WPF
- Modern C# practices
- Comprehensive unit testing

---

**Note**: This application requires Windows OS to run as it uses WPF (Windows Presentation Foundation).