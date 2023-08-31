using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransportHub.Core.ViewModels;

public partial class OrdersViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private ObservableCollection<OrderModel> _orders = new();

    [ObservableProperty]
    private OrderModel? _selectedOrder;

    public OrdersViewModel()
    {
        Func<int, int> r = (i) => _random.Next(i);
        Func<int, int, int> r1 = (i, n) => _random.Next(i, n);
        var count = _random.Next(30);


        for(int i = 0; i < count; i++)
        {
            var name = _companyNames[r(_companyNames.Length)];
            var employee = _employeeNames[r(_employeeNames.Length)];


            Orders.Add(new(
                LoremNET.Lorem.Words(r1(3, 6)),
                LoremNET.Lorem.Paragraph(r1(20,50), r1(1, 4)),
                name,
                LoremNET.Lorem.Words(r1(1, 3)),
                LoremNET.Lorem.Words(r1(1, 3)),
                LoremNET.Lorem.DateTime(new DateTime(2021, 1, 1), new DateTime(2025, 5, 6)),
                employee,
                r(100) < 50));
        }
    }


    static string[] _employeeNames = {
        "Tanya",
        "Miriam (Mitzi)",
        "Julie",
        "Leona (Loni)",
        "Mabel",
        "Emelia",
        "Sofia (Saffi)",
        "Haleema",
        "Francis",
        "Elin",
    };

    static string[] _companyNames = {
        "Faketime",
        "Neu Fake",
        "Swift Fake",
        "I'm Not Real",
        "Urban Fakes",
        "Pegasus Fake",
        "Makin",
        "Upper Fakes",
        "Be Fake",
        "Nomi Fake",
        "Cartoon Fake",
        "Lost Love",
        "Mister Faker",
        "Quake Fake",
        "Storyfake",
        "Pro Fakes",
        "Fake Wrap",
        "Bürz",
        "IMFake",
        "Urban Mimic",
        "Barbie Bomb",
        "Madefake",
        "Diamond Fake",
        "Spoof",
        "Lotus Fakes",
        "Quintet Fake",
        "Videosfake",
        "Rip Fake",
        "Doppelgangers",
        "Natural Fake",
        "Polyfake",
        "Online Made",
        "Marvellous",
        "Smart Phony",
        "Illusion Games",
        "Between",
        "Crazy Cover",
        "Get Real",
        "Fake Claims",
        "Epic Falsify",
        "Societyscene",
        "Elsie Faux",
        "Ahmed",
        "Everfake",
        "Red Lies",
        "Absolut Fakes",
        "Hey Faker",
        "See Lie",
        "Ellie's Fakes",
        "Koyun",
        "Antifake",
        "Mine Fakes",
        "Core Lyfe",
        "Many Fakes",
        "Varu",
        "Snazzy Fakes",
        "Unity Fakes",
        "Zoom Fakery",
        "The Fakeable",
        "Digifakes",
        "Ryan Fakes",
        "Phoney",
        "Pipe",
        "Big Hype",
        "CONn",
        "Classic Fakes",
        "Theatre",
        "ILiketo Fake",
        "Faked",
        "Maniq",
        "Parody Park",
        "Life Fakery",
        "Yourfake",
        "Forever Fakes",
        "Parody",
        "Counterfeit Couture",
        "Great Faker",
        "Open Drops",
        "Siren Shade",
        "Miss Fake",
        "Lovely Scams",
        "Krz",
        "Data Fake",
        "Sports Brand",
        "Sorta Real",
        "Supreme Sim",
        "Your Moo",
        "Mechanic Made",
        "Marginal",
        "Cubism",
        "preferredplastic",
        "Pretty Plastic",
        "Spider Webs",
        "Crisp Fakery",
        "Blog Bug",
        "Believe Bay",
        "Not Real",
        "Act Fake",
        "Betrayal",
        "Famous",
        "Wizart",
        "Kan",
        "Believish",
        "Death Impersonator",
        "Crudely Fake",
        "Search Meme",
        "Devilish",
        "Ideal Made",
        "Deputy Doubt",
        "Instafake",
        "Hogi",
        "Hey Look",
        "Whisper",
        "Pixelfake",
        "Megaphone",
        "Mode King",
        "Genuin",
        "featured",
        "Lab Fake",
        "Kikis",
        "Fest Pur",
        "Vidfy",
        "Afrifaze",
        "Affairs",
        "IDFlake",
        "Ez Fake",
        "Fae Kai",
        "True Shine",
        "Iconic D",
        "Poser",
    };
}
