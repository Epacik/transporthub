using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services.Impl.Empty;

internal class EmptyUserProvidedImageService : IUserProvidedImageService
{
    public Task<Result<string?, Exception>> GetImage()
    {
        return Task.FromResult<Result<string?, Exception>>(null as string);
    }
}
