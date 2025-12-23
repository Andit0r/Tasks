using System;
using System.Collections.Generic;

namespace ShoesMarketDbLibrary.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
