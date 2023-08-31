using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface IDriverService
{
    Task<Result<IEnumerable<DriverDto>, Exception>> List();
    Task<Result<DriverDto, Exception>> Get(string id);
    Task<Result<bool, Exception>> Add(DriverUpdateDto userDto);
    Task<Result<bool, Exception>> Remove(string id);
    Task<Result<bool, Exception>> Update(string id, DriverUpdateDto userDto);
}
