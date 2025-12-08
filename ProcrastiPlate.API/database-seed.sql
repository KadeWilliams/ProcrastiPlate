-- ProcrastiPlate MVP Schema
-- Phase 1: Core Recipe Management

DROP SCHEMA IF EXISTS public CASCADE;
DROP SCHEMA IF EXISTS reference CASCADE;
CREATE SCHEMA public;
CREATE SCHEMA reference;
GRANT ALL ON SCHEMA public TO procrastiplateuser;
GRANT ALL ON SCHEMA public TO public;
GRANT ALL ON SCHEMA reference TO procrastiplateuser;
GRANT ALL ON SCHEMA reference TO public;

-- Reference Tables

CREATE TABLE reference.UnitType (
    UnitTypeCd varchar(20) Primary Key
    , Description varchar(100) not null
);

-- Public/Main Tables

CREATE TABLE public.AppUser (
    UserId SERIAL PRIMARY KEY
    , FirstName varchar(100) not null
    , LastName varchar(100) not null
    , Username varchar(50) not null unique
    , Email varchar(255) not null unique
    , PasswordHash bytea not null
    , PasswordSalt bytea not null
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.Ingredient (
    IngredientId SERIAL Primary Key
    , Name varchar(200) not null
    , IsExotic BOOLEAN DEFAULT FALSE
    , IsPerishable BOOLEAN DEFAULT FALSE
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.Recipe (
    RecipeId SERIAL PRIMARY KEY
    , UserId INT NOt NULL REFERENCES 
    , Name varchar(300) not null
    , Description text
    , PrepTimeMinutes int
    , CookTimeMinutes int
    , Servings int
    , Instructions text 
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.RecipeIngredient (
    RecipeId int not null references Recipe(RecipeId)
    , IngredientId int not null references Ingredient(IngredientId)
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

-- Indexes for Performance 
CREATE INDEX idx_RecipeUserId on Recipe(UserId)
CREATE INDEX idx_RecipeName on Recipe(name);
CREATE INDEX idx_IngredientName on Ingredient(name);
CREATE INDEX idx_RecipeIngredientRecipe on RecipeIngredient(RecipeId);
CREATE INDEX idx_RecipeIngredientIngredient on RecipeIngredient(IngredientId);

-- Seed Data for Quick Testing 

-- Unit Types 
INSERT INTO UnitType (UnitTypeCd, Description)
VALUES 
('CUP', 'Cup'),
('TBSP', 'Tablespoon'),
('TSP', 'Teaspoon'),
('OZ', 'Ounce'),
('LB', 'Pound'),
('KG', 'Kilogram'),
('G', 'Gram'),
('ML', 'Millileter'),
('L', 'Liter'),
('WHOLE', 'Whole'),
('PINCH', 'Pinch'),
('TASTE', 'To Taste'),
('CLOVE', 'Clove'),
('BUNCH', 'Bunch'),
('HEAD', 'Head');


-- Test user (password: "password123" - implement real hashing)
-- Just placeholder for now 
INSERT INTO AppUser (FirstName, LastName, Username, Email, PasswordHash, PasswordSalt) 
VALUES 
('Test', 'User', 'testuser', 'test@example.com', decode('0', 'hex'), decode('0', 'hex'));

-- Same Ingredients
INSERT INTO Ingredient (Name, IsExotic, IsPerishable)
VALUES
('All-Purpose Flour', FALSE, FALSE),
('Sugar', FALSE, FALSE),
('Salt', FALSE, FALSE),
('Black Pepper', FALSE, FALSE),
('Olive Oil', FALSE, FALSE),
('Butter', FALSE, TRUE),
('Eggs', FALSE, TRUE),
('Milk', FALSE, TRUE),
('Garlic', FALSE, TRUE),
('Onion', FALSE, TRUE),
('Chicken Breast', FALSE, TRUE),
('Tomatoes', FALSE, TRUE),
('Basil', FALSE, TRUE),
('Mozzarella Cheese', FALSE, TRUE),
('Parmesan Cheese', FALSE, TRUE);

-- Sample Recipe
INSERT INTO Recipe (UserId, Name, Description, PrepTimeMinutes, CookTimeMinutes, Servings, Instructions) 
VALUES
(1, 'Simple Pasta Aglio e Olio', 
'A classic italian pasta dish with garlic and olive oil',
10, 15, 4,
'1. Boil pasta according to package directions
2. Saute garlic in olive oil until fragrant
3. Toss pasta with garlic oil
4. Add red pepper flakes and parsley
5. Serve with parmesan cheese');
