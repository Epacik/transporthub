
function SendRequest([string]$uri, [string]$method, [string]$body, [string]$token) {
    $requestInfo = @{
        Uri = "http://127.0.0.1:8080/api/v1/$uri"
        Method = $method
        Body = $body
        ContentType = "application/json"
    };

    if ([bool]$token) {
        $requestInfo["Headers"] = @{
            authorization = "Basic $token"
        }
    }

    return Invoke-WebRequest @requestInfo
}



function Encode-Base64([string]$content) {
    [OutputType([string])]
    $bytes = [System.Text.Encoding]::Unicode.GetBytes($content);
    return [System.Convert]::ToBase64String($bytes);
}

function Decode-Base64([string]$content) {
    [OutputType([string])]
    $bytes = [System.Convert]::FromBase64String($content);
    return [System.Text.Encoding]::Unicode.GetString($bytes);
}
