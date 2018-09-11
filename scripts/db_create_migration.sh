#!/bin/bash

dotnet ef migrations add $1 -p ElectionStatistics.Model -s ElectionStatistics.WebSite
