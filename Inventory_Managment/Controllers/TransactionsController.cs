using Humanizer;
using Inventory_Managment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Inventory_Managment.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly InventoryManagementContext _context;

        public TransactionsController(InventoryManagementContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var inventoryManagementContext = _context.Transactions.Include(t => t.ItemCodeNavigation);
            return View(await inventoryManagementContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.ItemCodeNavigation)
                .FirstOrDefaultAsync(m => m.TransactionNo == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionNo,TransactionType,TransactionDate,ItemCode,Quantity,EntryDate")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode", transaction.ItemCode);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode", transaction.ItemCode);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionNo,TransactionType,TransactionDate,ItemCode,Quantity,EntryDate")] Transaction transaction)
        {
            if (id != transaction.TransactionNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode", transaction.ItemCode);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.ItemCodeNavigation)
                .FirstOrDefaultAsync(m => m.TransactionNo == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'InventoryManagementContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return (_context.Transactions?.Any(e => e.TransactionNo == id)).GetValueOrDefault();
        }

        [HttpGet]
        public IActionResult TransactionReport()
        {
            ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode");

            //SqlParameter itemCodeParam = new SqlParameter("@ItemCode", itemCode);
            //SqlParameter startDateParam = new SqlParameter("@StartDate", startDate);
            //SqlParameter endDateParam = new SqlParameter("@EndDate", endDate);

            //// Call the stored procedure and map the results to the Transaction entity
            //var results = _context.Transactions.FromSqlRaw("EXEC GetTransactionReport @ItemCode, @StartDate, @EndDate",
            //    itemCodeParam, startDateParam, endDateParam).ToList();

            return View();
        }




        [HttpPost]
        public IActionResult TransactionReport(Transaction transaction)
        {
            TransactionReportViewModel viewModel;
            if (ModelState.IsValid)
            {
                viewModel = new TransactionReportViewModel
                {
                    Transaction = transaction,
                };

                // Retrieve data from your stored procedure and store it in a DataTable
                viewModel.ItemReportsDataTable = GetItemReportsFromStoredProcedure(transaction);

                if (viewModel.ItemReportsDataTable != null && viewModel.ItemReportsDataTable.Rows.Count > 0)
                {
                    // Store the view model with data in HttpContext.Items
                    HttpContext.Items["TransactionReportViewModel"] = viewModel;
                }
                else
                {
                    // Handle the case where there's no data
                    TempData["ErrorMessage"] = "No data available.";
                }

                ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode");

                return View("TransactionReport", viewModel);
            }
            else
            {
                viewModel = new TransactionReportViewModel();
                return View("TransactionReport", viewModel);
            }
        }



        // Helper method to retrieve data from the stored procedure and return it as a DataTable
        private DataTable GetItemReportsFromStoredProcedure(Transaction transaction)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("GetItemLedgerReport", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ItemCode", transaction.ItemCode));
                    cmd.Parameters.Add(new SqlParameter("@FromDate", transaction.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", transaction.ToDate));

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }


        [HttpPost]
        public IActionResult ExportExcel(Transaction transaction)
        {
            // Check if the transaction object is null
            if (transaction == null)
            {
                // Handle the case where the transaction is null
                TempData["ErrorMessage"] = "Transaction data is missing.";
                return RedirectToAction("TransactionReport");
            }

            // Retrieve data from your stored procedure and store it in a DataTable
            var dataTable = GetItemReportsFromStoredProcedure(transaction);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                try
                {
                    // Create an Excel package using EPPlus
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("TransactionReport");

                        // Add headers to the worksheet with formatting
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            var headerCell = worksheet.Cells[1, i + 1];
                            headerCell.Value = dataTable.Columns[i].ColumnName;

                            // Format header cells
                            headerCell.Style.Font.Color.SetColor(Color.White); // Change font color
                            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid; // Set background color
                            headerCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#007BFF")); // Header background color
                            headerCell.Style.Font.Bold = true; // Make header text bold

                            // Add borders to header cells
                            headerCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            headerCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            headerCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            headerCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        // Populate data into the worksheet
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                // Check if the column contains a date and format it as needed
                                if (dataTable.Columns[col].DataType == typeof(DateTime))
                                {
                                    worksheet.Cells[row + 2, col + 1].Value = ((DateTime)dataTable.Rows[row][col]).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                                }

                                // Add borders to data cells
                                worksheet.Cells[row + 2, col + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[row + 2, col + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[row + 2, col + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[row + 2, col + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                        }

                        // Save the Excel package to a MemoryStream
                        var stream = new MemoryStream(package.GetAsByteArray());

                        // Set the content type and return the Excel file to the client for download
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransactionReport.xlsx");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during Excel generation
                    TempData["ErrorMessage"] = "An error occurred during Excel export: " + ex.Message;
                    return RedirectToAction("TransactionReport");
                }
            }
            else
            {
                // Handle the case where there's no data to export
                TempData["ErrorMessage"] = "No data available for export.";
                return RedirectToAction("TransactionReport");
            }
        }



        ////public IActionResult TransactionReport(Transaction transaction)
        ////{
        ////    TransactionReportViewModel viewModel;
        ////    if (ModelState.IsValid)
        ////    {
        ////        viewModel = new TransactionReportViewModel
        ////        {
        ////            Transaction = transaction,
        ////            ItemReports = _context.ItemReports.FromSqlRaw("EXEC GetItemLedgerReport @ItemCode, @FromDate, @ToDate",
        ////    new SqlParameter("@ItemCode", transaction.ItemCode),
        ////    new SqlParameter("@FromDate", transaction.FromDate),
        ////    new SqlParameter("@ToDate", transaction.ToDate))
        ////    .ToList()
        ////        };

        ////        ViewData["ItemCode"] = new SelectList(_context.ItemMasters, "ItemCode", "ItemCode");

        ////        return View("TransactionReport", viewModel);
        ////    }
        ////    else
        ////    {
        ////        viewModel = new TransactionReportViewModel();
        ////        return View("TransactionReport", viewModel);

        ////    }

        ////}





    }
}
