using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Core.Models;

namespace TransportHub.Core.DesignHelpers;

public static class DesignModels
{
    public static UserModel User = new()
    {
        Id = "DefaultId",
        Name = "Design User",
        Picture = null,
        PasswordExpirationDate = DateTime.Now,
        UserType = Api.UserType.Admin,
        MultiLogin = true,
        Disabled = false,
    };
}
