
# Bulky
The Goal for this project is to create an E-commerce website for movies to completion using ASP.NET MVC architecture using microsoft visual studio.

## Tools 
- Microsoft Visual Studio 2023 (preview for net 8)
- SQL Server Management Studio
  
## Dependencies
 - Microsoft.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.SqlServer
 - Microsoft.EntityFrameworkCore.Tools
   
## Connecting Database
The following code is input into the appsettings.json to connect to the database and using the `Microsoft.EntityFrameworkCore` dependencies, we are able to create a class which inherits the DbContext. We also implenet it as a service in the `Program.cs` file.
To make model changes or seed the database, we run the following command in the Package Manager Console

```
update-databse
```

```
"AllowedHosts": "*",
"ConnectionStrings": {
  "DefaultConnection": "Server=FAKS\\FIKAYOMYSQL;Database=Razor;Trusted_Connection=true;TrustServerCertificate=True"
}
```


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


### Identity and Authentication
Introducing Identity and authentication into the application using the ASP.NETCore.Identity.EntityFrameworkcore.

We inherited the IdentityDbcontext from the package and now we will be scafolding the project for identity.
```
public class ApplicationDbContext : IdentityDbContext{

}

This is required in order for it to work
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
     base.OnModelCreating(modelBuilder);
}
```


### Connecting and Deploying to AZURE

####Create a SQL server
In order to have our local database online, we have to create an azure sql server.

**Create Firewall for SQL DB**
![image](https://github.com/fikay/Bulky/assets/32597117/8d79c474-a882-464d-8dcc-e2ff63c68819)

Once the firewall has been created, We go to the connection string and copy the string to use when we are deploying.

### Deploying  Project
We created the project in .NET 8 preview so to prevent deployment issues we have to downgrade to Net 7.

#### Steps
-Edit Project File of all projects and change TargetFramework and package reference to 7 (optional)
- ![image](https://github.com/fikay/Bulky/assets/32597117/d7737a31-1677-4df0-ae00-33bb470e14a5)
- Right clickk on the main Project and click **Publish** ![image](https://github.com/fikay/Bulky/assets/32597117/cad8d15e-c48f-47e2-8c94-11586b8d762d)
- To do this from Azure directly, 
![image](https://github.com/fikay/Bulky/assets/32597117/0a3976de-e3ea-41d1-9a28-a0ea8d6c6278)
![image](https://github.com/fikay/Bulky/assets/32597117/fb4cebf1-a45a-42dd-b973-3015b57189fc)
![image](https://github.com/fikay/Bulky/assets/32597117/9344c38f-572e-40ba-bd4c-2eb3809873d9)
In order to automatically make changes to the web App directly from github changes, we enable continuous deployment
![image](https://github.com/fikay/Bulky/assets/32597117/04889af8-8460-4dec-88ef-c8d66c447ebc)

Then we enable public access. Once it has been deployed, we have to connect to the azure Database unless the application will not work.

#### Steps to connect Database
- Head over to the database in Azure to get the connection string
- Head over to the project in Visual studio and create a new `appsettings.Production.json` file
- copy all items in the `appsettings.json` file and put it into the newly created file, then change the Db connection string to that of the azure Db.
- push to gitHub




