Import-Module "$psscriptRoot/auth.psm1"


$response = Login "admin" "admin" $true

Write-Host $response
