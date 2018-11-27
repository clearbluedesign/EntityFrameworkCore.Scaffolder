using System;
using System.Linq;
using System.Threading.Tasks;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Controllers {
	[Route("[controller]")]
	public class ProductsController : Controller {
		private readonly DataContext db;



		public ProductsController(DataContext db) {
			this.db = db;
		}



		[HttpGet]
		[Route("")]
		public async Task<IActionResult> Get() {
			var products = await this.db.Products
				.OrderBy(o => o.ProductName)
				.Take(10)
				.Select(product => new {
					Name = product.ProductName,
					Category = new {
						product.CategoryId,
						product.Category.CategoryName
					},
					Price = product.UnitPrice,
					Supplier = new {
						product.Supplier.SupplierId,
						product.Supplier.CompanyName,
					},
					Discontinued = product.Discontinued,
					Orders = product.OrderDetails.Take(5).Select(order => new {
						order.OrderId,
						order.Quantity,
						order.Discount
					})
				})
				.ToListAsync();

			return this.Ok(products);
		}



		[HttpGet]
		[Route("{id:int}")]
		public async Task<IActionResult> Get(Int32 id) {
			var product = await this.db.Products
				.FindAsync(id);

			if (product != null) {
				return this.Ok(new {
					Name = product.ProductName,
					Category = new {
						product.CategoryId,
						product.Category.CategoryName
					},
					Price = product.UnitPrice,
					Supplier = new {
						product.Supplier.SupplierId,
						product.Supplier.CompanyName,
					},
					Discontinued = product.Discontinued,
					Orders = product.OrderDetails.Take(5).Select(order => new {
						order.OrderId,
						order.Quantity,
						order.Discount
					})
				});
			}

			return this.NotFound();
		}
	}
}
