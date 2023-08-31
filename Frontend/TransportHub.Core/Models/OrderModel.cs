using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Models;

public partial class OrderModel : ObservableObject, IOrderModel
{

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _clientName;

    [ObservableProperty]
    private string _initialLocation;

    [ObservableProperty]
    private string _destination;

    [ObservableProperty]
    private DateTime _deadlineDate;

    [ObservableProperty]
    private string _modifiedBy;

    [ObservableProperty]
    private bool _fulfilled;

    public OrderModel(string name, string description, string clientName, string initialLocation, string destination, DateTime deadlineDate, string modifiedBy, bool fulfilled)
    {
        Name = name;
        Description = description;
        ClientName = clientName;
        InitialLocation = initialLocation;
        Destination = destination;
        DeadlineDate = deadlineDate;
        ModifiedBy = modifiedBy;
        Fulfilled = fulfilled;
    }
}

public interface IOrderModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClientName { get; set; }
    public string InitialLocation { get; set; }
    public string Destination { get; set; }
    public DateTime DeadlineDate { get; set; }
    public string ModifiedBy { get; set;}
    public bool Fulfilled { get; set; }
}
