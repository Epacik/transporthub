using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public class UsedLicenseModel
{
    public UsedLicenseModel(string libraryName, string license)
    {
        LibraryName = libraryName;
        License = license;
    }

    public string LibraryName { get; set; }
    public string License { get; set; }
}
