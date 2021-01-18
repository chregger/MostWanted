CREATE TABLE Secrets (
    SecretID int PRIMARY KEY,
    SecretType nvarchar(256) NOT NULL,
    Password nvarchar(256) NOT NULL
);