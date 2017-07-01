using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Security;

namespace WebApplication1.Controllers
{
    public class Account2Controller : ApiController
    {
        public class LoginViewModel
        {
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
        }

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login(LoginViewModel user)
        {
            if (!ModelState.IsValid)
            {
                //簡易版
                //return BadRequest();
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (user.username == "mark" && user.password == "123")
            {
                //簡易版 IHttpActionResult
                //FormsAuthentication.RedirectFromLoginPage(user.username, false);
                //return StatusCode(HttpStatusCode.Redirect);

                //複雜版
                // 將管理者登入的 Cookie 設定成 Session Cookie
                bool isPersistent = false;
                string userData = "admin,manager";

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                user.username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                isPersistent,
                userData,
                FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);

                // Create the cookie.
                var resp = new HttpResponseMessage();

                var cookie = new CookieHeaderValue(FormsAuthentication.FormsCookieName, encTicket);
                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";

                resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

                resp.StatusCode = HttpStatusCode.NoContent;

                return resp;
            }
            else
            {
                //簡易版
                //return BadRequest("登入失敗!");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "登入失敗");
            }
        }

        [Route("profile")]
        [HttpGet]
        public IHttpActionResult profile()
        {
            return Ok(User.Identity.Name);
        }

        [Route("logout")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
