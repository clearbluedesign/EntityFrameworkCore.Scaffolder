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
    public class CustomerCustomerDemoController : Controller
    {
        private readonly DataContext _context;

        public CustomerCustomerDemoController(DataContext context)
        {
            _context = context;
        }

        // GET: CustomerCustomerDemo
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.CustomerCustomerDemoes.Include(c => c.Customer).Include(c => c.CustomerType);
            return View(await dataContext.ToListAsync());
        }

        // GET: CustomerCustomerDemo/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerCustomerDemo = await _context.CustomerCustomerDemoes
                .Include(c => c.Customer)
                .Include(c => c.CustomerType)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerCustomerDemo == null)
            {
                return NotFound();
            }

            return View(customerCustomerDemo);
        }

        // GET: CustomerCustomerDemo/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerDemographics, "CustomerTypeId", "CustomerTypeId");
            return View();
        }

        // POST: CustomerCustomerDemo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerTypeId")] CustomerCustomerDemo customerCustomerDemo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerCustomerDemo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", customerCustomerDemo.CustomerId);
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerDemographics, "CustomerTypeId", "CustomerTypeId", customerCustomerDemo.CustomerTypeId);
            return View(customerCustomerDemo);
        }

        // GET: CustomerCustomerDemo/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerCustomerDemo = await _context.CustomerCustomerDemoes.FindAsync(id);
            if (customerCustomerDemo == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", customerCustomerDemo.CustomerId);
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerDemographics, "CustomerTypeId", "CustomerTypeId", customerCustomerDemo.CustomerTypeId);
            return View(customerCustomerDemo);
        }

        // POST: CustomerCustomerDemo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,CustomerTypeId")] CustomerCustomerDemo customerCustomerDemo)
        {
            if (id != customerCustomerDemo.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerCustomerDemo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerCustomerDemoExists(customerCustomerDemo.CustomerId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", customerCustomerDemo.CustomerId);
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerDemographics, "CustomerTypeId", "CustomerTypeId", customerCustomerDemo.CustomerTypeId);
            return View(customerCustomerDemo);
        }

        // GET: CustomerCustomerDemo/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerCustomerDemo = await _context.CustomerCustomerDemoes
                .Include(c => c.Customer)
                .Include(c => c.CustomerType)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerCustomerDemo == null)
            {
                return NotFound();
            }

            return View(customerCustomerDemo);
        }

        // POST: CustomerCustomerDemo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customerCustomerDemo = await _context.CustomerCustomerDemoes.FindAsync(id);
            _context.CustomerCustomerDemoes.Remove(customerCustomerDemo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerCustomerDemoExists(string id)
        {
            return _context.CustomerCustomerDemoes.Any(e => e.CustomerId == id);
        }
    }
}
