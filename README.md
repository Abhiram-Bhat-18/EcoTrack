# 🌱 EcoTrack — Smart Waste Management System

EcoTrack is a web application designed to help users report waste-related problems and enable administrators to manage waste reports, monitor collection progress, and support better waste management practices.

## 🎯 Project Objectives

- Make it easier to report waste-related issues.
- Track waste reports from submission to resolution.
- Provide administrators with tools to manage reports and users.
- Organize waste collection information in a centralized database.
- Support environmental awareness through waste management data.

## 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| C# | Backend programming and business logic |
| ASP.NET Core Web API | Backend services and REST APIs |
| SQL Server | Relational database |
| Entity Framework Core | Database access and ORM |
| HTML | Web page structure |
| CSS | User interface styling |
| JavaScript | Frontend interactions and API requests |
| Git and GitHub | Version control and source code management |

## ✨ Key Features

- **Waste reporting:** Submit reports describing waste-related problems.
- **Report management:** Retrieve and manage submitted reports.
- **Status tracking:** Track reports through their collection or resolution workflow.
- **User authentication:** Support authenticated access to application features.
- **Role-based administration:** Restrict administrative operations according to user permissions.
- **Database integration:** Store application data using SQL Server and Entity Framework Core.

## 🏗️ Application Architecture

The application follows a client-server architecture.

1. **Frontend:** HTML, CSS, and JavaScript provide the user interface.
2. **Backend:** ASP.NET Core Web API processes HTTP requests and applies application logic.
3. **Data access:** Entity Framework Core connects C# application models to SQL Server.
4. **Database:** SQL Server stores users, waste reports, and related application records.

The frontend communicates with the backend through HTTP requests and JSON responses.

## 📁 Project Structure

```text
EcoTrack/
├── EcoTrack.API.slnx
└── EcoTrack.API/
    ├── Controllers/
    ├── Data/
    ├── DTOs/
    ├── Migrations/
    ├── Models/
    ├── Security/
    ├── Services/
    ├── wwwroot/
    ├── Program.cs
    ├── appsettings.json
    └── EcoTrack.API.csproj
```

*Note: The structure above represents the intended solution layout. Adjust it to match the actual files in the repository.*

## 🚀 Getting Started

### Prerequisites

- Windows or another supported development environment
- A compatible .NET SDK
- Visual Studio with the ASP.NET and web development workload, or an equivalent .NET development setup
- SQL Server
- SQL Server Management Studio (optional, for database administration)

### Installation

1. Clone this repository:

   ```bash
   git clone https://github.com/Abhiram-Bhat-18/EcoTrack.git
   ```

2. Open `EcoTrack.API.slnx` in Visual Studio.

3. Configure the SQL Server connection string using local development secrets or another secure configuration method.

4. Configure the JWT signing key and any other required settings securely.

5. Restore dependencies and build the solution.

6. Apply the existing Entity Framework Core migrations to your development database, if migrations are configured for this project.

7. Run the ASP.NET Core application.

8. Open the local URL shown by Visual Studio or the application terminal.

**Configuration note:** Exact database setup and migration commands depend on the current project configuration. Never commit real passwords, JWT signing keys, or other secrets.

## 🌍 Environmental Impact

EcoTrack aims to support more organized waste reporting and collection management.

Potential outcomes include improved visibility into unresolved waste issues, better tracking of collection activities, and more informed waste management decisions.

Environmental impact should be measured using actual project data rather than assumed.

## 🔮 Future Enhancements

- Interactive map of reported waste locations
- Waste collection and recycling analytics
- Email notifications for report updates
- Photo attachments for waste reports
- Cloud deployment using Microsoft Azure

## 👨‍💻 Project Status

This project is being developed as a practical demonstration of Microsoft web development technologies, REST API design, database integration, and software engineering fundamentals.

## 📄 License

A license has not yet been specified. Please add an appropriate license if you intend to distribute or reuse this project publicly.
