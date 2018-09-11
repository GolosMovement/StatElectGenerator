#!/bin/bash

dotnet ef database update -p ElectionStatistics.Model -s ElectionStatistics.WebSite
