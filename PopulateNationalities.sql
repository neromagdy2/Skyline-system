-- SQL Script to populate Nationalities table with common countries
-- Run this script to add nationalities to the database

-- Clear existing data (optional - remove if you want to start fresh)
DELETE FROM Nationalities;
DBCC CHECKIDENT ('Nationalities', RESEED, 1);

-- Insert common nationalities
INSERT INTO Nationalities (Name, Code) VALUES 
('Egypt', 'EG'),
('United States', 'US'),
('United Kingdom', 'UK'),
('France', 'FR'),
('Germany', 'DE'),
('Italy', 'IT'),
('Spain', 'ES'),
('Canada', 'CA'),
('Australia', 'AU'),
('Japan', 'JP'),
('China', 'CN'),
('India', 'IN'),
('Brazil', 'BR'),
('Mexico', 'MX'),
('Russia', 'RU'),
('Saudi Arabia', 'SA'),
('United Arab Emirates', 'AE'),
('Qatar', 'QA'),
('Kuwait', 'KW'),
('Jordan', 'JO'),
('Lebanon', 'LB'),
('Turkey', 'TR'),
('South Africa', 'ZA'),
('Nigeria', 'NG'),
('Kenya', 'KE'),
('Morocco', 'MA'),
('Algeria', 'DZ'),
('Tunisia', 'TN'),
('Libya', 'LY'),
('Sudan', 'SD'),
('Iraq', 'IQ'),
('Iran', 'IR'),
('Pakistan', 'PK'),
('Bangladesh', 'BD'),
('Indonesia', 'ID'),
('Malaysia', 'MY'),
('Singapore', 'SG'),
('Thailand', 'TH'),
('Philippines', 'PH'),
('Argentina', 'AR'),
('Chile', 'CL'),
('Colombia', 'CO'),
('Peru', 'PE'),
('Venezuela', 'VE'),
('Greece', 'GR'),
('Portugal', 'PT'),
('Netherlands', 'NL'),
('Belgium', 'BE'),
('Switzerland', 'CH'),
('Austria', 'AT'),
('Sweden', 'SE'),
('Norway', 'NO'),
('Denmark', 'DK'),
('Finland', 'FI'),
('Poland', 'PL'),
('Czech Republic', 'CZ'),
('Hungary', 'HU'),
('Romania', 'RO'),
('Bulgaria', 'BG'),
('Croatia', 'HR'),
('Serbia', 'RS'),
('Ukraine', 'UA'),
('Belarus', 'BY'),
('New Zealand', 'NZ'),
('Israel', 'IL');

-- Verify insertion
SELECT COUNT(*) as TotalNationalities FROM Nationalities;

-- Show first 10 records
SELECT TOP 10 Id, Name, Code FROM Nationalities ORDER BY Id;
