/*SELECT login, email FROM users 
ORDER BY login ASC;

SELECT title, status FROM tasks 
WHERE status = 'done';

SELECT status, COUNT(*) AS liczba_zadan FROM tasks 
GROUP BY status 
ORDER BY liczba_zadan DESC;

SELECT tasks.title, users.login FROM tasks
JOIN users ON users.id = tasks.users_id
