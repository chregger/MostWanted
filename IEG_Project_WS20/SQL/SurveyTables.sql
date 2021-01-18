CREATE TABLE Surveys (
    SurveyID int IDENTITY(1,1) PRIMARY KEY,
    SurveyName nvarchar(256) NOT NULL
);

CREATE TABLE Questions (
    QuestionID int IDENTITY(1,1) PRIMARY KEY,
    SurveyID int NOT NULL,
    Question nvarchar(256),
    FOREIGN KEY (SurveyID) REFERENCES Surveys(SurveyID) 
    
);

CREATE TABLE Answers (
    AnswerID int IDENTITY(1,1) PRIMARY KEY,
    QuestionID int NOT NULL,
    Answer nvarchar(256),
    FOREIGN KEY (QuestionID) REFERENCES Questions(QuestionID) 
);



