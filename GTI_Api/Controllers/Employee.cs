using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GTI_Bll.Classes;
using GTI_Models.Models;

namespace GTI_Api.Controllers {
    public class EmployeeController : ApiController {
        private readonly string _connection = "GTIconnection";


        // GET api/values
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value5" };
        }

        // GET api/values/5
        public string Get(int id) {
            Employee_bll employeeRepository = new Employee_bll(_connection);
            return employeeRepository.RetornaEmployee(id).FirstName;
        }

        // POST api/values
        public void Post([FromBody] string value) {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/values/5
        public void Delete(int id) {
        }
    }
}
