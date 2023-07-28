using module ./auth.psm1


function Login() {
    [OutputType([LoginInfo])] param();

    ClearAndWrite-Host "Login: "
    $login = Read-Host;
    ClearAndWrite-Host "Password: "
    $pass = Read-Host;
    ClearAndWrite-Host "Disconnect other sessions?"
    $disconnect = (Show-Menu @("Yes", "No")) -eq "Yes";

    Write-Host "Login: $login; Password: $pass; Disconnect others: $disconnect";
    return LoginRequest -user $login -pass $pass -disconnectOtherSessions $disconnect
}

$items = @(
    "Login",
    "AAA",
    $(Get-MenuSeparator),
    "Quit"
);

$loggedIn = $false;
$loginInfo = $null;

do {
    ClearAndWrite-Host "Select entry to continue"
    if ($loggedIn) {
        Write-Host "Logged in as ${loginInfo.UserId} (Token: ${loginInfo.Token})";
    }
    $selection = Show-Menu $items

    if ($selection -eq $items[0]){
        $loginInfo = Login;
        $loggedIn = $loginInfo.IsSuccesful;
        Read-Host
    }

} until ($selection -eq [System.Linq.Enumerable]::Last($items));




# $response = Login "admin" "admin" $true

# Write-Host $response
