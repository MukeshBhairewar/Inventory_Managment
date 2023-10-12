using System;
using System.Collections.Generic;

namespace Inventory_Managment.Models;

public partial class ItemMaster
{
    public int ItemCode { get; set; }

    public string? ItemName { get; set; }

    public string? Uom { get; set; }

    public decimal? Mrp { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
