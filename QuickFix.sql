-- Simple Fix for NationalitiesId Issue
-- Run this script first to fix the data

-- Find and fix invalid records
UPDATE AspNetUsers
SET NationalitiesId = NULL
WHERE NationalitiesId IS NOT NULL 
  AND NationalitiesId NOT IN (SELECT Id FROM Nationalities);

-- Check results
SELECT 
    CASE 
        WHEN NationalitiesId IS NULL THEN 'Fixed (NULL)'
        WHEN NationalitiesId IN (SELECT Id FROM Nationalities) THEN 'Valid'
        ELSE 'Still Invalid'
    END as Status,
    COUNT(*) as Count
FROM AspNetUsers
GROUP BY 
    CASE 
        WHEN NationalitiesId IS NULL THEN 'Fixed (NULL)'
        WHEN NationalitiesId IN (SELECT Id FROM Nationalities) THEN 'Valid'
        ELSE 'Still Invalid'
    END;
