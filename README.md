# Restaurant POS System

A multi-tenant restaurant point-of-sale system with SuperAdmin portal.

## Project Structure

```
restaurent/
├── backend/           # .NET 8 API
├── admin/            # SuperAdmin Portal (React + TypeScript)
├── review/           # Documentation & SQL Schema
│   ├── database structure.txt
│   ├── workflow.txt
│   └── schema.sql    # PostgreSQL schema for Render
└── README.md
```

## Database

**PostgreSQL on Render:**
- Host: `dpg-d4plm1k9c44c73atige0-a.oregon-postgres.render.com`
- Database: `restaurant_pos_ag2k`
- Username: `restaurant_admin`

## Backend (.NET 8 API)

### Setup

```bash
cd backend
dotnet restore
dotnet run
```

### Configuration

Edit `appsettings.json` to configure:
- Database connection string
- JWT settings

### API Endpoints

| Endpoint | Description |
|----------|-------------|
| `POST /api/auth/superadmin/login` | SuperAdmin login |
| `POST /api/auth/company/login` | Company login |
| `GET /api/superadmin/dashboard` | Dashboard stats |
| `GET /api/superadmin/companies` | List companies |
| `POST /api/superadmin/companies` | Create company |
| `GET /api/superadmin/plans` | List plans |
| `GET /api/superadmin/billing` | List payments |
| `GET /api/seed/all` | Seed initial data |

### First Time Setup

1. Run the backend
2. Visit: `http://localhost:5000/api/seed/all`
3. Login with: `superadmin` / `Admin@123`

## Admin Portal (React)

### Setup

```bash
cd admin
npm install
npm run dev
```

Opens at: `http://localhost:3001`

### Features

- Dashboard with stats
- Companies management (CRUD)
- Subscription plans management
- Billing & payments tracking

## Tech Stack

### Backend
- .NET 8
- Entity Framework Core
- PostgreSQL (Npgsql)
- JWT Authentication
- BCrypt password hashing

### Frontend
- React 18
- TypeScript
- Vite
- TailwindCSS
- React Query
- React Router
- Lucide Icons

## Default Credentials

| Portal | Username | Password |
|--------|----------|----------|
| SuperAdmin | superadmin | Admin@123 |
