CREATE TABLE Results (
    ResultID int IDENTITY(1,1) PRIMARY KEY,
    SurveyName nvarchar(256) NOT NULL,
    Question nvarchar(256) NOT NULL,
    Answer nvarchar(256) NOT NULL
);

