﻿CREATE TABLE IF NOT EXISTS   ProblemCalsses
(
    Id SERIAL PRIMARY KEY,
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Name VARCHAR(50) NOT NULL,
  
)