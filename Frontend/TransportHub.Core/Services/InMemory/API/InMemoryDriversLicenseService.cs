
using Avalonia;
using Lindronics.OneOf.Result;
using LoremNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api;
using TransportHub.Api.Dtos;
using TransportHub.Api.Impl;

namespace TransportHub.Core.Services.InMemory.API;

internal class InMemoryDriversLicenseService : IDriversLicenseService
{
    public Task<Result<bool, Exception>> Add(DriversLicenseUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            StaticDriver.Add(new()
            {
                Id = $"ID_{dto.Driver}_{dto.License}",
                Disabled = false,
                Driver = dto.Driver,
                License = dto.License,
            });

            return true;
        });
    }

    public Task<Result<DriversLicenseDto, Exception>> Get(string id)
    {
        return Task.Run<Result<DriversLicenseDto, Exception>>(() =>
        {
            var user = StaticDriver.Find(x => x.Id == id);
            if (user is null)
                return new InvalidOperationException("User not found");

            return user;
        });
    }

    public Task<Result<IEnumerable<DriversLicenseDto>, Exception>> List()
    {
        return Task.FromResult((Result<IEnumerable<DriversLicenseDto>, Exception>)StaticDriver);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var user = StaticDriver.Find(x => x.Id == id);

            if (user is not null)
            {
                StaticDriver.Remove(user);
            }

            return user is not null;
        });
    }

    internal static readonly List<DriversLicenseDto> StaticDriver = new()
    {
        CreateDriversLicenseDto(),
        CreateDriversLicenseDto(),
        CreateDriversLicenseDto(),
        CreateDriversLicenseDto(),
        CreateDriversLicenseDto()
    };
   
    private static Random _random = new();
    private static DriversLicenseDto CreateDriversLicenseDto()
    {
        _random ??= new();
        var name = Lorem.Words(1, true);
        var disabled = _random.NextDouble() <= 0.2;

        var drivers = InMemoryDriverService.StaticDrivers;
        var licenses = InMemoryLicenseTypeService.StaticLicenses;

        var driver = drivers[_random.Next(0, drivers.Count)];
        var license = licenses[_random.Next(0, licenses.Count)];

        return new()
        {
            Id = $"ID_{name}",
            Driver = driver.Id,
            License = license.Id,
            Disabled = disabled,
        };
    }
}
