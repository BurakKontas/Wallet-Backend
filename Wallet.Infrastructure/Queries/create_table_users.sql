CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    phone VARCHAR(255) UNIQUE NOT NULL,
    username VARCHAR(255),
    hashPassword VARCHAR(255) NOT NULL,
    walletId INTEGER REFERENCES wallets(id)
);