using System;
using System.Collections.Generic;

namespace Inventory_Managment.Models;

public partial class ItemAuditTrail
{
    public int AuditTrailId { get; set; }

    public int ItemCode { get; set; }

    public string ItemName { get; set; } = null!;

    public string Uom { get; set; } = null!;

    public decimal Mrp { get; set; }

    public string Action { get; set; } = null!;

    public DateTime? Timestamp { get; set; }
}
