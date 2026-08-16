# 🏛️ BookMyHall

> Enterprise Hall Booking & Venue Management System built with **.NET 10**, **React 19**, **PostgreSQL 17**, and **Clean Architecture**.

---

## 📖 Overview

BookMyHall is a modern venue booking platform that enables customers to discover, compare, and book halls for weddings, conferences, birthdays, corporate events, and other occasions.

The solution follows **Enterprise Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles with a scalable modular structure suitable for real-world production systems.

---

# 🚀 Technology Stack

## Backend

- .NET 10
- ASP.NET Core Minimal API
- Clean Architecture
- CQRS (MediatR)
- Entity Framework Core 10
- PostgreSQL 17
- JWT Authentication
- Refresh Token
- FluentValidation
- AutoMapper
- Serilog
- Hangfire
- Firebase Cloud Messaging
- OpenAPI
- RabbitMq
- Radis Cache
- In Memory
- Power Scripts
- 

## Frontend

- React 19
- TypeScript
- Vite
- Material UI
- AG Grid
- React Query
- Zustand
- React Hook Form

---

# 🏗 Architecture

```
                React 19

                    │

                    ▼

           ASP.NET Core API

                    │

                    ▼

            Application Layer
             (CQRS + MediatR)

                    │

                    ▼

              Domain Layer

                    │

                    ▼

      Infrastructure / Persistence

                    │

                    ▼

             PostgreSQL 17
```

---

# 📂 Solution Structure

```
BookMyHall

│

├── src

│   ├── BookMyHall.Api

│   ├── BookMyHall.Application

│   ├── BookMyHall.Application.Abstractions

│   ├── BookMyHall.Contracts

│   ├── BookMyHall.Domain

│   ├── BookMyHall.Infrastructure

│   ├── BookMyHall.Persistence

│   └── BookMyHall.Shared

│

├── tests

│   ├── BookMyHall.Api.Tests

│   ├── BookMyHall.Application.Tests

│   ├── BookMyHall.Domain.Tests

│   └── BookMyHall.Infrastructure.Tests

│

├── database

├── docker

├── docs

└── scripts
```

---

# 📦 Modules

## Identity

- Authentication
- Authorization
- Roles
- Permissions
- User Management
- Refresh Tokens

---

## Master

- Country
- State
- City
- Area
- Event Type
- Hall Category
- Amenities
- Payment Methods
- Tax
- Lookup Data

---

## Venue

- Hall Management
- Pricing
- Availability
- Images
- Amenities
- Policies
- Documents
- Staff

---

## Booking

- Booking
- Guests
- Invoice
- Timeline
- Cancellation
- Refund
- Attachments

---

## Payment

- Payment
- Gateway
- Transactions
- Refunds

---

## Notification

- Email
- SMS
- WhatsApp
- Push Notification
- In-App Notification

---

## Review

- Rating
- Review
- Reply
- Report

---

## Audit

- Audit Log
- API Request Log
- Error Log
- Login History

---

## Support

- Ticket
- Comments
- Attachments
- FAQ

---

# ✨ Features

- Clean Architecture
- CQRS
- Repository Pattern
- Unit of Work
- JWT Authentication
- Refresh Token
- Role Based Authorization
- Global Exception Handling
- Audit Logging
- Localization
- Soft Delete
- Pagination
- Filtering
- Sorting
- File Upload
- Background Jobs
- Push Notifications
- Email Notifications
- WhatsApp Integration
- Redis Caching
- Output Caching
- Health Checks
- Docker Support
- CI/CD Ready

---

# 🗄 Database

Database Engine

- PostgreSQL 17+

Database Scripts

```
database

00_Extensions.sql

01_Schemas.sql

Identity.sql

Master.sql

Venue.sql

Booking.sql

Payment.sql

Review.sql

Notification.sql

Audit.sql

Support.sql

SeedData.sql
```

---

# 🛠 Development Setup

Clone repository

```bash
git clone https://github.com/your-org/BookMyHall.git
```

Restore packages

```bash
dotnet restore
```

Build solution

```bash
dotnet build
```

Run API

```bash
dotnet run --project src/BookMyHall.Api
```

---

# 🧪 Running Tests

```bash
dotnet test
```

---

# 📋 Coding Standards

- Clean Architecture
- SOLID Principles
- DRY
- KISS
- CQRS
- DDD
- Dependency Injection
- Async/Await
- Nullable Reference Types
- XML Documentation

---

# 🔒 Security

- JWT Authentication
- Refresh Tokens
- Password Hashing
- Role Based Authorization
- HTTPS
- Secure Headers
- Rate Limiting

---

# 📈 Roadmap

- [x] Database Design
- [x] Clean Architecture Skeleton
- [ ] Identity Module
- [ ] Master Module
- [ ] Venue Module
- [ ] Booking Module
- [ ] Payment Module
- [ ] Notification Module
- [ ] Review Module
- [ ] Support Module
- [ ] React Frontend
- [ ] Docker
- [ ] CI/CD
- [ ] Azure Deployment

---

# 👨‍💻 Author

**Bhushan Kachave**

Senior Software Engineer

---

# 📄 License

This project is licensed under the MIT License.

---

# ⭐ Future Enhancements

- AI-based Venue Recommendations
- AI Search
- OCR for Documents
- Voice Search
- Chatbot
- Dynamic Pricing
- Analytics Dashboard
- Mobile Application
