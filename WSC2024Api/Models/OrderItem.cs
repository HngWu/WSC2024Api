using System;
using System.Collections.Generic;

namespace WSC2024Api.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Order Transaction { get; set; } = null!;
}
