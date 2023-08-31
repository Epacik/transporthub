using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class DriversLicenseDto
{
    public string Id {get;set;}
    public int Driver {get;set;}
    public int License {get;set;}
    public bool Disabled { get; set; }
}

public class DriversLicenseUpdateDto
{
    public int Driver { get; set; }
    public int License { get; set; }
    public bool Disabled { get; set; }
}
