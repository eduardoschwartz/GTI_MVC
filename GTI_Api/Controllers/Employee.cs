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
        [HttpGet]
        public IEnumerable<employees> List_Employees() {
            Employee_bll employeeRepository = new Employee_bll(_connection);
            return employeeRepository.ListaEmployee().ToList();
        }

        // GET api/values/5
        [HttpGet]
        public HttpResponseMessage Return_Employee(int id) {
            Employee_bll employeeRepository = new Employee_bll(_connection);
            var reg= employeeRepository.RetornaEmployee(id);
            if (reg != null) {
                return Request.CreateResponse(HttpStatusCode.OK, reg);
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id " + id.ToString() + " not found!");
            }
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody] employees employee) {
            try {
                Employee_bll employeeRepository = new Employee_bll(_connection);
                Exception ex = employeeRepository.SaveEmployee(employee);

                var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                message.Headers.Location = new Uri(Request.RequestUri + employee.Id.ToString());
                return message;
            } catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int id, [FromBody] employees employee) {
            try {
                Employee_bll employeeRepository = new Employee_bll(_connection);
                Exception ex = employeeRepository.UpdateEmployee(id,employee);

                var message = Request.CreateResponse(HttpStatusCode.OK, employee);
                message.Headers.Location = new Uri(Request.RequestUri + employee.Id.ToString());
                return message;
            } catch {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id " + id.ToString() + " not found!");
            }
        }

        // DELETE api/values/5
        public HttpResponseMessage Delete(int id) {
            Employee_bll employeeRepository = new Employee_bll(_connection);
            var reg = employeeRepository.RetornaEmployee(id);
            if (reg != null) {
                employeeRepository.DeleteEmployee(id);
                return Request.CreateResponse(HttpStatusCode.OK, "Employee with id " + id.ToString() + " was deleted!");
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id " + id.ToString() + " not found!");
            }
        }


    }
}
