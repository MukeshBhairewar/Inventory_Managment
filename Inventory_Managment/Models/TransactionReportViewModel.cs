using System.Data;

namespace Inventory_Managment.Models
{
    public class TransactionReportViewModel
    {
       
            public Transaction? Transaction { get; set; }
            public List<ItemReport>? ItemReports { get; set; }

            public DataTable? ItemReportsDataTable { get; set; }

  

    }
}
