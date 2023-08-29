using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Converters;

public class Base64ToIconSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str || (!str.StartsWith("data:") && !str.Contains(";base64,")))
            return parameter;

        var splitted = str.Split(':', ';', ',');

        if (splitted.Length < 4)
            return parameter;

        try
        {
            var mimeType = splitted[1];
            var data = System.Convert.FromBase64String(splitted[3]);

            //Bitmap? image = mimeType switch
            //{
            //    "image/png" => new Bitmap(data),
            //    "image/jpeg" => null,
            //    "image/gif" => null,
            //    _ => null,
            //};

            var bitmap = new Bitmap(new MemoryStream(data));

            return new ImageIconSource() { Source = bitmap };
        }
        catch (Exception)
        {
            return parameter;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
