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
    public class CustomerDemographicController : Controller
    {
        private readonly DataContext _context;

        public CustomerDemographicController(DataContext context)
        {
            _context = context;
        }

        // GET: CustomerDemographic
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomerDemographics.ToListAsync());
        }

        // GET: CustomerDemographic/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics
                .FirstOrDefaultAsync(m => m.CustomerTypeId == id);
            if (customerDemographic == null)
            {
                return NotFound();
            }

            return View(customerDemographic);
        }

        // GET: CustomerDemographic/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerDemographic/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerTypeId,CustomerDesc")] CustomerDemographic customerDemographic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerDemographic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerDemographic);
        }

        // GET: CustomerDemographic/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics.FindAsync(id);
            if (customerDemographic == null)
            {
                return NotFound();
            }
            return View(customerDemographic);
        }

        // POST: CustomerDemographic/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerTypeId,CustomerDesc")] CustomerDemographic customerDemographic)
        {
            if (id != customerDemographic.CustomerTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerDemographic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerDemographicExists(customerDemographic.CustomerTypeId))
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
            return View(customerDemographic);
        }

        // GET: CustomerDemographic/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDemographic = await _context.CustomerDemographics
                .FirstOrDefaultAsync(m => m.CustomerTypeId == id);
            if (customerDemographic == null)
            {
                return NotFound();
            }

            return View(customerDemographic);
        }

        // POST: CustomerDemographic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customerDemographic = await _context.CustomerDemographics.FindAsync(id);
            _context.CustomerDemographics.Remove(customerDemographic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerDemographicExists(string id)
        {
            return _context.CustomerDemographics.Any(e => e.CustomerTypeId == id);
        }
    }
}
