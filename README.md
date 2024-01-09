Step 1: Download the zip file.

Step 2: Unzip file and Open "ATM Code.csproj" file in Visual Studio (I was using VS 2019)

Step 3: In Solution Explorer: Click on "Show All Files".

Step 4: Right click on "Bank.cs" file and click on "include in Project" (if it is not included).

Step 5: Create a database for this project. 

Step 6: Run below Query in MSSQL "New Query"/"Empty File" and run to create a Users table.

Script:
CREATE TABLE Users (
    AccountID INT IDENTITY(14576432,68),
	Pin INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
	Balance decimal,
    Age INT
);
(You can watch YT videos on how to create database in MSSQL and connect sql with c# code)

Step 7: Open "Bank.cs" File. ServerName and DatabaseName is empty, add your Server Name and Database Name in those variables.

Step 8: Press F5 to run the program.


