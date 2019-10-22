using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Controllers
{
    public class EmployeeTerritoryController : Controller
    {
        private readonly DataContext _context;

        public EmployeeTerritoryController(DataContext context)
        {
            _context = context;
        }

        // GET: EmployeeTerritory
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.EmployeeTerritories.Include(e => e.Employee).Include(e => e.Territory);
            return View(await dataContext.ToListAsync());
        }

        // GET: EmployeeTerritory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeTerritory = await _context.EmployeeTerritories
                .Include(e => e.Employee)
                .Include(e => e.Territory)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employeeTerritory == null)
            {
                return NotFound();
            }

            return View(employeeTerritory);
        }

        // GET: EmployeeTerritory/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            ViewData["TerritoryId"] = new SelectList(_context.Territories, "TerritoryId", "TerritoryId");
            return View();
        }

        // POST: EmployeeTerritory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,TerritoryId")] EmployeeTerritory employeeTerritory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeTerritory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employeeTerritory.EmployeeId);
            ViewData["TerritoryId"] = new SelectList(_context.Territories, "TerritoryId", "TerritoryId", employeeTerritory.TerritoryId);
            return View(employeeTerritory);
        }

        // GET: EmployeeTerritory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeTerritory = await _context.EmployeeTerritories.FindAsync(id);
            if (employeeTerritory == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employeeTerritory.EmployeeId);
            ViewData["TerritoryId"] = new SelectList(_context.Territories, "TerritoryId", "TerritoryId", employeeTerritory.TerritoryId);
            return View(employeeTerritory);
        }

        // POST: EmployeeTerritory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,TerritoryId")] EmployeeTerritory employeeTerritory)
        {
            if (id != employeeTerritory.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeTerritory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeTerritoryExists(employeeTerritory.EmployeeId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", employeeTerritory.EmployeeId);
            ViewData["TerritoryId"] = new SelectList(_context.Territories, "TerritoryId", "TerritoryId", employeeTerritory.TerritoryId);
            return View(employeeTerritory);
        }

        // GET: EmployeeTerritory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeTerritory = await _context.EmployeeTerritories
                .Include(e => e.Employee)
                .Include(e => e.Territory)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employeeTerritory == null)
            {
                return NotFound();
            }

            return View(employeeTerritory);
        }

        // POST: EmployeeTerritory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeTerritory = await _context.EmployeeTerritories.FindAsync(id);
            _context.EmployeeTerritories.Remove(employeeTerritory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeTerritoryExists(int id)
        {
            return _context.EmployeeTerritories.Any(e => e.EmployeeId == id);
        }
    }
}
