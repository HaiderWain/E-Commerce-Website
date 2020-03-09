using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amazonistan.Models;


namespace Amazonistan.Controllers
{
    public class HomeController : Controller
    {
        public static String loggedInEmail;
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult authenticate(String email, String password)
        {
            int result = CRUD.Login(email, password);

            if (result == -1)
            {
                String data = "Something went wrong while connecting with the database.";
                return View("Login", (object)data);
            }
            else if (result == 0)
            {

                String data = "Incorrect Credentials";
                return View("Login", (object)data);
            }

            if (email == "admin@gmail.com")
            {
                loggedInEmail = email;
                return RedirectToAction("homePageAdmin");
            }

            loggedInEmail = email;
            return RedirectToAction("homePageUser");

        }
        public ActionResult authenticateForSignup(String name, String email, String password, String number, String address)
        {
            int result = CRUD.Signup(name, email, password, number, address);

            if (result == -1)
            {
                String data = "Something went wrong while connecting with the database.";
                return View("Signup", (object)data);
            }
            else if (result == 0)
            {

                String data = "Incorrect Credentials";
                return View("Signup", (object)data);
            }

            return RedirectToAction("Login");

        }

        public ActionResult homePage()
        {
            loggedInEmail = null;
            return View(CRUD.popularProducts());
        }
        public ActionResult homePageUser()
        {
            ViewBag.currentUser = loggedInEmail;
            return View(CRUD.popularProducts());
        }
        public ActionResult homePageAdmin()
        {
            ViewBag.currentUser = loggedInEmail;
            return View(CRUD.popularProducts());
        }
        public ActionResult searchedProducts(String productName)
        {
            return View(CRUD.Search(productName));
        }
        public ActionResult AccountSettings()
        {
            String email = loggedInEmail;
            return View(CRUD.viewAccountDetails(email));
        }
        public ActionResult changePassword()
        {
            return View();
        }
        public ActionResult authenticateForChangePassword(String email, String password, String newPassword)
        {
            string email2 = loggedInEmail;

            if (email2 == email)
            {
                int result = CRUD.changePassword(email, password, newPassword);

                if (result == -1)
                {
                    String data = "Something went wrong while connecting with the database.";
                    return View("Login", (object)data);
                }

                else if (result == 0)
                {
                    String data = "Incorrect Combination of Email/Password";
                    return View("AccountSettings", (object)data);
                }
                return RedirectToAction("Login");
            }
            else
                return RedirectToAction("homePageUser");

        }

        public ActionResult itemBought(int productID)
        {
            string email = loggedInEmail;

            if (email != null)
            {
                if (CRUD.itemBuying(email, productID) == 1)
                {
                    return View();
                }
                else
                {
                    return itemNotBought();
                }
            }
            else
            {
                return null;
            }

        }

        public ActionResult itemNotBought()
        {

            return View();
        }

        public ActionResult Reviews(int productID)
        {
            return View(CRUD.productReviews(productID));
        }

        public ActionResult insertItem()
        {
            return View();
        }
        public ActionResult authenticateForItemInsert(String productCategory, String productName, String productPrice, String productAmount)
        {
            int result = CRUD.itemInsert(productCategory, productName, productPrice, productAmount);

            if (result == 1)
            {
                return View("itemAdded");
            }
            else
                return View("itemNotAdded");
        }

        public ActionResult deleteItem()
        {
            return View();
        }

        public ActionResult authenticateFordeleteItem(String productName)
        {
            int result = CRUD.itemDelete(productName);

            if (result == 1)
            {
                return View("itemDeleted");
            }
            else
                return View("itemNotDeleted");
        }



    }
}
