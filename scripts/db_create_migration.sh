#!/bin/bash

if [ -z "$1" ];
then
    echo
    echo "usage: $0 <new migration name>"
    echo
    echo "example: db_create_migration.sh AddIndexesForEveryone"
    echo
    exit 1
fi

dotnet clean -v q
dotnet build -v q
dotnet ef migrations add $1 -p ElectionStatistics.Model -s ElectionStatistics.WebSite
