# Przejście z SQL Server na PostgreSQL

Dotyczy osób, które pobrały backend w wersji pod Microsoft SQL Server (commit `82ee843`).

## Sposób 1 – pobranie nowej wersji (zalecany)

Jeśli masz repozytorium sklonowane przez git i nie zmieniałeś plików z listy poniżej:

```bash
git pull
```

Potem wykonaj tylko kroki **5–7** (baza, connection string, sprawdzenie).

Jeśli `git pull` zgłasza konflikt, a nie zależy Ci na własnych zmianach w tych plikach:

```bash
git checkout -- backend/TaskManager.Api
git pull
```

## Sposób 2 – ręczne zmiany

Wszystkie ścieżki są względem katalogu `backend/TaskManager.Api`.

### 1. Pakiet NuGet – `TaskManager.Api.csproj`

Usuń pakiety SQL Servera i dodaj sterownik PostgreSQL:

```bash
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
dotnet remove package System.Security.Cryptography.Pkcs
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

Albo ręcznie w pliku `.csproj` – zamiast:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.13" />
<PackageReference Include="System.Security.Cryptography.Pkcs" Version="10.0.13" />
```

wpisz:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.3" />
```

### 2. `Program.cs` – dostawca bazy

```csharp
// było
options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
// jest
options.UseNpgsql(connectionString, sql => sql.EnableRetryOnFailure()));
```

Opcjonalnie popraw komunikat błędu przy braku connection stringa, żeby wskazywał na
`appsettings.Local.json`.

### 3. `Data/AppDbContext.cs` – funkcja daty

W trzech miejscach (`users`, `tasks`, `comments` → `created_at`):

```csharp
// było
.HasDefaultValueSql("SYSDATETIME()")
// jest
.HasDefaultValueSql("now()")
```

### 4. `appsettings.Development.json` – usuń stary connection string

Usuń cały blok:

```json
"ConnectionStrings": {
  "TaskManagerDb": "Server=DESKTOP-QVGC1HI;Database=Task_manager;Trusted_Connection=True;TrustServerCertificate=True"
},
```

Zostaw w pliku tylko sekcje `Logging` i `Cors`.

### 5. Baza danych PostgreSQL

Weź plik [`database/schema.postgresql.sql`](../database/schema.postgresql.sql) z repozytorium
i z katalogu głównego projektu uruchom:

```bash
psql -U postgres -c "CREATE DATABASE task_manager"
psql -U postgres -d task_manager -f database/schema.postgresql.sql
```

W pgAdmin: *Databases → Create → Database…* (`task_manager`), potem *Query Tool* na tej bazie
i uruchom zawartość pliku.

### 6. Connection string – `appsettings.Local.json`

Utwórz plik `appsettings.Local.json` (obok `appsettings.json`) z treścią jak w
`appsettings.Local.example.json`, wpisując hasło ustawione przy instalacji PostgreSQL:

```json
{
  "ConnectionStrings": {
    "TaskManagerDb": "Host=localhost;Port=5432;Database=task_manager;Username=postgres;Password=HASLO"
  }
}
```

Plik jest w `.gitignore` – **nie commituj hasła**. Jeśli miałeś wcześniej `appsettings.Local.json`
z connection stringiem SQL Servera (`Server=...;User Id=...`), zamień go na powyższy.

### 7. Sprawdzenie

```bash
dotnet build
dotnet run --launch-profile http
```

Otwórz `http://localhost:5113/api/health/db` – powinno być `{"database":"ok"}`,
a `http://localhost:5113/` pokaże tablicę z 5 zadaniami.

## Typowe błędy

| Komunikat | Przyczyna |
|---|---|
| `password authentication failed for user "postgres"` | złe hasło w `appsettings.Local.json` |
| `database "task_manager" does not exist` | nie wykonano kroku 5 albo baza ma inną nazwę |
| `relation "tasks" does not exist` | baza istnieje, ale nie uruchomiono skryptu `schema.postgresql.sql` |
| `Failed to connect to 127.0.0.1:5432` | PostgreSQL nie działa – uruchom usługę `postgresql-x64-…` |
| `Brak connection stringa 'ConnectionStrings:TaskManagerDb'` | brak pliku `appsettings.Local.json` (krok 6) |
| `The type or namespace name 'Npgsql'…` / `UseNpgsql` nie istnieje | nie dodano pakietu (krok 1) – uruchom `dotnet restore` |
