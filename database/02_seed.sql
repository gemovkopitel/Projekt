INSERT INTO users (login, email, password, role, created_at) VALUES
('admin', 'admin@example.com', 'hash-placeholder-1', 'admin', NOW()),
('anna', 'anna@example.com', 'hash-placeholder-2', 'user', NOW()),
('piotr', 'piotr@example.com', 'hash-placeholder-3', 'user', NOW()),
('kasia', 'kasia@example.com', 'hash-placeholder-4', 'user', NOW()),
('marek', 'marek@example.com', 'hash-placeholder-5', 'user', NOW());

INSERT INTO tasks (title, description, status, users_id) VALUES
('Przygotować makietę', 'Ekran listy i formularza', 'done', 2),
('Utworzyć tabele', 'Users, Tasks, Comments', 'in_progress', 2),
('Napisać README', NULL, 'new', 3),
('Skonfigurować CI', 'Workflow dla frontendu', 'done', 1),
('Dodać filtrowanie', 'Po tytule i statusie', 'new', 4);

INSERT INTO comments (content, tasks_id, users_id) VALUES
('Makieta zaakceptowana', 1, 1),
('Brakuje klucza obcego w tasks', 2, 1),
('Poprawione, dzięki', 2, 2),
('Zacznę od sekcji uruchomienia', 3, 3),
('Świetna robota z CI', 4, 5);