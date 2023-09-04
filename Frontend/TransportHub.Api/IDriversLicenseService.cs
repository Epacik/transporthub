using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface IDriversLicenseService
{
    Task<Result<IEnumerable<DriversLicenseDto>, Exception>> List();
    //Task<Result<DriversLicenseDto, Exception>> Get(string id);
    Task<Result<bool, Exception>> Add(DriversLicenseUpdateDto userDto);
    Task<Result<bool, Exception>> Remove(string id);
    //Task<Result<bool, Exception>> Update(string id, DriversLicenseUpdateDto userDto);
}
