CREATE TABLE Results (
    ResultID int IDENTITY(1,1) PRIMARY KEY,
    SurveyID int NOT NULL,
    QuestionID int NOT NULL,
    AnswerID int NOT NULL
);