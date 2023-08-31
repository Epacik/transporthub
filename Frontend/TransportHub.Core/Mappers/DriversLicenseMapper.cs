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
public static partial class DriversLicenseMapper
{
    public static DriversLicenseModel ToDriversLicenseModel(this DriversLicenseDto dto)
    {
        var model = dto.ToDriversLicenseModelInternal();
        model.IsDirty = false;
        return model;
    }
    private static partial DriversLicenseModel ToDriversLicenseModelInternal(this DriversLicenseDto dto);

    public static IEnumerable<DriversLicenseModel> ToDriversLicenseModels(this IEnumerable<DriversLicenseDto> DriversLicenses)
    {
        var models = DriversLicenses.ToDriversLicenseModelsInternal();
        foreach (var DriversLicense in models)
        {
            DriversLicense.IsDirty = false;
        }
        return models;
    }
    private static partial IEnumerable<DriversLicenseModel> ToDriversLicenseModelsInternal(this IEnumerable<DriversLicenseDto> DriversLicenses);
    public static DriversLicenseDto ToDriversLicenseDto(this DriversLicenseModel model)
    {
        return new()
        {
            Id = model.Id,
            Driver = model.Driver,
            License = model.License,
            Disabled = model.Disabled,
        };
    }


    public static DriversLicenseUpdateDto ToDriversLicenseAddDto(this DriversLicenseModel model)
    {
        return new()
        {
            Driver = model.Driver,
            License = model.License,
            Disabled = model.Disabled,
        };
    }
}
