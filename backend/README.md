# Backend
## Przeznaczenie
Odpowiada za integracje z zewnętrznymi systemami
## Komunikacja
Backend udostępnia API i komunikuje się z bazą PostgreSQL.
## Planowane endpointy
- GET /api/tasks
- POST /api/tasks
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}
## Proponowana struktura
```text
backend/ Controllers/
 Models/
 Services/
 Data/
 Tests/
```

