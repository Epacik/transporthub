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

internal class InMemoryLicenseTypeService : ILicenseTypeService
{

    public Task<Result<bool, Exception>> Add(LicenseTypeUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            StaticLicenses.Add(new()
            {
                Id = $"ID_{dto.Name}",
                Name = dto.Name,
                Disabled = false,
                Description = dto.Description,
                MinimalAgeOfHolder = dto.MinimalAgeOfHolder,
                AlternativeMinimalAgeOfHolder = dto.AlternativeMinimalAgeOfHolder,
                ConditionForAlternativeMinimalAge = dto.ConditionForAlternativeMinimalAge,
            });

            return true;
        });
    }

    public Task<Result<LicenseTypeDto, Exception>> Get(string id)
    {
        return Task.Run<Result<LicenseTypeDto, Exception>>(() =>
        {
            var user = StaticLicenses.Find(x => x.Id == id);
            if (user is null)
                return new InvalidOperationException("User not found");

            return user;
        });
    }

    public Task<Result<IEnumerable<LicenseTypeDto>, Exception>> List()
    {
        return Task.FromResult((Result<IEnumerable<LicenseTypeDto>, Exception>)StaticLicenses);
    }

    public Task<Result<bool, Exception>> Remove(string id)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {
            var user = StaticLicenses.Find(x => x.Id == id);

            if (user is not null)
            {
                StaticLicenses.Remove(user);
            }

            return user is not null;
        });
    }

    public Task<Result<bool, Exception>> Update(string id, LicenseTypeUpdateDto dto)
    {
        return Task.Run<Result<bool, Exception>>(() =>
        {

            var item = StaticLicenses.Find(x => x.Id == id);
            if (item is null)
            {
                return new InvalidOperationException("User not found");
            }

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.MinimalAgeOfHolder = dto.MinimalAgeOfHolder;
            item.AlternativeMinimalAgeOfHolder = dto.AlternativeMinimalAgeOfHolder;
            item.ConditionForAlternativeMinimalAge = dto.ConditionForAlternativeMinimalAge;
            item.Disabled = dto.Disabled;

            return true;
        });
    }

    internal static readonly List<LicenseTypeDto> StaticLicenses = new()
    {
        CreateLicenseDto(),
        CreateLicenseDto(),
        CreateLicenseDto(),
        CreateLicenseDto(),
        CreateLicenseDto()
    };


    private static Random _random = new();
    private static LicenseTypeDto CreateLicenseDto()
    {
        _random ??= new();
        var name = LoremNET.Lorem.Words(1, true);
        var hasAlternativeAge = _random.NextDouble() <= 0.3;
        var disabled = _random.NextDouble() <= 0.2;

        var cond = hasAlternativeAge ? Lorem.Sentence(5, 9) : null;

        return new()
        {
            Id = $"ID_{name}",
            Name = name,
            Description = Lorem.Paragraph(13,60, 1,5),
            MinimalAgeOfHolder = _random.Next(18,24),
            AlternativeMinimalAgeOfHolder = hasAlternativeAge ? _random.Next(16,20) : null,
            ConditionForAlternativeMinimalAge = cond,
            Disabled = disabled,
        };
    }
}
