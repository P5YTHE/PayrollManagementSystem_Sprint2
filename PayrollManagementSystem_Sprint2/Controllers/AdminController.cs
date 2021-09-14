using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayrollManagementSystem_Sprint2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PayrollManagementSystem_Sprint2.Controllers
{
    
    public class AdminController : Controller
    {
        static string localHostLink = "https://localhost:44314/";  //localhost link variable

        static List<PayrollMaster> payrollReport = new List<PayrollMaster>();  //static list to hold payroll for downloading
        
        public IActionResult Index()
        {            
                return View();
        }      //Index Page

        public IActionResult AdminDashboard()
        {
            return View();
        }          //Admin Dashboard Page
      
        public void ViewBagCheck(bool variable)
        {
            if (variable)
            {
                ViewBag.Message = "Operation Successfull";
                ViewBag.Color = "greenyellow";
            }
            else
            {
                ViewBag.Message = "Operation unsuccessful";
                ViewBag.Color = "tomato";
            }
        }        //Pass change status to viewbag 
        
        #region Employees     //Consists of all functions regarding EmployeeMaster and EmpAddress Tables

        public async Task<IActionResult> Employees()
        {
            List<EmployeeMaster> empList = new List<EmployeeMaster>();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"api/EmployeeMasters");            
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                empList = JsonConvert.DeserializeObject<List<EmployeeMaster>>(apiResponse);
                return View(empList);
            }
            else { return Unauthorized(); }
        }    //View All Employees

        [HttpGet]
        public async Task<IActionResult> EmployeeDelete(string id)
        {
            var emp = new EmployeeMaster();
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{localHostLink}api/EmployeeMasters/{id}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                emp = JsonConvert.DeserializeObject<EmployeeMaster>(apiResponse);                
            }
            return View(emp);
        }       //Delete one employee

        [HttpPost]
        public async Task<IActionResult> EmployeeDelete(string id, EmployeeMaster emp) 
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"{localHostLink}api/EmployeeMasters/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    ViewBagCheck(response.IsSuccessStatusCode);
                }                 
            }
            return View();
        }        //Post method Delete one employee

        public async Task<IActionResult> ViewAddressDetails(string id) 
        {
            var empAddress = new EmpAddress();
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{localHostLink}api/EmpAddresses/{id}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                empAddress = JsonConvert.DeserializeObject<EmpAddress>(apiResponse);
            }
            return View(empAddress);
        }          //View All Employee Addresses

        public ActionResult CreateNewEmployee()     
        {            
            return View();
        }          //Create New Employee Function

        [HttpPost]
        public async Task<ActionResult> CreateNewEmployee(EmployeeMaster employee)
        {
            HttpClient httpClient = new HttpClient();
            var response = httpClient.PostAsJsonAsync<EmployeeMaster>($"{localHostLink}api/EmployeeMasters", employee);
            response.Wait();
            var result = response.Result;
            if (result.IsSuccessStatusCode)
            {
                ViewBag.Message = "New Employee Created Successfully";
                ViewBag.Color = "greenyellow";
            }
            else
            {
                ViewBag.Message = "Exployee not inserted,Kindly check all the details again";
                ViewBag.Color = "tomato";
            }
            return View();
        }     //Post Method for Create New Employee

        public async Task<IActionResult> EditEmployeeDetails(string id)
        {
            EmployeeMaster emplist = new EmployeeMaster();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{localHostLink}api/EmployeeMasters/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    emplist = JsonConvert.DeserializeObject<EmployeeMaster>(apiResponse);
                }
            }
            return View(emplist);
        }    //Edit Employee Details

        [HttpPost]
        public async Task<IActionResult> EditEmployeeDetails(EmployeeMaster employee)
        {
            using (var httpClient = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(employee);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync($"{localHostLink}api/EmployeeMasters/" + employee.EmployeeId, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);
            }
            return View(employee);
        }   //Post Method for Edit Employee

        #endregion

        #region Timesheet   //Consists of all functions regarding Timesheet

        public async Task<IActionResult> EmployeeTimesheets()
        {
            List<TimeSheet> timesheetList = new List<TimeSheet>();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"api/TimeSheets");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                timesheetList = JsonConvert.DeserializeObject<List<TimeSheet>>(apiResponse);
                return View(timesheetList);
            }
            else
            {
                return Unauthorized();
            }
        }        //View All Timesheets

        public async Task<IActionResult> EditTimesheet(int id)
        {
            TimeSheet timesheetList = new TimeSheet();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{localHostLink}api/Timesheets/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    timesheetList = JsonConvert.DeserializeObject<TimeSheet>(apiResponse);
                }
            }
            return View(timesheetList);
        }       //Approve Timesheets

        [HttpPost]
        public async Task<IActionResult> EditTimesheet(TimeSheet timesheet)
        {
            using (var httpClient = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(timesheet);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync($"{localHostLink}api/Timesheets/" + timesheet.IndexTs, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);
            }
            return View(timesheet);
        }    //Post method Approve Timesheets

        #endregion

        #region Payroll   //Consists of all functions regarding Payroll Master and Payroll Details        	

        public async Task<IActionResult> PayrollReport() 
        {
            payrollReport.Clear();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = HttpContext.Session.GetString("EmployeeID");
            var response = await httpClient.GetAsync($"api/PayrollMasters");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                
                payrollReport = JsonConvert.DeserializeObject<List<PayrollMaster>>(apiResponse);
                var a = 3;
                return View(payrollReport);
            }
            else
            {
                return Unauthorized();
            }
        }    //View All Payroll 

        public async Task<IActionResult> ViewPayrollDetails(string id)
        {
            var payrollDetail = new PayrollMaster();
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{localHostLink}api/PayrollMasters/{id}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                payrollDetail = JsonConvert.DeserializeObject<PayrollMaster>(apiResponse);
            }
            return View(payrollDetail);
        }   //View Payroll Details

        public async Task<IActionResult> EditGradeDetails(string id)
        {
            PayrollMaster emplist = new PayrollMaster();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{localHostLink}api/PayrollMasters/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    emplist = JsonConvert.DeserializeObject<PayrollMaster>(apiResponse);
                }
            }
            return View(emplist);
        }             //Edit Employee  Grade Details

        [HttpPost]
        public async Task<IActionResult> EditGradeDetails(PayrollMaster employee2)
        {
            using (var httpClient = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(employee2);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync($"{localHostLink}api/PayrollMasters/" + employee2.EmployeeId, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);
            }
            return View(employee2);
        }   //Edit Employee Grade and Designation

        public async Task<IActionResult> PayrollReportDownload()
        {            
           
            return View(payrollReport);
        }       //Download Payroll Report 

        public FileResult DownloadPayroll()
        {            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Payroll report - " + DateTime.Now);
            sb.AppendLine("_______________________________________________");
            foreach (var item in payrollReport)
            {
                sb.AppendLine(item.EmployeeId+"|"+item.EmployeeDesignation+"|"+item.EmployeeGrade + "|" + item.EmployeeSalary);
            }                
            var data = sb.ToString();
            var byteArray = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);            
            return File(stream, "text/plain", "Report"+DateTime.Today+".txt");
        }       //Return Report File

        #endregion

        #region Leaves  //Consists of all functions regarding Leave Details and Leave Master Table

        public async Task<IActionResult> LeaveDetails()
        {
            List<LeaveDetail> leaveDetail = new List<LeaveDetail>();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"api/LeaveDetails");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                leaveDetail = JsonConvert.DeserializeObject<List<LeaveDetail>>(apiResponse);
                return View(leaveDetail);
            }
            else
            {
                return Unauthorized();
            }
        }         //Display Leave Details table

        public async Task<IActionResult> ViewLeaveDetails(string id)
        {
            var leaveDetail = new LeaveDetail();
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{localHostLink}api/LeaveDetails/{id}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                leaveDetail = JsonConvert.DeserializeObject<LeaveDetail>(apiResponse);
            }
            return View(leaveDetail);
        }    //View Leave Details

        public async Task<IActionResult> EditLeaveStatus(int id)
        {
            LeaveDetail leaveDetail = new LeaveDetail();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{localHostLink}api/LeaveDetails/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    leaveDetail = JsonConvert.DeserializeObject<LeaveDetail>(apiResponse);
                }
            }
            return View(leaveDetail);
        }     //Edit Leave Status

        [HttpPost]
        public async Task<IActionResult> EditLeaveStatus(LeaveDetail leaveDetail)
        {
            using (var httpClient = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(leaveDetail);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PutAsync($"{localHostLink}api/LeaveDetails/" + leaveDetail.IndexLd, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);

            }
            return View(leaveDetail);
        }   //Edit Leave Status Post Method

        public async Task<IActionResult> LeaveMaster()
        {
            List<LeaveMaster> leaveMaster = new List<LeaveMaster>();
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{localHostLink}api/LeaveMasters");
            string apiResponse = await response.Content.ReadAsStringAsync();
            leaveMaster = JsonConvert.DeserializeObject<List<LeaveMaster>>(apiResponse);
            return View(leaveMaster);
        }    //View Leave Master Table

        public async Task<IActionResult> EditLeaveMaster(int id)
        {
            LeaveMaster leavesObj = new LeaveMaster();
            using (var httpClient = new HttpClient())
            {
                var res = HttpContext.Session.GetString("EmployeeID");
                
                using (var response = await httpClient.GetAsync($"{localHostLink}api/LeaveMasters/{res}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    leavesObj = JsonConvert.DeserializeObject<LeaveMaster>(apiResponse);
                }
            }
            return View(leavesObj);
        }     //Edit Leave Master

        [HttpPost]
        public async Task<IActionResult> EditLeaveMaster(LeaveMaster leavesObj)
        {
            using (var httpClient = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(leavesObj);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                var res = HttpContext.Session.GetString("EmployeeID");                
                HttpResponseMessage response = httpClient.PutAsync($"{localHostLink}api/LeaveMasters/" + res, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);
            }
            return View(leavesObj);
        }    //Edit Leave master Post

        #endregion

    }
}
