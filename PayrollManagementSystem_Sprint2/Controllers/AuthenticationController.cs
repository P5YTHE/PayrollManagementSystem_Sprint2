using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollManagementSystem_Sprint2.Models;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PayrollManagementSystem_Sprint2.Controllers
{

    public class AuthenticationController : Controller
    {
        static string localHostLink = "https://localhost:44314/";   //Common localhost link variable
        private HttpContent content;       

        public async Task<AuthorizationResponse> IsValidEmployee(LoginDetails authResponse)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{localHostLink}api/");
                client.DefaultRequestHeaders.Clear();

                //Defining Request Data Format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string tmp = JsonConvert.SerializeObject(authResponse).ToString();
                var content = new StringContent(tmp, Encoding.UTF8, "application/json");
                HttpResponseMessage Result = await client.PostAsync($"EmployeeMasters/Login", content);
                if (Result.IsSuccessStatusCode)
                {
                    //storing the response details recieved from web api
                    var employeeResponse = Result.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web Api and storing into the mployee list
                    var resultValue = JsonConvert.DeserializeObject<AuthorizationResponse>(employeeResponse);
                    
                    HttpContext.Session.SetString("Token", resultValue._token);
                    return resultValue;
                }
                else
                {
                    return null;
                }
            }
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> Register(EmployeeMaster empObj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new IdentityUser() { Id =empObj.EmployeeId };
        //        var result = await UserManager.CreateAsync(user, empObj.EmployeePassword);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
        //    }
        //    return View(empObj);
        //}

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginDetails model)
        {
            if (ModelState.IsValid)
            {
                //validation of the Employee weather the employee is valid or Not
                 var isValidUser = IsValidEmployee(model).Result;

                //If the employee Is Valid and present in Db we redirect them to their respective Pages that is weather
                //admin page or Employee Page
                if (isValidUser != null)
                {
                    string isAdminString = null;
                    if (isValidUser.employeeObj.AdminPrivilege == true)
                    {
                        isAdminString = "True";
                    }
                    else
                    {
                        isAdminString = "False";
                    }
                    HttpContext.Session.SetString("EmployeeID", isValidUser.employeeObj.EmployeeId);
                    HttpContext.Session.SetString("EmpFirstName", isValidUser.employeeObj.EmployeeFirstname);
                    HttpContext.Session.SetString("AdminPrivelege", isAdminString);
                    return HttpContext.Session.GetString("AdminPrivelege") == "True" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index","Employee");
                    //return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You entered wrong username and password Combination");
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }




}
