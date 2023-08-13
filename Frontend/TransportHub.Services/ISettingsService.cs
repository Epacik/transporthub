using TransportHub.Common;

namespace TransportHub.Services;

public interface ISettingsService
{
    string? Read(Settings setting);
    bool ReadBool(Settings settings);
    T? Read<T>(Settings setting) where T : ISpanParsable<T>;
    string? Read(Settings path, string defaultValue);
    void Write(Settings path, string? value);
    bool Read(Settings path, bool defaultValue);
    void Write(Settings path, bool value);
    int Read(Settings path, int defaultValue);
    void Write(Settings path, int value);
}
