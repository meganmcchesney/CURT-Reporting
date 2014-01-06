using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reporting.Models;
using Newtonsoft.Json;

namespace Reporting.Controllers {
    public class BaseController : Controller
    {
        async protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try
            {
                var custId = Convert.ToInt32(Session["custID"]);
                if (custId == 0)
                {
                    var cookie = Request.Cookies.Get("custID");
                    if (cookie != null) custId = Convert.ToInt32(cookie.Value);
                    if (custId == 0)
                    {
                        Response.Redirect("/auth");
                    }
                }
                string loginResponse = "";
                if (Session["ICustomer"] != null) {
                    loginResponse = Session["ICustomer"].ToString();
                } else if (Request.Cookies.Get("ICustomer") != null) {
                    loginResponse = Request.Cookies.Get("ICustomer").Value;
                } else {
                    throw new Exception("Your session has timed out. Please log back in.");
                }
                // assign loginResponse to an object
                ICustomer customer = JsonConvert.DeserializeObject<ICustomer>(loginResponse);

                ICustomer.ICustomerUser ActualUser = null;
                foreach (var user in customer.Users) 
                {
                    if (user.Current == true) 
                    {
                        ActualUser = user;
                    }
                }

                // if customerUserID = 12345601 and isSudo = true, then isAdmin is true.
                bool isAdmin = false;

                if (customer.Id == 1 || customer.Id == 12345601) {
                    if (ActualUser.Sudo == true) {
                        isAdmin = true;
                    }
                }

                // pass IsAdmin bool back to view through viewbag

                ViewBag.isAdmin = isAdmin;
                ViewBag.Customer = await new Customer { customerID = custId }.Get();
            }
            catch (Exception e)
            {
                Response.Redirect("/auth");
           }
        }
    }

 }
