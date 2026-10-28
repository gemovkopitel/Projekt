const http = require("http");
const { Pool } = require("pg");

// Konfiguracja połączenia z Twoją bazą danych PostgreSQL
const pool = new Pool({
  user: "postgres",
  host: "localhost",
  database: "task_manager",
  password: "postgres", // Введите пароль от PostgreSQL, который указывали при установке
  port: 5432,
});

const STATUSES = ["new", "in_progress", "done"];

function send(res, status, body) {
  res.writeHead(status, {
    "Content-Type": "application/json; charset=utf-8",
    "Access-Control-Allow-Origin": "*",
    "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, OPTIONS",
    "Access-Control-Allow-Headers": "Content-Type",
  });
  res.end(body === undefined ? "" : JSON.stringify(body));
}

http.createServer(async (req, res) => {
  if (req.method === "OPTIONS") return send(res, 204);
  const match = req.url.match(/^\/api\/tasks(?:\/(\d+))?$/);
  if (!match) return send(res, 404, { error: "Nie znaleziono" });
  const id = match[1] ? Number(match[1]) : null;

  let raw = "";
  req.on("data", (c) => (raw += c));
  req.on("end", async () => {
    let data = {};
    try {
      data = raw ? JSON.parse(raw) : {};
    } catch {
      return send(res, 400, { error: "Niepoprawny JSON" });
    }

    try {
      // GET /api/tasks – Pobranie wszystkich zadań z bazy
      if (req.method === "GET" && id === null) {
        const result = await pool.query("SELECT id, title, description, status FROM tasks ORDER BY id ASC");
        return send(res, 200, result.rows);
      }

      // POST /api/tasks – Dodanie nowego zadania do bazy (domyślnie user_id = 2 np. 'anna')
      if (req.method === "POST" && id === null) {
        if (!data.title || !data.title.trim()) {
          return send(res, 400, { error: "Tytuł jest wymagany." });
        }
        const status = STATUSES.includes(data.status) ? data.status : "new";
        const query = `
          INSERT INTO tasks (title, description, status, user_id) 
          VALUES ($1, $2, $3, 2) 
          RETURNING id, title, description, status;
        `;
        const values = [data.title.trim(), data.description || null, status];
        const result = await pool.query(query, values);
        return send(res, 201, result.rows[0]);
      }

      // Sprawdzenie czy zadanie o danym ID istnieje w bazie
      if (id !== null) {
        const checkTask = await pool.query("SELECT * FROM tasks WHERE id = $1", [id]);
        if (checkTask.rows.length === 0) {
          return send(res, 404, { error: "Nie znaleziono zadania." });
        }

        // GET /api/tasks/:id
        if (req.method === "GET") {
          return send(res, 200, checkTask.rows[0]);
        }

        // PUT /api/tasks/:id – Zmiana statusu lub danych w bazie
        if (req.method === "PUT") {
          if (data.status && !STATUSES.includes(data.status)) {
            return send(res, 400, { error: "Niepoprawny status." });
          }
          const query = `
            UPDATE tasks 
            SET status = COALESCE($1, status), 
                title = COALESCE($2, title), 
                description = COALESCE($3, description) 
            WHERE id = $4 
            RETURNING id, title, description, status;
          `;
          const values = [data.status, data.title, data.description, id];
          const result = await pool.query(query, values);
          return send(res, 200, result.rows[0]);
        }

        // DELETE /api/tasks/:id – Usunięcie zadania z bazy
        if (req.method === "DELETE") {
          await pool.query("DELETE FROM tasks WHERE id = $1", [id]);
          return send(res, 204);
        }
      }

      send(res, 405, { error: "Metoda niedozwolona" });
    } catch (err) {
      console.error(err);
      send(res, 500, { error: "Błąd serwera bazy danych" });
    }
  });
}).listen(5000, () => console.log("API zintegrowane z PostgreSQL: http://localhost:5000/api/tasks"));