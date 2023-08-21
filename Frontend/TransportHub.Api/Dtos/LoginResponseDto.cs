using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Api.Dtos;

public class LoginResponseDto
{
    public required string User { get; set; }
    public required string Key { get; set; }
}
