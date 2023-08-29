using Avalonia.Input.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Services;

namespace TransportHub.Core.Services.Impl;

internal class ReportErrorService : IReportErrorService
{
    private readonly IDialogService _dialogService;
    private readonly IClipboardProvider _clipboardProvider;

    public ReportErrorService(
        IDialogService dialogService,
        IClipboardProvider clipboardProvider)
    {
        _dialogService = dialogService;
        _clipboardProvider = clipboardProvider;
    }
    public async Task ShowError(Exception error)
    {
        var res = await _dialogService.ShowConfirmation(
            "Błąd",
            $"""
            Wystąpił błąd.
            Kliknij "Yes" aby skopiować dane o błędzie.

            {error}
            """);

        if (res)
        {
            await (_clipboardProvider.Get()?.SetTextAsync(error.ToString()) ?? Task.CompletedTask);
        }
    }
}
