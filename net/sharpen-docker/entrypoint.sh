#!/bin/sh
echo "Working dir: $1"
echo "Project dir: $2"
echo "Sharpen args: $3"
echo "Running sharpen..."
cd "$1" && java -jar /sharpen/sharpencore-0.0.1-SNAPSHOT.jar $2 $3