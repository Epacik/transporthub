#Import-Module "$PSScriptRoot/functions.psm1"
using module ./functions.psm1


class LoginInfo {
    [bool]$IsSuccesful
    [int]$ReturnCode
    [string]$UserId
    [string]$Token

    LoginInfo([bool]$IsSuccesful, [int]$ReturnCode, [string]$UserId, [string]$Token) {
        $this.IsSuccesful = $IsSuccesful
        $this.ReturnCode = $ReturnCode
        $this.UserId = $UserId
        $this.Token = $Token
    }
}

function LoginRequest() {
    [OutputType([LoginInfo])]
    param([string]$user, [string]$pass, [bool]$disconnectOtherSessions)

    $disconnect = "false";
    if ($disconnectOtherSessions) {
        $disconnect = "true"
    }

    $body = @"
{
    "user": "$user",
    "password": "$pass",
    "disconnectOtherSessions": $disconnect
}
"@;

    Write-host "Body: $body"
    $response = SendRequest "/auth/login" "Post" $body;
    $isSuccessful = $response.StatusCode -eq 200;
    $body = ConvertFrom-Json $response.Content

    return [LoginInfo]::new($isSuccessful, $response.StatusCode, $body.user, $body.key);
}


