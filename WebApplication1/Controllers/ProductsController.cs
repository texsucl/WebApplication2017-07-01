﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[RoutePrefix("products")]
	public class ProductsController : ApiController
    {
        private FabricsEntities1 db = new FabricsEntities1();

		public ProductsController()
		{
			db.Configuration.LazyLoadingEnabled = false;
		}

		// GET: api/Products
		/// <summary>
		/// 搜尋所有商品
		/// </summary>
		/// <returns></returns>
		[Route("")]
		public IQueryable<Product> GetProduct()
        {
            return db.Product;
        }

		// GET: api/Products/5
		/// <summary>
		/// 搜尋特定商品 By Id
		/// </summary>
		/// <param name="id">ProductId</param>
		/// <returns></returns>
		[Route("{id}",Name = "GetProductById")]
		[ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
		[Route("{id:int}/orderlines")]
		[ResponseType(typeof(IQueryable<OrderLine>))]
		public IHttpActionResult GetProductOrderLines(int id)
		{
			var orderlines = db.OrderLine.Where(x => x.ProductId == id);

			return Ok(orderlines);
		}

		// PUT: api/Products/5
		[Route("{id}")]
		[ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

		// POST: api/Products
		[Route("")]
		[ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, product);
        }

		// DELETE: api/Products/5
		[Route("{id}")]
		[ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductId == id) > 0;
        }
    }
}