Event Management System is a web application for creating, viewing, and managing events.
The project consists of an Angular frontend and an ASP.NET Core Web API backend, connected via REST API.
Everything is fully containerized with Docker and uses PostgreSQL as the database.

Technologies

Frontend: Angular

Backend: ASP.NET Core Web API (.NET 8)

Database: PostgreSQL

Containerization: Docker & Docker Compose

Reverse Proxy / Static Hosting: Nginx

📁 Project Structure
EventManagementSystem/
├── Api/                        # ASP.NET Core Web API
│   └── EventManagementSystemApi/
├── Client/                     # Angular frontend
│   └── EventManagementSystemClient/
├── docker-compose.yml           # Docker orchestration
├── .env                         # Environment variables (not committed)
└── README.md


🐳 Running the Project with Docker

Clone the repository:

git clone https://github.com/Cureson228/EventManagementSystem.git
cd EventManagementSystem


Build and start the containers:

docker compose up --build


After startup, the app will be available at:

Frontend → http://localhost:4200

Backend (Swagger) → http://localhost:7189/swagger

PostgreSQL database → available on port 5432 (inside the db container)

🧠 Useful Commands

Stop all containers:

docker compose down


View API logs:

docker compose logs api -f

🧩 Author

Author: Bohdan Ratushnyi
GitHub: https://github.com/Cureson228
