# Restoration and cleanup notes

## Original environment

The server targets `netcoreapp3.1`. The React client uses React 16 and Create React App 3, with the submitted `package-lock.json`. The database code uses PostgreSQL/Npgsql, Entity Framework and IdentityServer. Docker files describe the historical packaging; the backend image does not install Node, so they need review before use with the client build.

## Configuration

Run development commands from `src/PBDASHBOARD`. Supply `ConnectionStrings__DefaultConnection` and `JwtConfig__Secret` as environment variables. The database context now reads environment overrides as well as `appsettings.json`. Choose a long random JWT secret locally. If the administrator initializer is used, also set `INITIAL_ADMIN_PASSWORD` and optionally `INITIAL_ADMIN_EMAIL`; the initializer no longer contains a password.

The `.env.example` file is a template for Docker Compose. Copy it to `.env` locally and replace the placeholders. A `.env` file is not automatically loaded by `dotnet run`; export the required settings in the shell or use the host's configuration system.

With a compatible .NET/Node toolchain, the historical entry points are:

```sh
cd src/PBDASHBOARD
dotnet restore
cd ClientApp
npm ci
cd ..
dotnet run
```

These commands are setup guidance, not a successful build transcript. Review `Startup.cs` before restoration: the explicit `AddDbContext` registration is commented out in the submission, while Identity, background tasks and controllers depend on the database context. Database migrations, identity roles and the external API also require verification. Do not connect the archived app to a production database while restoring it.

## Changes made for GitHub

- Kept source and submitted report, slides, demonstration and PDF appendices.
- Removed local `.env`, compiled `wwwroot`, IDE state, `bin`, `obj` and runtime logs.
- Removed database credentials and the JWT secret from JSON configuration files.
- Replaced the hardcoded administrator password with an environment setting and waited for account creation before assigning a role.
- Added environment configuration overrides and passed them through Compose.
- Pointed the React development server at the actual `ClientApp` source directory.
- Omitted the raw source ZIP because it includes the removed credentials and generated files. Omitted the obsolete VM/link note and deployment helper that refers to the original internal proxy.

## Validation

Configuration JSON and project XML were parsed, repository links were checked, and the cleaned source was scanned for the removed credential values. Original report and appendix PDFs remain byte-identical. The larger presentation and demonstration are preserved losslessly as parts; `scripts/restore_artifacts.py` reconstructs and verifies their original bytes. This environment has no .NET SDK, so the server was not compiled, the database was not migrated and the React/API application was not run. No new performance or functionality claims are made.
