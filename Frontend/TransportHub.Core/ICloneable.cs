using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core;

public interface IClonable<T>
{
    T Clone();
}
