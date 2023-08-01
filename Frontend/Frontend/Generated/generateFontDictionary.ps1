using namespace System.Text.RegularExpressions;
$content = Get-Content "$PSScriptRoot/../Assets/Icons/tabler/tabler-icons.css";

$pattern = '(?:\.ti-)([A-Za-z0-9\-]+)(?:\:before\s+\{\s+content:\s+"\\)([0-9a-f]+)(?:";\s+})';
$options = [RegexOptions]::Multiline;

$matches =  [Regex]::Matches($content, $pattern, $options);

$lines = "";

foreach ($match in $matches)
{
    $key = $match.Groups[1].Value;
    $value = $match.Groups[2].Value;

    $lines += "        { `"$key`", `"\u$value`" },`n";
}

$out = @"
// THIS FILE IS AUTOGENERATER
// CHANGES WITHIN WILL BE LOST

using System.Collections.Generic;

namespace Frontend.Assets.Icons;

internal class Tabler
{
    public static Dictionary<string, string> IconMap = new()
    {
$lines
    };
}
"@

Out-File -FilePath "$PSScriptRoot/TablerIcons.g.cs" -InputObject $out
