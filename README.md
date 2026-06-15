Blood Donation Management System

A web-based Blood Donation Management System developed using ASP.NET Core MVC (.NET 8) and SQLite. The application bridges the gap between blood donors and patients in need by providing a structured and secure platform for managing donations and blood requests.



 Features

 Donor Portal
- Register as a blood donor with blood type, contact details, and city
- Secure login with hashed password authentication
- Personal dashboard with profile summary
- Toggle availability status as available or unavailable
- Submit blood requests on behalf of patients
- Search for available donors by blood type and city
- Update profile anytime after login

Admin Portal
- Real-time dashboard with total donors, pending, approved, and emergency request stats
- Approve, reject, or escalate blood requests to emergency
- Edit or delete donor profiles
- Reports page with blood type distribution across all donors
- View all registered users

Security
- Session-based authentication with role separation for Admin and User
- Custom AuthFilter for route-level access control
- SHA-256 password hashing
- CSRF protection via anti-forgery tokens


Tech Stack
- Framework: ASP.NET Core MVC (.NET 8)
- Database: SQLite via Entity Framework Core 8
- Frontend: Razor Views, HTML, CSS, Bootstrap
- Authentication: Session-based with custom Auth Filter
- Language: C#
- IDE: Visual Studio 2022





Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension





Project Structure

BloodDonationApp/
├── Controllers/
│   ├── AccountController.cs       # Login, logout, session management
│   ├── AdminController.cs         # Admin dashboard and management
│   ├── BloodRequestController.cs  # Submit and view blood requests
│   ├── DonorController.cs         # Registration, profile, search
│   └── HomeController.cs          # Landing page, about, contact
├── Models/
│   ├── Donor.cs                   # Donor entity with validation
│   └── BloodRequest.cs            # Blood request entity
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DB context
│   ├── DbInitializer.cs           # Seeds initial data on startup
│   └── PasswordHasher.cs          # SHA-256 hashing utility
├── Filters/
│   └── AuthFilter.cs              # Role-based session auth filter
└── Views/
    ├── Admin/                     # Admin panel views
    ├── Donor/                     # Donor portal views
    ├── BloodRequest/              # Blood request forms
    └── Shared/                    # Layouts for Admin, User, Public



