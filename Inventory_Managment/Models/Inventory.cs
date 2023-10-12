using System;
using System.Collections.Generic;

namespace Inventory_Managment.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? ItemCode { get; set; }

    public int? TransactionNo { get; set; }

    public string? TransactionType { get; set; }

    public int? Quantity { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual ItemMaster? ItemCodeNavigation { get; set; }

    public virtual Transaction? TransactionNoNavigation { get; set; }
}
