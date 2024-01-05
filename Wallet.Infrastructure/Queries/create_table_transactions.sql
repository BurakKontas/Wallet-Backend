CREATE TABLE transactions (
    id SERIAL PRIMARY KEY,
    senderId INTEGER REFERENCES users(id),
    receiverId INTEGER REFERENCES users(id),
    amount NUMERIC(10, 2) NOT NULL
);