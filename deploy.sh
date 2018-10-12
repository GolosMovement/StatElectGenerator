#!/bin/bash

echo -n "Sure (y/n)? "
read answer

if [ "$answer" == "y" ]; then
    az webapp deployment source sync --name laboratoryWeb --resource-group electoral-graphics
    if [ $? -eq 0 ]; then
        echo "🔥🔥🔥"
    fi
fi
