using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;

namespace TransportHub.Api;

public interface ILicenseTypeService
{
    Task<Result<IEnumerable<LicenseTypeDto>, Exception>> List();
    Task<Result<LicenseTypeDto, Exception>> Get(string id);
    Task<Result<bool, Exception>> Add(LicenseTypeUpdateDto userDto);
    Task<Result<bool, Exception>> Remove(string id);
    Task<Result<bool, Exception>> Update(string id, LicenseTypeUpdateDto userDto);
}
