#!/bin/bash

dotnet clean -v q
dotnet build -v q
dotnet ef database update -p ElectionStatistics.Model -s ElectionStatistics.WebSite
