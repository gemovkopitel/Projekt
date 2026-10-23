# Backend – TaskManager.Api

ASP.NET Core Web API (.NET 10) + Entity Framework Core + Microsoft SQL Server.

## Baza danych

API korzysta z istniejącej bazy `Task_manager` (tabele `users`, `tasks`, `comments`).
Schemat jest utrzymywany w bazie – API nie tworzy ani nie zmienia tabel (brak migracji EF).

Połączenie jest ustawione w `TaskManager.Api/appsettings.Development.json`:

```
Server=DESKTOP-QVGC1HI;Database=Task_manager;Trusted_Connection=True;TrustServerCertificate=True
```

`Trusted_Connection=True` oznacza **uwierzytelnianie Windows** – API loguje się do SQL Servera
jako użytkownik Windows, który je uruchomił (np. `DESKTOP-QVGC1HI\Wojtek`), więc login i hasło
nie są potrzebne.

Inny serwer albo logowanie loginem SQL: skopiuj `appsettings.Local.example.json` do
`appsettings.Local.json` (plik jest w `.gitignore`) i wpisz tam swój connection string.
Można też użyć zmiennej środowiskowej `ConnectionStrings__TaskManagerDb`.

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
