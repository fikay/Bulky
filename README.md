
# Bulky
The Goal for this project is to create an E-commerce website for movies to completion using ASP.NET MVC architecture using microsoft visual studio.

## Tools 
- Microsoft Visual Studio 2023 (preview for net 8)
- SQL Server Management Studio

## Dependencies
 - Microsoft.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.SqlServer
 - Microsoft.EntityFrameworkCore.Tools

### Category CRUD Operations

We have created a category model which implements the CRUD Operations to include movie categories and everything looks to be fully functional.  
We connected this operation to a server **FAKS/FIKAYOMYSQL** and created a Database **Razor** where the table category was created. 

# Adding Repository
We created a new service called Repository which contains interfaces used for the basic CRUD Operations. We also created an ICategory interface to contain functionality for the **edit** action

## Interfaces
- IRepository - contains the ** Add, Delete, DeleteRange, GetAll ** methods
- Repository - contains the implemented methods for the IRepository
- ICategoryRepository - contains the ** Save and Update** methods
- CategoryRepository  - contains the implemented methods for the ICategoryRepository

## CRUD Operations
- Create Category
- Edit Category
- Delete Category

Once the Repositories were implemented, they were included as a service in the Program:
```c#
using BulkyWeb.DataAccess.Repository.IRepository;


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
```
### Dependency Lifetimes
- Transient - New Service is created every time requested
- Scoped - New Service is created once per request (Most Recommended )
- Singleton - New Service is created once per application lifetime
  
`AddScoped` was utilized in order to create the service once per request. 

The ICategoryRepository was then implemented in the category Controller

