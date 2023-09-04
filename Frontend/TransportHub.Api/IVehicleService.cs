using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface IVehicleService
{
    Task<Result<IEnumerable<VehicleDto>, Exception>> List();
    Task<Result<VehicleDto, Exception>> Get(string id);
    Task<Result<bool, Exception>> Add(VehicleUpdateDto dto);
    Task<Result<bool, Exception>> Remove(string id);
    Task<Result<bool, Exception>> Update(string id, VehicleUpdateDto dto);
}
