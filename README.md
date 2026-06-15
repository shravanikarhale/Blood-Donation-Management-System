Blood Donation Management System
A full-stack web application built with ASP.NET Core MVC and SQLite that connects blood donors with patients in need. Separate portals for donors and administrators to manage donations, blood requests, and donor records.
 Features
 
Donor Portal
Register as a blood donor with blood type, contact info, and location
Secure login with hashed password authentication
Personal dashboard with profile summary and donation history
Toggle availability — mark yourself as available or unavailable to donate
Submit blood requests on behalf of patients
Search donors by blood type and city

Admin Portal
Dashboard with real-time stats: total donors, pending/approved/emergency requests
Approve, reject, or escalate blood requests to emergency
Edit or delete donor profiles
Reports page with blood type distribution and request status breakdown

Security
Session-based authentication with role separation (Admin / User)
Custom AuthFilter for route-level access control
SHA-256 password hashing
CSRF protection via anti-forgery tokens


Tech Stack
Framework: ASP.NET Core MVC (.NET 8)
Database: SQLite via Entity Framework Core 8
Frontend: Razor Views (CSHTML), HTML, CSS, Bootstrap
Authentication: Session-based with custom Auth Filter
IDE: Visual Studio 2022


Prerequisites

.NET 8 SDK — https://dotnet.microsoft.com/download/dotnet/8.0
Visual Studio 2022 or VS Code with C# extension

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




🔮 Future Improvements

Email notifications when a blood request is approved or matched
Blood type compatibility matching (e.g., O- can donate to all groups)
SMS alerts for emergency requests
Google Maps integration to find nearby donors
