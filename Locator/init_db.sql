CREATE DATABASE users_db;
CREATE DATABASE ratings_db;
CREATE DATABASE vacancies_db;

\c users_db
\i /docker-entrypoint-initdb.d/db_dumps/users_db.sql

\c ratings_db
\i /docker-entrypoint-initdb.d/db_dumps/ratings_db.sql

\c vacancies_db
\i /docker-entrypoint-initdb.d/db_dumps/vacancies_db.sql
