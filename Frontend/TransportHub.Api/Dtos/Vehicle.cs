using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class VehicleDto
{
    public string Id {get;set;}
    public string Name {get;set;}
    public int VehicleType {get;set;}
    public string Picture {get;set;}
    public string RequiredLicense {get;set;}
    public string RegistrationNumber {get;set;}
    public string Vin {get;set;}
    public bool Disabled { get; set; }
}

public class VehicleUpdateDto
{
    public string Name { get; set; }
    public int VehicleType { get; set; }
    public string Picture { get; set; }
    public string RequiredLicense { get; set; }
    public string RegistrationNumber { get; set; }
    public string Vin { get; set; }
    public bool Disabled { get; set; }
}
