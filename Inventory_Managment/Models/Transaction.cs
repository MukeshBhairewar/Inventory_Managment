using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Managment.Models;

public partial class Transaction
{
    public int TransactionNo { get; set; }

    public string? TransactionType { get; set; }

    public DateTime? TransactionDate { get; set; }

    public int? ItemCode { get; set; }

    public int? Quantity { get; set; }

    public DateTime? EntryDate { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ItemMaster? ItemCodeNavigation { get; set; }

    [NotMapped]
    public DateTime FromDate { get; set; }

    [NotMapped]
    public DateTime ToDate { get; set; }

}
