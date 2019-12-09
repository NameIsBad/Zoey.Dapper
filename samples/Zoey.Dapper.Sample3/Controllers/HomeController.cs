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
        private readonly ISqlCommand _sqlCommand;
        public HomeController(ISqlCommand sqlCommand)
        {
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
    }
}
