CREATE ROLE userek login password 'P@ssW0rD';
GRANT CONNECT ON DATABASE task_manager to userek;
GRANT USAGE ON SCHEMA public to userek;
GRANT SELECT on tasks, users, comments to userek;
 
SET ROLE userek;
select current_user; --powinno dzialac
select count(*) from tasks; --powinno dzialac
select count(*) from comments; --powinno dzialac
Delete from comments; -- permission denied
RESET ROLE;
