-- Таблиця користувачів
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "UserName" VARCHAR(100) NOT NULL,
    "PasswordHash" VARCHAR(255) NOT NULL
);

-- Таблиця фільмів
CREATE TABLE "Movies" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "FilePath" VARCHAR(500) NOT NULL,
    "Duration" INTEGER, 
    "ReleaseDate" DATE
);

-- Таблиця жанрів
CREATE TABLE "Genres" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL
);

-- Зв'язок Багато-до-Багатьох (Movies <-> Genres)
CREATE TABLE "MovieGenres" (
    "MovieId" INTEGER REFERENCES "Movies"("Id") ON DELETE CASCADE,
    "GenreId" INTEGER REFERENCES "Genres"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("MovieId", "GenreId")
);

-- Збережені фільми (Saved)
CREATE TABLE "SavedItems" (
    "UserId" INTEGER REFERENCES "Users"("Id") ON DELETE CASCADE,
    "MovieId" INTEGER REFERENCES "Movies"("Id") ON DELETE CASCADE,
    "AddedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ("UserId", "MovieId")
);

-- Історія переглядів (History)
CREATE TABLE "WatchHistory" (
    "UserId" INTEGER REFERENCES "Users"("Id") ON DELETE CASCADE,
    "MovieId" INTEGER REFERENCES "Movies"("Id") ON DELETE CASCADE,
    "LastWatched" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "TimeStop" INTEGER, 
    PRIMARY KEY ("UserId", "MovieId")
);