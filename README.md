# One-Time Access Code Extractor

A .NET 10 application designed to automate the extraction of one-time access codes from emails (e.g., Disney+) and deliver them directly to authorized users via Discord.

## 🚀 Features

- **Automated Email Scanning**: On-demand scanning of Gmail for specific service emails.
- **Discord Integration**: Receive access codes through Discord bot commands.
- **Whitelist System**: Secure access control ensuring only authorized Discord users can request codes.
- **OAuth 2.0 Authentication**: Secure integration with Google Gmail API.
- **JWT Authorization**: Secure API endpoints for management.
- **Docker Ready**: Easy deployment using containerization.

## 🛠️ Technologies

- **Framework**: .NET 10.0
- **Language**: C# 14
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Discord Library**: [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
- **Email API**: [Google Gmail API](https://developers.google.com/gmail/api)
- **Containerization**: Docker

## 📋 Prerequisites

- **.NET 10 SDK** (for local development)
- **PostgreSQL** instance
- **Google Cloud Project**:
  - Enabled Gmail API.
  - OAuth 2.0 Credentials (ClientId, ClientSecret).
- **Discord Bot Token**:
  - Create a bot on the [Discord Developer Portal](https://discord.com/developers/applications).
  - Required Gateway Intents: `Message Content`.

## ⚙️ Configuration

The application requires several configuration settings in `appsettings.json`. You can use `appsettings.json.example` as a template.

### Connection Strings
- `ConnectionStrings:Default`: PostgreSQL connection string.

### Google Auth
- `GoogleAuth:ClientId`: Your Google Client ID.
- `GoogleAuth:ClientSecret`: Your Google Client Secret.
- `GoogleAuth:RedirectUri`: The callback URL for OAuth (e.g., `https://localhost:7226/googleauth/callback`).

### Discord
- `Discord:Token`: Your Discord Bot Token.

### Provider Emails
Configure the email patterns to search for:
```json
{
  "ProviderEmails": [
    {
      "Provider": "DisneyPlus",
      "Email": "disneyplus@trx.mail2.disneyplus.com",
      "Subject": "Jednorazowy kod dostępu do Disney+"
    }
  ]
}
```

## 🚀 Getting Started

### Local Setup
1. Clone the repository.
2. Configure `appsettings.json` with your credentials.
3. Apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run --project one-time_access_code_extractor
   ```

### Docker Compose (Recommended)
1. Configure `one-time_access_code_extractor/appsettings.json` with your actual credentials and connection string.
   - **Note for Local Database Tunnels**: If you are tunneling a database from a VPS to your local `5432` port, use `host.docker.internal` instead of `127.0.0.1` or `localhost` in your connection string (e.g., `Host=host.docker.internal;Port=5432;...`).
2. Start the application:
   ```bash
   docker compose up -d --build
   ```

### Docker Setup (Manual)
1. Configure `one-time_access_code_extractor/appsettings.json` with your actual credentials and connection string.
2. Build the image:
   ```bash
   docker build -t access-code-extractor -f one-time_access_code_extractor/Dockerfile .
   ```
3. Run the container:
   ```bash
   docker run -d --name access-code-extractor -p 8080:8080 access-code-extractor
   ```

## 📖 Usage

1. **Authorize Gmail**:
   - Start the application.
   - Navigate to `/GoogleAuth/Login` to authorize the app to read your emails.
2. **Discord Commands**:
   - `!ping`: Check if the bot is alive.
   - `!disneyplus`: Request the latest Disney+ access code (requires whitelist).

## 📁 Project Structure

- `one-time_access_code_extractor/Configuration`: Dependency injection and app setup.
- `one-time_access_code_extractor/Controllers`: API endpoints for Auth and Whitelist management.
- `one-time_access_code_extractor/Data`: Entity Framework context and migrations.
- `one-time_access_code_extractor/Entities`: Database models.
- `one-time_access_code_extractor/Services`: Business logic (Discord, Gmail, Auth).
- `one-time_access_code_extractor/Repositories`: Data access layer.
