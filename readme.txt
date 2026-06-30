HOTEL RESERVATIONS MANAGER
==========================

A web-based hotel reservation management system built with ASP.NET Core MVC.
The application allows administrators to manage hotel rooms, customers, and
reservations, while providing clients with a secure way to browse rooms and
create reservations.

------------------------------------------------------------
FEATURES
------------------------------------------------------------

• User authentication and authorization with ASP.NET Core Identity
• Role-based access control (Admin / Client)
• Room management
• Customer management
• Reservation management
• Automatic reservation price calculation
• Data validation and business rules
• SQL Server database integration using Entity Framework Core

------------------------------------------------------------
USER ROLES
------------------------------------------------------------

Administrator
- Create, edit, and delete rooms
- Create, edit, and delete customers
- Create, edit, and delete reservations
- Full system access

Client
- View available rooms
- View customer information
- Create reservations for themselves
- Restricted from modifying existing data
- Unauthorized actions display an "Access Denied" message

------------------------------------------------------------
TECHNOLOGIES
------------------------------------------------------------

- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Razor Views
- HTML
- CSS

------------------------------------------------------------
DATABASE MODELS
------------------------------------------------------------

Client
- Full Name
- Phone Number
- Adult Status

Room
- Room Number
- Capacity
- Room Type
- Price

Reservation
- Room
- Client
- Check-in Date
- Check-out Date
- Total Price

AppUser
- User Account
- Role
- Active Status

------------------------------------------------------------
BUSINESS RULES
------------------------------------------------------------

- Required field validation
- Maximum phone number length: 10 characters
- Check-out date must be after check-in date
- Reservation total is calculated automatically based on:
  - Number of nights
  - Room price
- Unique room numbers

------------------------------------------------------------
PROJECT PURPOSE
------------------------------------------------------------

This project was developed as a university coursework assignment for the
Internet Programming course. It demonstrates practical implementation of:

- ASP.NET Core MVC architecture
- Entity Framework Core
- ASP.NET Identity
- Role-based authorization
- CRUD operations
- Database-driven web applications
