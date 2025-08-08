# Repository Guidelines

## Project Structure & Module Organization
- `Core/`: core abstractions and definitions (e.g., `IEndpointDefinition`, DI and auth helpers).
- `Core/Definitions/` and `Core/BaseConcretes/`: base endpoint patterns and reusable building blocks.
- `Extensions/`: extension methods (e.g., `QuickApiExtensions.cs`) for `IServiceCollection`, middleware, and setup.
- `Attributes/`: custom attributes for ordering and helper discovery.
- `Helpers/` and `Settings/`: small interfaces and configuration contracts.
- `MY.QuickAPI.csproj`: .NET 8 class library packaged to NuGet; output in `bin/<Config>/`.

## Build, Test, and Development Commands
- Restore: `dotnet restore`
- Build (packages on build): `dotnet build -c Release`
- Optional pack: `dotnet pack -c Release` (NuGet in `bin/Release/`)
- Clean: `dotnet clean`
- Tests (if present): `dotnet test -c Release`

## Coding Style & Naming Conventions
- C#: 4-space indent, `nullable` enabled, target `net8.0`.
- Naming: `PascalCase` for types/methods; `camelCase` for locals/params; interfaces prefixed with `I`.
- Extensions: static class + static methods; name like `SomethingExtensions` in `Extensions/`.
- Files: one public type per file; match file name to type.
- Analyzers/formatting: use `dotnet format` before PRs.

## Testing Guidelines
- Framework: xUnit (suggested). Create `MY.QuickAPI.Tests` sibling project referencing this library.
- Naming: `MethodName_ShouldExpectedOutcome_WhenCondition`.
- Coverage: aim for meaningful tests around endpoint definitions, DI wiring, and auth helpers.
- Run: `dotnet test` locally and in CI.

## Commit & Pull Request Guidelines
- Commits: imperative mood, concise subject (e.g., “Enhance endpoint error handling”).
- Scope tags optional (e.g., `Core`, `Extensions`). Keep < 72 chars.
- PRs: include description, rationale, linked issues, and before/after notes. Add usage snippets when changing APIs.
- Checks: build green, formatted code, and tests added/updated as needed.

## Security & Configuration Tips
- Do not commit secrets. Use environment variables or user-secrets for JWT and connection settings.
- Prefer configuring auth via helpers in `Core` and `Extensions` to keep hosts minimal.
- Validate inputs and surface consistent problem responses using provided base endpoint patterns.

