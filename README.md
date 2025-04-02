# Chat Room Application

A real-time chat application built with Angular 19 for the frontend and .NET for the backend.

## Project Structure

- `chat-room-app-ui/` - Angular frontend application
- `ChatRoom/` - .NET backend application
  - `ChatRoom.API/` - Main API project
  - `ChatRoom.Tests/` - Test project

## Prerequisites

- Node.js (Latest LTS version)
- .NET 8.0 SDK
- Angular CLI (`npm install -g @angular/cli`)

## Frontend Setup (chat-room-app-ui)

1. Navigate to the frontend directory:
   ```bash
   cd chat-room-app-ui
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:4200`

## Backend Setup (ChatRoom)

1. Navigate to the backend directory:
   ```bash
   cd ChatRoom
   ```

2. Restore .NET packages:
   ```bash
   dotnet restore
   ```

3. Run the API project:
   ```bash
   cd ChatRoom.API
   dotnet run
   ```
   The API will be available at `http://localhost:5076`
   The API documentation (Swagger UI) will be available at `http://localhost:5076/swagger`

## Testing

### Backend Tests
```bash
cd ChatRoom
dotnet test
```