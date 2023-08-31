using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class LicenseTypeDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int MinimalAgeOfHolder { get; set; }
    public int? AlternativeMinimalAgeOfHolder { get; set; }
    public string? ConditionForAlternativeMinimalAge { get; set; }
    public bool Disabled { get; set; }
}

public class LicenseTypeUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int MinimalAgeOfHolder { get; set; }
    public int? AlternativeMinimalAgeOfHolder { get; set; }
    public string? ConditionForAlternativeMinimalAge { get; set; }
    public bool Disabled { get; set; }
}
