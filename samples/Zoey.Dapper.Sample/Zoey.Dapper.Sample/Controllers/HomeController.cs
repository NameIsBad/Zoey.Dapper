using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Sample.Models;

namespace Zoey.Dapper.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISqlContext _sqlContext;
        private readonly ISqlCommand _sqlCommand;
        public HomeController(ISqlContext sqlContext, ISqlCommand sqlCommand)
        {
            this._sqlContext = sqlContext;
            _sqlCommand = sqlCommand;
        }

        public IActionResult Index()
        {
            var student = _sqlCommand.GetSqlElement("Student.GetStudentByID").Query<Student>(new
            {
                ID = 1
            });

            return View(student);
        }

        public IActionResult About()
        {
            var sql = _sqlContext.GetSqlElement("Student.UpdateStudentByID");
            using (var db = new SqlConnection("Data Source=.;Initial Catalog=Test;Integrated Security=True"))
            {
                db.Execute(sql.CommandText, new
                {
                    Age = new Random().Next(100),
                    Name = "Hello Zoey!",
                    ID = 1
                });
            }
            return View();
        }
    }
}
