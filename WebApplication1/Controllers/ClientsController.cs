using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.ActionFilters;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "admin")]
    public class ClientsController : ApiController
    {
        private FabricsEntities1 db = new FabricsEntities1();

		public ClientsController()
		{
			db.Configuration.LazyLoadingEnabled = false;
		}

		// GET: api/Clients
		public IQueryable<Client> GetClient()
        {
            return db.Client;
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
		[Route("clients/{id:int}",Name = "GetClientById")]
		public HttpResponseMessage GetClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return Request.CreateResponse(HttpStatusCode.OK, client);
        }

		[Route("clients/{id:int}/orders")]
		//[ResponseType(typeof(Client))]
		public IHttpActionResult GetClientByClientId(int id)
		{
			var orders = db.Order.Where(x => x.ClientId == id);

            if (orders == null || !orders.Any())
			{
				return RedirectToRoute("GetClientById", new { id = id });
			}

			return Ok(orders);
		}

		[Route("clients/{id:int}/orders/{oid:int}")]
		public IHttpActionResult GetClientByClientIdOrOrderId(int id,int oid)
		{
			var orders = db.Order.Where(x => x.ClientId == id && x.OrderId == oid);

			return Ok(orders);
		}

		[Route("clients/{id:int}/orders/{status:alpha}")]
		public IHttpActionResult GetClientByClientIdOrOrderStatus(int id, string status)
		{
			var orders = db.Order.Where(x => x.ClientId == id && x.OrderStatus == status);

			return Ok(orders);
		}

		[Route("clients/{id:int}/orders/{*dateTime:datetime}")]
		public IHttpActionResult GetClientByClientIdOrOrderDate(int id, DateTime dateTime)
		{
			var orders = db.Order.Where(x => x.ClientId == id && x.OrderDate > dateTime);

			return Ok(orders);
		}

		// PUT: api/Clients/5
		[ResponseType(typeof(void))]
        public IHttpActionResult PutClient(int id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.ClientId)
            {
                return BadRequest();
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // POST: api/Clients
        [ValidateModel]
        [ResponseType(typeof(Client))]
        [Route("clients")]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Client.Add(client);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = client.ClientId }, client);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            db.Client.Remove(client);
            db.SaveChanges();

            return Ok(client);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Client.Count(e => e.ClientId == id) > 0;
        }
    }
}