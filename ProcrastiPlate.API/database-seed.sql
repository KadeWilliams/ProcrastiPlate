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
    , UnitDescription varchar(100) not null
);

-- Public/Main Tables

CREATE TABLE public.AppUser (
    UserId SERIAL PRIMARY KEY
    , FirstName varchar(100) not null
    , LastName varchar(100) not null
    , Username varchar(50) not null unique
    , Email varchar(255) not null unique
    , GoogleId varchar(255)
    , AppleId varchar(255)
    , PasswordHash bytea 
    , PasswordSalt bytea
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.Ingredient (
    IngredientId SERIAL Primary Key
    , IngredientName varchar(200) not null
    , IsExotic BOOLEAN DEFAULT FALSE
    , IsPerishable BOOLEAN DEFAULT FALSE
    , IngredientImage bytea 
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.Recipe (
    RecipeId SERIAL PRIMARY KEY
    , UserId INT NOt NULL REFERENCES AppUser(UserId)  
    , RecipeName varchar(300) not null
    , RecipeDescription text
    , PrepTimeMinutes int
    , CookTimeMinutes int
    , Servings int
    , RecipeImage bytea 
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

CREATE TABLE public.RecipeStep (
    RecipeStepId SERIAL PRIMARY KEY
    , RecipeId INT not null REFERENCES Recipe(RecipeId)
    , StepNumber int not null 
    , Instruction text not null
    , DurationMinutes int
    , Temperature int 
    , TemperatureUnit varchar(1)
    , StepImage bytea
);

CREATE TABLE public.RecipeIngredient (
    RecipeId int not null references Recipe(RecipeId)
    , IngredientId int not null references Ingredient(IngredientId)
    , UnitTypeCd varchar(20) not null references reference.UnitType(UnitTypeCd)
    , Quantity decimal(10, 3) not null
    , Notes varchar(500) -- e.g. "finely chopped", "to taste"
    , DisplayOrder int DEFAULT 0
    , PRIMARY KEY (RecipeId, IngredientId, UnitTypeCd)
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

-- Indexes for Performance 
CREATE INDEX idx_RecipeUserId on Recipe(UserId);
CREATE INDEX idx_RecipeName on Recipe(RecipeName);
CREATE INDEX idx_IngredientName on Ingredient(IngredientName);
CREATE INDEX idx_RecipeIngredientRecipe on RecipeIngredient(RecipeId);
CREATE INDEX idx_RecipeIngredientIngredient on RecipeIngredient(IngredientId);

-- Seed Data for Quick Testing 

-- Unit Types 
INSERT INTO reference.UnitType (UnitTypeCd, UnitDescription)
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
('Test', 'User', 'testuser', 'test@example.com', decode('0000000000000000000000000000000000000000000000000000000000000000', 'hex'), decode('0000000000000000000000000000000000000000000000000000000000000000', 'hex'));

-- Sample Ingredients
INSERT INTO Ingredient (IngredientName, IsExotic, IsPerishable)
VALUES
('Spaghetti', FALSE, FALSE),
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
INSERT INTO Recipe (UserId, RecipeName, RecipeDescription, PrepTimeMinutes, CookTimeMinutes, Servings) 
VALUES
(1, 'Simple Pasta Aglio e Olio', 
'A classic italian pasta dish with garlic and olive oil',
10, 15, 4);

INSERT INTO RecipeStep (RecipeId, StepNumber, Instruction, DurationMinutes, Temperature, TemperatureUnit, StepImage)
VALUES 
(1, 1, 'Boil pasta according to package directions', null, null, null, null),
(1, 2, 'Saute garlic in olive oil until fragrant', null, null, null, null),
(1, 3, 'Toss pasta with garlic oil', null, null, null, null),
(1, 4, 'Add red pepper flakes and parsley', null, null, null, null),
(1, 5, 'Serve with parmesan cheese', null, null, null, null);

INSERT INTO RecipeIngredient (RecipeId, IngredientId, UnitTypeCd, Quantity, Notes, DisplayOrder)
VALUES
(1,1,'LB', 1, null, 1),
(1,6,'CUP', 0.5, null, 2),
(1,10,'CLOVE', 6.0, 'thinly sliced', 3),
(1,4,'TASTE', 6.0, 'thinly sliced', 3);
