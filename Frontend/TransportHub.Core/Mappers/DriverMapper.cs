using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;
using TransportHub.Core.Models;

namespace TransportHub.Core.Mappers;

[Mapper(UseDeepCloning = true)]
public static partial class DriverMapper
{
    public static DriverModel ToDriverModel(this DriverDto dto)
    {
        var model = dto.ToDriverModelInternal();
        model.IsDirty = false;
        return model;
    }
    private static partial DriverModel ToDriverModelInternal(this DriverDto dto);

    public static IEnumerable<DriverModel> ToDriverModels(this IEnumerable<DriverDto> Drivers)
    {
        var models = Drivers.ToDriverModelsInternal();
        foreach (var Driver in models)
        {
            Driver.IsDirty = false;
        }
        return models;
    }
    private static partial IEnumerable<DriverModel> ToDriverModelsInternal(this IEnumerable<DriverDto> Drivers);
    public static DriverDto ToDriverDto(this DriverModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            Picture = model.Picture,
            Nationality = model.Nationality,
            BaseLocation = model.BaseLocation,
            Disabled = model.Disabled,
        };
    }


    public static DriverUpdateDto ToDriverAddDto(this DriverModel model)
    {
        return new()
        {
            Name = model.Name,
            Picture = model.Picture,
            Nationality = model.Nationality,
            BaseLocation = model.BaseLocation,
            Disabled = model.Disabled,
        };
    }
}
