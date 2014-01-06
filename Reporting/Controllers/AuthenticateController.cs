using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reporting.Models;
using Newtonsoft.Json;

namespace Reporting.Controllers {
    public class AuthenticateController : Controller {

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index() {

            ViewBag.Error = TempData["error"];

            if (Convert.ToInt32(Session["custID"]) != 0)
            {
                return Redirect("/Reports");
            }

            return View();
        }

        
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult xAuth(string key) 
        {
            try
            {
                if (key.Length > 0)
                {
                    Customer cust = new Customer();
                    string loginResponse = cust.loginAPIAuth(key);
                    Session["ICustomer"] = loginResponse;
                    //ICustomer is already bringing all customer class information, which includes the cust_ID
                    ICustomer customer = JsonConvert.DeserializeObject<ICustomer>(loginResponse);
                    Session["custID"] = customer.Id;
                    
                    
                    HttpCookie cookie = new HttpCookie("custID");
                    cookie.Value = customer.Id.ToString(CultureInfo.InvariantCulture);
                    cookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(cookie);

                    HttpCookie cookie2 = new HttpCookie("ICustomer");
                    cookie2.Value = loginResponse;
                    cookie2.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(cookie2);

                }
                else
                {
                    return Redirect("/auth");
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Failed to log you into reporting services. " + e.Message;
                Response.Redirect("/auth");
            }
            return Redirect("/Reports");
        }


        [ActionName("Index")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login()
        {
            try {
                    string email = Request.Form["email"];
                    string password = Request.Form["password"];

                    Customer cust = new Customer();
                    string loginResponse = cust.loginAPIAuth(email, password); 
                    ICustomer customer = JsonConvert.DeserializeObject<ICustomer>(loginResponse);

                    //if there is not customer login result
                    if (!cust.Login(email, password))
                    {
                        throw new Exception("Invalid credentials");
                    }

                    //create session variables
                    Session["custID"] = customer.Id;
                    Session["ICustomer"] = loginResponse;

                    if (Request.Form["remember"] != null && Request.Form["remember"] == "1")
                    {
                        var cookie = new HttpCookie("custID")
                        {
                            Value = customer.Id.ToString(),
                            Expires = DateTime.Now.AddDays(30)
                        };

                        Response.Cookies.Add(cookie);

                        HttpCookie cookie2 = new HttpCookie("ICustomer");
                        cookie2.Value = loginResponse;
                        cookie2.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookie2);
                    }

                    return Redirect("/Reports");
                    }
            catch (Exception e)
            {
                TempData["error"] = "Failed to log you into reporting services. " + e.Message;
                return Redirect("/auth");
            }
            
        }

        public void Logout() {
            Session.Clear();
            HttpCookie cookie = Response.Cookies.Get("custID");
            cookie.Expires = DateTime.Now.AddYears(-1);
            HttpCookie cookie2 = Response.Cookies.Get("ICustomer");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Redirect("/auth");
        }

    }
}
