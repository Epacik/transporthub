using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services;

public interface IUserProvidedImageService
{
    Task<Result<string?, Exception>> GetImage();
}
