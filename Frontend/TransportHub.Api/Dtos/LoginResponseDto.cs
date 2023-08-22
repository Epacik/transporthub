using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class LoginResponseDto
{
    public string? User { get; set; }
    public string? Key { get; set; }
}
