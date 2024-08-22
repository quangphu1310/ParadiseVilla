## Paradise Villa - Villa Management System

Paradise Villa is a villa management system built as a RESTful web API in ASP.NET Core and consume them effectively in an ASP.NET Core Web application. The system features secure authentication and authorization using JWT, comprehensive villa and villa number management, file and image processing, and support for multiple API versions. The project is structured with a multi-project architecture, utilizing the repository pattern, dependency injection, DTO models, and AutoMapper to ensure scalability and maintainability.

## About The Project

This project enables users to effectively manage villas and villa numbers, while also providing robust role-based access control.


## Built with
* [![ASP.NET Web API][WebAPI-logo]][WebAPI-url]
* [![ASP.NET Core MVC][MVC-logo]][MVC-url]
* [![SQL Server][SQLServer-logo]][SQLServer-url]
* [![Entity Framework][EF-logo]][EF-url]
* [![Bootstrap][Bootstrap-logo]][Bootstrap-url]
* [![Identity][Identity-logo]][Identity-url]
* [![JWT][JWT-logo]][JWT-url]
    
[Bootstrap-logo]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[WebAPI-logo]: https://img.shields.io/badge/ASP.NET_Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[WebAPI-url]: https://dotnet.microsoft.com/apps/aspnet/apis
[MVC-logo]: https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[MVC-url]: https://dotnet.microsoft.com/apps/aspnet/mvc

[SQLServer-logo]: https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white
[SQLServer-url]: https://www.microsoft.com/en-us/sql-server

[EF-logo]: https://img.shields.io/badge/Entity_Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[EF-url]: https://docs.microsoft.com/en-us/ef/

[Identity-logo]: https://img.shields.io/badge/Identity-4B0082?style=for-the-badge&logo=dotnet&logoColor=white
[Identity-url]: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity

[JWT-logo]: https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white
[JWT-url]: https://jwt.io/

## Getting Started
### Requirements
Before you begin, ensure you have the following prerequisites:
- .NET Core SDK installed on your machine.
- A code editor, preferably Visual Studio or Visual Studio Code.
- SQL Server or any other database server of your choice.

### Setup
To explore the QUANGPHUWEB project, follow these steps:

1. Clone the Repository
Clone this repository to your local machine using:
```bash
git clone https://github.com/quangphu1310/ParadiseVilla.git
```
2. Configure the Database Connection
Update the appsettings.json file with your database connection string:
```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
  }
```
3. To create the database and apply the initial schema, run the following commands
- Open Package Manager Console:
```bash
Add-Migration InitialCreate
Update-Database
```
4. Run the Application
- Configure Multiple Startup Projects:
  ![image](https://github.com/user-attachments/assets/3a22d271-d9e0-465b-91c4-ebee998524f6)

- Press F5 in your IDE to start the application.

## Images From The Project
![image](https://github.com/user-attachments/assets/627b5b42-2800-4087-924a-ccc7cc1d4b7e)
![image](https://github.com/user-attachments/assets/48d39b83-71d3-482a-a696-7f3323609f70)
![image](https://github.com/user-attachments/assets/9cf0e29d-f3fd-4e19-b877-ead9d50daba0)
![image](https://github.com/user-attachments/assets/ecb324b1-6805-4f02-9e34-8486f8e88b81)
![image](https://github.com/user-attachments/assets/690bf366-201d-4f8e-8de5-9740b55f44b7)
![image](https://github.com/user-attachments/assets/b1057d31-a870-4efd-bd8b-dba42b8e0fe1)


