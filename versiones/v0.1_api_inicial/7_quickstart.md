# Quickstart

This guide documents local foundation execution for the repository.

## Expected workflow

```bash
git clone <repository-url>
cd <repository>
docker compose up -d --build
docker compose ps
dotnet build
docker compose down
```

Notes:

- Domain smoke tests are not defined yet in this foundation phase.
- Complete the active version specification before implementing domain features.
