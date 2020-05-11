# Добавление таблицы при помощи скрипта
```
CREATE TABLE users
(
    Id SERIAL PRIMARY KEY,
    username CHARACTER VARYING(30),
    email CHARACTER VARYING(30),
    passwordHash CHARACTER VARYING(255),
    salt CHARACTER VARYING(30),
    accountNumber BIGINT,
    balance REAL
);

```
