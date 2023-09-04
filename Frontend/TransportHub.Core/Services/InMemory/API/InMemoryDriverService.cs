
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

internal class InMemoryDriverService : IDriverService
{
    public Task<Result<bool, Exception>> Add(DriverUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            StaticDrivers.Add(new()
            {
                Id = $"ID_{dto.Name}",
                Name = dto.Name,
                Disabled = false,
                Picture = dto.Picture,
                BaseLocation = dto.BaseLocation,
                Nationality = dto.Nationality,
            });

            return true;
        });
    }

    public Task<Result<DriverDto, Exception>> Get(string id)
    {
        return Task.Run<Result<DriverDto, Exception>>(() =>
        {
            var user = StaticDrivers.Find(x => x.Id == id);
            if (user is null)
                return new InvalidOperationException("User not found");

            return user;
        });
    }

    public Task<Result<IEnumerable<DriverDto>, Exception>> List()
    {
        return Task.FromResult((Result<IEnumerable<DriverDto>, Exception>)StaticDrivers);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var user = StaticDrivers.Find(x => x.Id == id);

            if (user is not null)
            {
                StaticDrivers.Remove(user);
            }

            return user is not null;
        });
    }

    public Task<Result<bool, Exception>> Update(string id, DriverUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {

            var item = StaticDrivers.Find(x => x.Id == id);
            if (item is null)
            {
                return new InvalidOperationException("User not found");
            }

            item.Name = dto.Name;
            item.Picture = dto.Picture;
            item.Nationality = dto.Nationality;
            item.BaseLocation = dto.BaseLocation;
            item.Disabled = dto.Disabled;

            return true;
        });
    }

    internal static readonly List<DriverDto> StaticDrivers = new()
    {
        CreateDriverDto(),
        CreateDriverDto(),
        CreateDriverDto(),
        CreateDriverDto(),
        CreateDriverDto()
    };
   
    private static Random _random = new();
    private static DriverDto CreateDriverDto()
    {
        _random ??= new();
        var name = Lorem.Words(1, true);
        var disabled = _random.NextDouble() <= 0.2;

        return new()
        {
            Id = $"ID_{name}",
            Name = name,
            Picture = "",
            Nationality = Lorem.Words(1),
            BaseLocation = Lorem.Words(1),
            Disabled = disabled,
        };
    }
}
