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

