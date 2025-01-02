# Diary API Project

## Introduction
Welcome to the Diary API project! This project is built using ASP.NET Core and represents the backend and API for a diary application. The goal of this project is to provide a simple .NET-based daily planner API powered by the most popular and cutting-edge technologies.

## Features
- **User Authentication**: Secure JWT Token system.
- **Diary Entry Management**: Create, read, update, and delete reports, role and users.
- **Data Persistence**: Store data using Entity Framework Core.
- **API Documentation**: Swagger UI for API endpoint visualization and interaction.

## Technologies
- **ASP.NET Core**: For creating the web API.
- **PostgreSQL**: To strore data
- **Entity Framework Core**: For ORM and database interactions.
- **Swagger**: For API documentation and testing.
- **RabbitMq**: RabbitMq consumer and prudecer implemented.
- **Redis**: The requested reports are cached.
- **Open Telemetry** - All activity is tracked.
- **Unit tests** - To test new features.
- **Serilog** - Logs only necessary and saves logs in `.txt` files in `JSON` format.
- **GraphQL** - For flexible requests and subscriptions.
- **JWT Tokens** - To make secure requests.

## Getting Started
To get started with this project, you'll need to have the following prerequisites installed:
- .NET Core SDK
- An IDE like Visual Studio or Rider
- A PostgreSQL Server instance for the database (or use PostgreSQL Server instance in `docker-compose.yml` 
- Docker

### Setup
1. Clone the repository to your local machine.
2. Open the solution in your IDE.
3. Restore the NuGet packages.
4. Update the connection string in `secrets.json` of `Diary.Api` to point to your PostgreSQL Server instance.
5. Set up Redis(after installing execute `redis-server`)
6. Execute `docker-compose -p diary-auxiliary-services up -d` in project directory. 
7. Start the application.

## Usage
Once the application is running, you can interact with the API using tools like Postman or directly through the Swagger UI.

## Contribution
This project is still a work in progress, and contributions are welcome. If you'd like to contribute, please:
1. Fork the repository.
2. Create a new branch for your feature.
3. Commit your changes.
4. Push to your fork.
5. Submit a pull request.

Do not contribute branches with `-EXAMPLE` tag. They are for test purposes only.
Branchers with `-NOT-COMPLETE` tag are not complete and are most likely not working.

## License
This project is licensed under the MIT License - see the `LICENSE.txt` file for details.

## Acknowledgments
- Thanks to all the contributors who have helped with the development of this project.

## Contact
For any queries or contributions, please open an issue in the repository.

