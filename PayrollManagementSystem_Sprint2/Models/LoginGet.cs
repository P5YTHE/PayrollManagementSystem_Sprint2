using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementSystem_Sprint2.Models
{
    public class LoginGet
    {
        public EmployeeMasterDTO EmpMasterObj { get; set; }
        public string _applicationSideToken { get; set; }

        public LoginGet()
        {
            
        }
    }
}
