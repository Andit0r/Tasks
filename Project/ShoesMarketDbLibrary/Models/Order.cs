using System;
using System.Collections.Generic;

namespace ShoesMarketDbLibrary.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? StatusId { get; set; }

    public int UserId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int ReceiveCode { get; set; }

    public virtual ICollection<ShoeOrder> ShoeOrders { get; set; } = new List<ShoeOrder>();

    public virtual Status? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
