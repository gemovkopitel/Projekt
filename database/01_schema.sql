CREATE TABLE users (
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 	login VARCHAR(50) NOT NULL,
 	email VARCHAR(50) NOT NULL UNIQUE,
 	password VARCHAR(255) NOT NULL,
 	role VARCHAR(50) NOT NULL DEFAULT 'user' CHECK (role IN ('user', 'admin')),
 	created_at TIMESTAMP NOT NULL
);

CREATE TABLE tasks (
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 	title VARCHAR(200) NOT NULL CHECK (length(trim(title)) > 0),
 	description VARCHAR(200),
 	status VARCHAR(50) NOT NULL DEFAULT 'new' CHECK (status IN ('new', 'in_progress', 'done')),
 	created_at TIMESTAMP NOT NULL DEFAULT now(),
 	users_id INT NOT NULL
REFERENCES users (id)
);

CREATE TABLE comments (
 	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 	content VARCHAR(200) NOT NULL,
 	created_at TIMESTAMP NOT NULL DEFAULT now(),
 	tasks_id INT NOT NULL
REFERENCES tasks (id) ON DELETE CASCADE,
 	users_id INT NOT NULL
REFERENCES users (id)
);
