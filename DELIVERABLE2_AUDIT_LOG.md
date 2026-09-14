# Deliverable 2: Prompt Log & AI Architecture Audit

**Student / Engineer Name:** [Your Name]  
**Course / Module:** Enterprise Application Development  
**Activity:** Task A–D: Generic Repository Pattern & Unit of Work Implementation  
**Date:** September 14, 2026

---

## 1. System Prompt String Used

```text
Act as a Lead C# Architect. Scaffolding an asynchronous Generic Repository pattern (IRepository<T> and Repository<T>) and a Unit of Work pattern (IUnitOfWork and UnitOfWork) for an ASP.NET Core 8 MVC application. Use Entity Framework Core 8. Ensure asynchronous DB execution methods (GetAllAsync, GetByIdAsync, AddAsync, CompleteAsync) are used. Ensure the architecture hides ApplicationDbContext entirely from controllers.
```

---

## 2. AI Error Analysis & Correction Log

### Incident Log

| Item                           | Details                                                                                                                                                                             |
| :----------------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Tool / Assistant**           | GitHub Copilot / LLM                                                                                                                                                                |
| **Target Files**               | `IRepository.cs`, `Repository.cs`                                                                                                                                                   |
| **Initial Generation Failure** | Generated asynchronous signatures `Task UpdateAsync(T entity)` and `Task DeleteAsync(T entity)`, implemented via `_dbSet.UpdateAsync(entity)`.                                      |
| **Compiler Error Encountered** | `CS1061: 'DbSet<T>' does not contain a definition for 'UpdateAsync' and no accessible extension method 'UpdateAsync' accepting a first argument of type 'DbSet<T>' could be found.` |

### Technical Root Cause

In Entity Framework Core, entity state modifications—such as updating or removing entities via `DbSet<T>.Update()` or `DbSet<T>.Remove()`—are purely local in-memory operations. They modify the state of the entity within the EF Core `ChangeTracker` without making an immediate I/O or network round-trip to the SQL Server database. Because no asynchronous I/O operation takes place at that stage, EF Core intentionally does not provide `UpdateAsync` or `RemoveAsync` methods on `DbSet<T>`. The actual asynchronous database round-trip is executed when calling `SaveChangesAsync()` (or `_context.SaveChangesAsync()` via `CompleteAsync()` in the Unit of Work).

### Corrective Prompt Deployed

```text
EF Core Change Tracker methods for single entities do not have async variants. Update IRepository<T> and Repository<T> so that Update and Delete are synchronous methods, while leaving read queries (GetAllAsync, GetByIdAsync) and CompleteAsync() asynchronous.
```

### Outcome & Architectural Verification

1. `IRepository<T>` defines synchronous `void Update(T entity)` and `void Delete(T entity)`.
2. I/O-bound queries (`GetAllAsync`, `GetByIdAsync`, `FindAsync`) and transaction commits (`CompleteAsync`) remain asynchronous with non-blocking I/O.
3. Strict abstraction layer achieved: `ProductsController` references only `IUnitOfWork`, having zero direct coupling or references to `ApplicationDbContext`.
