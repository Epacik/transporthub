using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class DriversLicenseDto
{
    public string Id {get;set;}
    public string Driver {get;set;}
    public string License {get;set;}
    public bool Disabled { get; set; }
}

public class DriversLicenseUpdateDto
{
    public string Driver { get; set; }
    public string License { get; set; }
    public bool Disabled { get; set; }
}
