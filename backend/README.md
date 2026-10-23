# Backend – TaskManager.Api

ASP.NET Core Web API (.NET 10) + Entity Framework Core + PostgreSQL.

> Masz starszą wersję kodu (pod SQL Server)? Zobacz [MIGRACJA-POSTGRESQL.md](MIGRACJA-POSTGRESQL.md).

## Baza danych

API korzysta z bazy `task_manager` (tabele `users`, `tasks`, `comments`).
Schemat jest w pliku [`database/schema.postgresql.sql`](../database/schema.postgresql.sql) –
API nie tworzy ani nie zmienia tabel (brak migracji EF).

1. Utwórz bazę i tabele (z katalogu głównego repozytorium):

   ```bash
   psql -U postgres -c "CREATE DATABASE task_manager"
   psql -U postgres -d task_manager -f database/schema.postgresql.sql
   ```

   Albo w pgAdmin: utwórz bazę `task_manager`, otwórz na niej *Query Tool* i uruchom zawartość pliku.

2. Skopiuj `TaskManager.Api/appsettings.Local.example.json` do `TaskManager.Api/appsettings.Local.json`
   (plik jest w `.gitignore` – hasło nie trafia do repozytorium) i wpisz swoje hasło:

   ```
   Host=localhost;Port=5432;Database=task_manager;Username=postgres;Password=HASLO
   ```

   Można też użyć zmiennej środowiskowej `ConnectionStrings__TaskManagerDb`.

3. Po uruchomieniu API sprawdź `GET http://localhost:5113/api/health/db` – powinno zwrócić `{"database":"ok"}`.

## Uruchomienie

```bash
dotnet run --project TaskManager.Api --launch-profile http
```

API: `http://localhost:5113`, opis OpenAPI: `http://localhost:5113/openapi/v1.json`.

Frontend: w trybie Development API serwuje też katalog `frontend/`, więc aplikacja działa pod
`http://localhost:5113/`. Można ją też uruchomić przez Live Server (port 5500) – adres API jest
w stałej `API_URL` w `frontend/js/app.js`.
Przykładowe zapytania: `TaskManager.Api/TaskManager.Api.http`.

## Endpointy

| Metoda | Ścieżka                      | Opis                                                  |
|--------|------------------------------|-------------------------------------------------------|
| GET    | `/api/health`                | czy API działa                                        |
| GET    | `/api/health/db`             | czy jest połączenie z bazą                            |
| GET    | `/api/tasks?status=`         | lista zadań (opcjonalny filtr)                        |
| GET    | `/api/tasks/{id}`            | jedno zadanie                                         |
| POST   | `/api/tasks`                 | nowe zadanie `{title, description, status, userId}`   |
| PUT    | `/api/tasks/{id}`            | edycja zadania `{title, description, status}`         |
| PATCH  | `/api/tasks/{id}/status`     | zmiana statusu `{status}`                             |
| DELETE | `/api/tasks/{id}`            | usunięcie zadania (razem z komentarzami)              |
| GET    | `/api/tasks/{id}/comments`   | komentarze zadania                                    |
| POST   | `/api/tasks/{id}/comments`   | nowy komentarz `{content, userId}`                    |
| DELETE | `/api/comments/{id}`         | usunięcie komentarza                                  |
| GET    | `/api/users`                 | lista użytkowników (bez haseł)                        |
| GET    | `/api/users/{id}`            | jeden użytkownik                                      |

Statusy: `new`, `in_progress`, `done` (jak we frontendzie).
CORS dla frontendu: `Cors:AllowedOrigins` w `appsettings.Development.json` (domyślnie Live Server, port 5500).
