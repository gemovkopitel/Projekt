CREATE ROLE userek login password 'P@ssW0rD';
GRANT CONNECT ON DATABASE "taskmanagerDB" to userek;
GRANT USAGE ON SCHEMA public to userek;
GRANT SELECT on tasks, users, comment to userek;
 
SET ROLE userek;
select current_user; --powinno dzialac
select count(*) from tasks; -powinno dzialac
select count(*) from comment; --powinno dzialac
Delete from comment; -- permission denied
RESET ROLE;