#!/bin/bash

if [ -z "$1" ];
then
    echo
    echo "usage: $0 <migration name to rollback to>"
    echo
    echo "example: db_rollback_migration.sh AddOptimizationIndexForLineNumbers"
    echo
    exit 1
fi

dotnet clean -v q
dotnet build -v q
dotnet ef database update -p ElectionStatistics.Model -s ElectionStatistics.WebSite $1
