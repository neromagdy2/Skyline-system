-- Fix NationalitiesId Foreign Key Constraint Issue
-- Run this script in SQL Server Management Studio

-- Step 1: Drop the foreign key constraint if it exists
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AspNetUsers_Nationalities_NationalitiesId')
BEGIN
    ALTER TABLE AspNetUsers DROP CONSTRAINT FK_AspNetUsers_Nationalities_NationalitiesId;
    PRINT 'Foreign key constraint dropped successfully';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint does not exist';
END

-- Step 2: Fix invalid NationalitiesId values (set them to NULL)
UPDATE AspNetUsers
SET NationalitiesId = NULL
WHERE NationalitiesId IS NOT NULL 
  AND NationalitiesId NOT IN (SELECT Id FROM Nationalities WHERE Id IS NOT NULL);

PRINT 'Fixed invalid NationalitiesId values';

-- Step 3: Make NationalitiesId column nullable if it's not already
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('AspNetUsers') 
    AND name = 'NationalitiesId' 
    AND is_nullable = 1
)
BEGIN
    ALTER TABLE AspNetUsers ALTER COLUMN NationalitiesId INT NULL;
    PRINT 'Made NationalitiesId column nullable';
END
ELSE
BEGIN
    PRINT 'NationalitiesId column is already nullable';
END

-- Step 4: Recreate the foreign key constraint with proper settings
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AspNetUsers_Nationalities_NationalitiesId')
BEGIN
    ALTER TABLE AspNetUsers 
    ADD CONSTRAINT FK_AspNetUsers_Nationalities_NationalitiesId 
    FOREIGN KEY (NationalitiesId) REFERENCES Nationalities(Id) 
    ON DELETE SET NULL;
    
    PRINT 'Foreign key constraint recreated successfully';
END

-- Step 5: Verify the fix
SELECT 
    'Invalid Records Count' as Status,
    COUNT(*) as Count
FROM AspNetUsers u
WHERE u.NationalitiesId IS NOT NULL 
  AND u.NationalitiesId NOT IN (SELECT Id FROM Nationalities WHERE Id IS NOT NULL);

SELECT 
    'Null NationalitiesId Users' as Status,
    COUNT(*) as Count
FROM AspNetUsers 
WHERE NationalitiesId IS NULL;

SELECT 
    'Valid NationalitiesId Users' as Status,
    COUNT(*) as Count
FROM AspNetUsers u
WHERE u.NationalitiesId IS NOT NULL 
  AND u.NationalitiesId IN (SELECT Id FROM Nationalities WHERE Id IS NOT NULL);

PRINT 'Verification completed';
