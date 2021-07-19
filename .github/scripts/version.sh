#!/bin/bash

regex='^(?<version>[0-9]+\.[0-9]+\.[0-9]+)(?=:)'
file='./ext/SDL/WhatsNew.txt'
grep -oP -m1 $regex $file