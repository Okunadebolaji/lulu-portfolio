#!/bin/bash
dotnet ef database update --project Lulu_Portfolio.Infrastructure -c AppDbContext
dotnet Lulu_Portfolio.API.dll