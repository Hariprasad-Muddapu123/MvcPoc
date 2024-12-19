---- Step 1: Update NULL values in the column with a default value (e.g., 0 or a specific admin ID)
--UPDATE Bikes
--SET ReviewedByAdmin = null;
---- Step 2: Alter the column to make it NOT NULL
--ALTER TABLE Bikes
--ALTER COLUMN ReviewedByAdmin VARCHAR(max) NULL;
delete from notifications;
