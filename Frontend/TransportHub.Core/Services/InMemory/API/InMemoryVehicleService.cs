
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

internal class InMemoryVehicleService : IVehicleService
{
    public Task<Result<bool, Exception>> Add(VehicleUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            StaticVehicles.Add(new()
            {
                Id = $"ID_{dto.Name}",
                Name = dto.Name,
                Disabled = false,
                VehicleType = dto.VehicleType,
                Picture = dto.Picture,
                RequiredLicense = dto.RequiredLicense,
                RegistrationNumber = dto.RegistrationNumber,
                Vin = dto.Vin,
            });

            return true;
        });
    }

    public Task<Result<VehicleDto, Exception>> Get(string id)
    {
        return Task.Run<Result<VehicleDto, Exception>>(() =>
        {
            var user = StaticVehicles.Find(x => x.Id == id);
            if (user is null)
                return new InvalidOperationException("User not found");

            return user;
        });
    }

    public Task<Result<IEnumerable<VehicleDto>, Exception>> List()
    {
        return Task.FromResult((Result<IEnumerable<VehicleDto>, Exception>)StaticVehicles);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var user = StaticVehicles.Find(x => x.Id == id);

            if (user is not null)
            {
                StaticVehicles.Remove(user);
            }

            return user is not null;
        });
    }

    public Task<Result<bool, Exception>> Update(string id, VehicleUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {

            var item = StaticVehicles.Find(x => x.Id == id);
            if (item is null)
            {
                return new InvalidOperationException("User not found");
            }

            item.Name = dto.Name;
            item.VehicleType = dto.VehicleType;
            item.Picture = dto.Picture;
            item.RequiredLicense = dto.RequiredLicense;
            item.RegistrationNumber = dto.RegistrationNumber;
            item.Vin = dto.Vin;
            item.Disabled = dto.Disabled;

            return true;
        });
    }

    internal static readonly List<VehicleDto> StaticVehicles = new()
    {
        CreateVehicleDto(),
        CreateVehicleDto(),
        CreateVehicleDto(),
        CreateVehicleDto(),
        CreateVehicleDto()
    };
   
    private static Random _random = new();
    private static VehicleDto CreateVehicleDto()
    {
        _random ??= new();
        var name = LoremNET.Lorem.Words(1, true);
        var hasAlternativeAge = _random.NextDouble() <= 0.3;
        var disabled = _random.NextDouble() <= 0.2;

        var cond = hasAlternativeAge ? Lorem.Sentence(5, 9) : null;

        var licenses = InMemoryLicenseTypeService.StaticLicenses;
        var license = licenses[_random.Next(0, licenses.Count)];

        var regNum = Enumerable
                .Repeat(' ', 10)
                !.Select(_ => Lorem.Letter());
        var vin = Enumerable
                .Repeat(' ', 40)
                !.Select(_ => Lorem.Letter());

        return new()
        {
            Id = $"ID_{name}",
            Name = name,
            VehicleType = _random.Next(0,5),
            Picture = "",
            RequiredLicense = license.Id!,
            RegistrationNumber = string.Join("", regNum).ToUpperInvariant(),
            Vin = string.Join(separator: "", vin).ToUpperInvariant(),
            Disabled = disabled,
        };
    }


}
