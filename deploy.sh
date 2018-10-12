#!/bin/bash

echo -n "Sure (y/n)? "
read answer

if [ "$answer" == "y" ]; then
    echo "🔥🔥🔥"
    az webapp deployment source sync --name laboratoryWeb --resource-group electoral-graphics
fi
