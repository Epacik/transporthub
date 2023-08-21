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

$content = Get-Content "$PSScriptRoot/../Assets/Icons/tabler/tabler-icons.css";

$pattern = '(?:\.ti-)([A-Za-z0-9\-]+)(?:\:before\s+\{\s+content:\s+"\\)([0-9a-f]+)(?:";\s+})';
$options = [RegexOptions]::Multiline;

$matches =  [Regex]::Matches($content, $pattern, $options);

$lines = "";
$constants = "";

foreach ($match in $matches)
{
    $key = $match.Groups[1].Value;
    $value = $match.Groups[2].Value;

    $lines += "        { `"$key`", `"\u$value`" },`n";

    $first = $key[0];
    if ($first -ge [char]"0" -and $first -le [char]"9") {
        $key = "T" + $key;
    }

    $key = ConvertTo-PascalCase $key

    $constants += "    public const string $key = `"\u$value`";`n"
}

$out = @"
// THIS FILE IS AUTOGENERATER
// CHANGES WITHIN WILL BE LOST

using System.Collections.Generic;

namespace TransportHub.Core.Assets.Icons;

internal class Tabler
{
    public static Dictionary<string, string> IconMap = new()
    {
$lines
    };

$constants
}
"@

Out-File -FilePath "$PSScriptRoot/TablerIcons.g.cs" -InputObject $out
