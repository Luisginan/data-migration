-- create table script history on databasePostgres
-- field: id auto increment, script_name, script_content, script_status, script_time
-- script_status: 0: success, 1: failed 

CREATE TABLE IF NOT EXISTS script_history (
    id SERIAL PRIMARY KEY,
    order_number INT NOT NULL,
    script_name VARCHAR(255) NOT NULL,
    script_file_name VARCHAR(255) NOT NULL,
    script_version VARCHAR(255) NOT NULL,
    script_content TEXT NOT NULL,
    script_status INT NOT NULL,
    script_time TIMESTAMP NOT NULL
);
 