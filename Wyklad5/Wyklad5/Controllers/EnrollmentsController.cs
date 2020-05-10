using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wyklad5.ModelsDTO;
using Wyklad5.Services;

namespace Wyklad5.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : Controller
    {
        private IStudentsDbService _dbService;
        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Enroll(StudentEnroll newStudent)
        {
            try
            {
                return CreatedAtAction("Enroll", _dbService.Enroll(newStudent));
            }
            catch (Exception e)
            {
                return NotFound("Exception: " + e.Message);
            }
        }

        [HttpPost("promotions")]
        public IActionResult Promote(Promotion promo)
        {
            try
            {
                return CreatedAtAction("Promote", _dbService.Promote(promo));
            }
            catch (Exception e)
            {
                return NotFound("Exception: " + e.Message);
            }
        }
    }
}