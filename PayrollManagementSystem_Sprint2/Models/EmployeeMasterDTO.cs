using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementSystem_Sprint2.Models
{
    public class EmployeeMasterDTO
    {

        public string EmployeeId { get; set; }        
        public string EmployeeFirstname { get; set; }
        public string EmployeeLastname { get; set; }
        public string EmployeeUserName { get; set; }
        public bool? AdminPrivilege { get; set; }

        
    }
}
