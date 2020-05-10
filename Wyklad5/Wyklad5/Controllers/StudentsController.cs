using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wyklad5.DataAccessLayer;
using Wyklad5.ModelsDTO;
using Wyklad5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Wyklad5.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string S1 = "Kowalski";
        private const string S2 = "Majewski";
        private const string S3 = "Andrzejewski";
        private readonly IStudentsDbService _dbService;

        public StudentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult FetchStudents(string orderBy)
        {
            try
            {
                return Ok(_dbService.FetchStudents());
            }
            catch (Exception e)
            {
                return BadRequest("Exception: " + e.Message + "\n" + e.StackTrace);
            }
        }

        [HttpGet("{id}")]
        public IActionResult FetchStudent(string id)
        {
            try
            {
                return Ok(_dbService.FetchStudent(id));
            }
            catch (Exception e)
            {
                return BadRequest("Exception: " + e.Message + "\n" + e.StackTrace);
            }
        }

        [HttpGet("enrollments/{id}")]
        public IActionResult FetchStudentEnrollments(string id)
        {
            try
            {
                return Ok(_dbService.FetchStudentEnrollments(id));
            }
            catch (Exception e)
            {
                return BadRequest("Exception: " + e.Message + "\n" + e.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut]
        public IActionResult UpdateStudent(Student student)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}