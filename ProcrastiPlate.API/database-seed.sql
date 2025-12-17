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
    , UnitTypeDescription varchar(100) not null
);

CREATE TABLE reference.Category (
    Category varchar(20) PRIMARY KEY
    , CategoryDescription varchar(100) not null
);

CREATE TABLE reference.Ingredient (
    IngredientId SERIAL primary key
    , IngredientName varchar(200) not null
    , Category varchar(20) REFERENCES reference.Category(Category)
    , IsExotic BOOLEAN DEFAULT FALSE
    , IsPerishable BOOLEAN DEFAULT FALSE
    , CreateDttm timestamp default CURRENT_TIMESTAMP
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

CREATE TABLE public.UserIngredient (
    UserIngredientId SERIAL Primary Key
    , UserId int not null REFERENCES AppUser(UserId)
    , IngredientName varchar(200) not null
    , Category varchar(20) REFERENCES reference.Category(Category)
    , IsExotic BOOLEAN DEFAULT FALSE
    , IsPerishable BOOLEAN DEFAULT FALSE
    , IngredientImage bytea 
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
    , UNIQUE(UserId, IngredientName) -- User can't create duplicates
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
    , IngredientId int references reference.Ingredient(IngredientId)
    , UserIngredientId int references UserIngredient(UserIngredientId)
    , UnitTypeCd varchar(20) not null references reference.UnitType(UnitTypeCd)
    , Quantity decimal(10, 3) not null
    , Notes varchar(500) -- e.g. "finely chopped", "to taste"
    , DisplayOrder int DEFAULT 0
    , PRIMARY KEY (RecipeId, IngredientId, UnitTypeCd)
    , CHECK (
        (IngredientId IS NOT NULL AND UserIngredientId IS NULL) OR
        (IngredientId IS NULL AND UserIngredientId IS NOT NULL) 
    ) -- Must reference one or the other not both 
    , CreateDttm timestamp default CURRENT_TIMESTAMP
    , UpdateDttm timestamp default CURRENT_TIMESTAMP
);

-- Indexes for Performance 
CREATE INDEX idx_RecipeUserId on Recipe(UserId);
CREATE INDEX idx_RecipeName on Recipe(RecipeName);
CREATE INDEX idx_IngredientName on reference.Ingredient(IngredientName);
CREATE INDEX idx_UserIngredientName on UserIngredient(IngredientName);
CREATE INDEX idx_RecipeIngredientRecipe on RecipeIngredient(RecipeId);
CREATE INDEX idx_RecipeIngredientIngredient on RecipeIngredient(IngredientId);

-- Seed Data for Quick Testing 

-- Unit Types 
INSERT INTO reference.UnitType (UnitTypeCd, UnitTypeDescription)
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
INSERT INTO reference.Ingredient (IngredientName, IsExotic, IsPerishable)
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

INSERT INTO public.UserIngredient (UserId, IngredientName)
VALUES
(1, 'Random Ingredient');

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

INSERT INTO RecipeIngredient (RecipeId, IngredientId, UserIngredientId, UnitTypeCd, Quantity, Notes, DisplayOrder)
VALUES
(1,1, null, 'LB', 1, null, 1),
(1,6, null, 'CUP', 0.5, null, 2),
(1,10, null, 'CLOVE', 6.0, 'thinly sliced', 3),
(1,4, null, 'TASTE', 6.0, 'thinly sliced', 3);
