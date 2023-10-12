using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventory_Managment.Models;

namespace Inventory_Managment.Controllers
{
    public class ItemMastersController : Controller
    {
        private readonly InventoryManagementContext _context;

        public ItemMastersController(InventoryManagementContext context)
        {
            _context = context;
        }

        // GET: ItemMasters
        public async Task<IActionResult> Index()
        {
              return _context.ItemMasters != null ? 
                          View(await _context.ItemMasters.ToListAsync()) :
                          Problem("Entity set 'InventoryManagementContext.ItemMasters'  is null.");
        }

        // GET: ItemMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemMasters == null)
            {
                return NotFound();
            }

            var itemMaster = await _context.ItemMasters
                .FirstOrDefaultAsync(m => m.ItemCode == id);
            if (itemMaster == null)
            {
                return NotFound();
            }

            return View(itemMaster);
        }

        // GET: ItemMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemCode,ItemName,Uom,Mrp")] ItemMaster itemMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemMaster);
        }

        // GET: ItemMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemMasters == null)
            {
                return NotFound();
            }

            var itemMaster = await _context.ItemMasters.FindAsync(id);
            if (itemMaster == null)
            {
                return NotFound();
            }
            return View(itemMaster);
        }

        // POST: ItemMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemCode,ItemName,Uom,Mrp")] ItemMaster itemMaster)
        {
            if (id != itemMaster.ItemCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemMasterExists(itemMaster.ItemCode))
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
            return View(itemMaster);
        }

        // GET: ItemMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemMasters == null)
            {
                return NotFound();
            }

            var itemMaster = await _context.ItemMasters
                .FirstOrDefaultAsync(m => m.ItemCode == id);
            if (itemMaster == null)
            {
                return NotFound();
            }

            return View(itemMaster);
        }

        // POST: ItemMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemMasters == null)
            {
                return Problem("Entity set 'InventoryManagementContext.ItemMasters'  is null.");
            }
            var itemMaster = await _context.ItemMasters.FindAsync(id);
            if (itemMaster != null)
            {
                _context.ItemMasters.Remove(itemMaster);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemMasterExists(int id)
        {
          return (_context.ItemMasters?.Any(e => e.ItemCode == id)).GetValueOrDefault();
        }
    }
}
