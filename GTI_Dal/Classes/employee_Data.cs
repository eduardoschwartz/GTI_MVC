using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Employee_Data {

        private static string _connection;
        public Employee_Data(string sConnection) {
            _connection = sConnection;
        }


        public IEnumerable<employees> ListaEmployee() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return db.Employees.ToList();
            }

        }

        public employees RetornaEmployee(int id) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return db.Employees.FirstOrDefault(e=>e.Id==id);
            }
        }

        public Exception SaveEmployee(employees employee) {
            using (GTI_Context db = new GTI_Context(_connection)) {

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO employees(id,firstname,lastname,gender,salary) VALUES(@id,@firstname,@lastname,@gender,@salary)",
                        new SqlParameter("@id", employee.Id),
                        new SqlParameter("@firstname", employee.FirstName),
                        new SqlParameter("@lastname", employee.LastName),
                        new SqlParameter("@gender", employee.Gender),
                        new SqlParameter("@salary", employee.Salary));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public void DeleteEmployee(int id) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                employees b = db.Employees.First(i => i.Id == id);
                db.Employees.Remove(b);
                db.SaveChanges();
            }
        }

        public Exception UpdateEmployee(int id, employees employee) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                employees b = db.Employees.First(i => i.Id == id);
                b.FirstName = employee.FirstName;
                b.LastName = employee.LastName;
                b.Gender = employee.Gender;
                b.Salary = employee.Salary;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }



    }
}
