using namespace System.Text.RegularExpressions;

function ConvertTo-PascalCase
{
    [OutputType('System.String')]
    param(
        [Parameter(Position=0)]
        [string] $Value
    )

    # https://devblogs.microsoft.com/oldnewthing/20190909-00/?p=102844
    return [regex]::replace($Value.ToLower(), '(^|[_-])(.)', { $args[0].Groups[2].Value.ToUpper()})
}

$source = "$PSScriptRoot/../Assets/Icons/tabler/tabler-icons.css";
$target = "$PSScriptRoot/TablerIcons.g.cs";
$sourceHash = (Get-FileHash $source).Hash

if (Test-Path -Path $target -PathType Leaf)
{
    $currentContent = Get-Content $target;
    if ($null -ne $currentContent -and $currentContent.Length -gt 9000) {
        $currentHash = ($currentContent[2] ?? "").Replace("// ", "");

        if ($currentHash -eq $sourceHash)
        {
            Write-Host "Source has not change, exiting"
            exit 0;
        }
    }
}


$content = Get-Content $source;

$pattern = '(?:\.ti-)([A-Za-z0-9\-]+)(?:\:before\s+\{\s+content:\s+"\\)([0-9a-f]+)(?:";\s+})';
$options = [RegexOptions]::Multiline;

$matches =  [Regex]::Matches($content, $pattern, $options);

$lines = "";
$constants = "";

foreach ($match in $matches)
{
    $key = $match.Groups[1].Value;
    $value = $match.Groups[2].Value;

    $key = ConvertTo-PascalCase $key

    $lines += "        { `"Icon$key`", `"\u$value`" },`n";

    $constants += "    public const string Icon$key = `"\u$value`";`n"
}

$out = @"
// THIS FILE IS AUTOGENERATER
// CHANGES WITHIN WILL BE LOST
// $sourceHash

using System.Collections.Generic;

namespace TransportHub.Core.Assets.Icons;

internal class Tabler
{
    public static Dictionary<string, string> IconsMap = new()
    {
$lines
    };

$constants
}
"@

Out-File -FilePath $target -InputObject $out
