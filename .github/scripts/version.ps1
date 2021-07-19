#!/usr/bin/env pwsh

$regex='^(?<version>[0-9]+\.[0-9]+\.[0-9]+)(?=:)'
$file='.\ext\SDL\WhatsNew.txt'
select-string -Path $file -Pattern $regex -AllMatches | Select-Object -ExpandProperty Matches -First 1 | % { $_.Value }