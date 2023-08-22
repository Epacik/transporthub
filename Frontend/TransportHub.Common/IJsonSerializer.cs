using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;


namespace TransportHub.Common;

public interface IJsonSerializer
{
    public Result<T?, Exception> Deserialize<T>(string json);
    public Result<string?, Exception> Serialize<T>(T? value);
}
