# 🎟️ Event Booking System

A full-stack ASP.NET Core MVC web application for browsing, managing, and booking events online. The platform includes authentication, role-based authorization, event management, ticket booking, dashboards, and an AI-powered chatbot assistant using Gemini API.

---

# 🚀 Features

## 👤 Authentication & Authorization

* User registration and login using ASP.NET Identity
* Role-based access control (Admin & User)
* Secure authentication with cookies

## 📅 Event Management

* Create, edit, delete, and view events
* Categorize events
* Manage ticket types and event details

## 🎫 Booking System

* Book tickets for events
* Track bookings and payment records
* User event history tracking

## 📊 Dashboard

* Admin dashboard for monitoring events and bookings
* User-friendly event browsing interface

## 🤖 AI Chatbot Integration

* Integrated Gemini AI chatbot service
* Helps users with event-related questions and recommendations

---

# 🛠️ Technologies Used

* **ASP.NET Core MVC (.NET 8)**
* **Entity Framework Core**
* **SQL Server**
* **ASP.NET Identity**
* **Bootstrap**
* **C#**
* **Gemini API**

---

# 📂 Project Structure

```bash
EventBooking/
│
├── Controllers/
├── Models/
├── Views/
├── Services/
├── Data/
├── Migrations/
├── wwwroot/
├── Program.cs
└── appsettings.json
```

---

# ⚙️ Installation & Setup

## 1️⃣ Clone the Repository

```bash
git clone https://github.com/razanEmad/Event-Management-System-asp.net-MVC
cd event-booking-system
```

## 2️⃣ Configure the Database

Update the connection string inside:

```json
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EventBookingDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

---

## 3️⃣ Configure Gemini API Key

Replace the API key inside:

```json
"GeminiApiKey": "YOUR_API_KEY"
```

---

## 4️⃣ Apply Database Migrations

```bash
dotnet ef database update
```

---

## 5️⃣ Run the Application

```bash
dotnet run
```

The application will start on:

```bash
https://localhost:xxxx
```

---

# 🔑 Default Admin Account

The application automatically seeds a default admin account:

```text
Email: admin@eventbooking.com
Password: Admin123
```

> Change the default password after first login.

---

# 🧠 AI Chatbot Service

The project includes a Gemini-powered chatbot service located in:

```bash
Services/GeminiChatbotService.cs
```

It uses HttpClient and Gemini API integration to generate intelligent responses for users.

# 🌟 Future Improvements

* Online payment gateway integration
* Email notifications
* QR code ticket generation
* Event recommendation system
* Responsive mobile optimization
