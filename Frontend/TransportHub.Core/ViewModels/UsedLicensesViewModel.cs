using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Core.Models;

namespace TransportHub.Core.ViewModels;

public partial class UsedLicensesViewModel : ObservableObject ,INavigationAware
{
    [ObservableProperty]
    private ObservableCollection<UsedLicenseModel> _licenses = new();

    [ObservableProperty]
    private int _alwaysNegativeOne;

    partial void OnAlwaysNegativeOneChanged(int value)
    {
        if (value != -1)
            AlwaysNegativeOne = -1;
    }

    public Task OnNavigatedFrom()
    {
        return Task.Run(Licenses.Clear);
    }

    public Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        return Task.Run(() =>
        {
            // that's probably a warcrime, but I don't care

            //var licenses = typeof(Resources.Licenses)
            //    .GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
            //    .Where(x => x.PropertyType == typeof(string))
            //    .Select(x => (name: x.Name, value: x.GetMethod!.Invoke(null, null) as string))
            //    .ToDictionary(x => x.name, x => x.value);

            //var names = typeof(Resources.LibraryNames)
            //    .GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
            //    .Where(x => x.PropertyType == typeof(string))
            //    .Select(x => (name: x.Name, value: x.GetMethod!.Invoke(null, null) as string))
            //    .ToDictionary(x => x.name, x => x.value);

            //foreach (var propName in licenses.Keys)
            //{
            //    var name = names.TryGetValue(propName, out var value) ? value : propName;
            //    var license = licenses[propName];

            //    if (name is not null && license is not null)
            //        Licenses.Add(new(name, license));
            //}

            var licenses = Resources.Licenses.ResourceManager
                .GetResourceSet(
                    CultureInfo.InvariantCulture,
                    true,
                    false);

            if (licenses is null)
                return;

            SortedDictionary<string, string> sortedLicenses = new();

            foreach (DictionaryEntry entry in licenses)
            {
                var name = entry.Key as string;
                var license = entry.Value as string;
                if (name is not null && license is not null)
                    sortedLicenses[name] = license;
            }

            foreach (var lic in sortedLicenses)
                Licenses.Add(new(lic.Key, lic.Value));
        });
    }
}
