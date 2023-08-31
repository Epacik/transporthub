using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class DriverDto
{
    public string Id {get;set;}
    public string Name {get;set;}
    public string Picture {get;set;}
    public string Nationality { get;set;}
    public string BaseLocation {get;set;}
    public bool Disabled { get; set; }
}

public class DriverUpdateDto
{
    public string Name { get; set; }
    public string Picture { get; set; }
    public string Nationality { get; set; }
    public string BaseLocation { get; set; }
    public bool Disabled { get; set; }
}
