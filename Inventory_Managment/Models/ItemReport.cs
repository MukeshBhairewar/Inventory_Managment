using System.ComponentModel.DataAnnotations;

namespace Inventory_Managment.Models
{
    public class ItemReport
    {


        public string ?Item { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ?TransactionType { get; set; }
        public int ReceiptQuantity { get; set; }
        public int IssueQuantity { get; set; }
        public int ClosingQuantity { get; set; }
    }

   
}
