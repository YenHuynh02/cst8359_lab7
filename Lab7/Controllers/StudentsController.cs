using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab7.Data;
using Lab7.Models;

namespace Lab7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the list of all students.
        /// </summary>
        /// <returns>A collection of students</returns>
        /// <response code="200">Returns the list of students</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        /// <summary>
        /// Returns a specific student by ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <returns>A single student</returns>
        /// <response code="200">Returns the student</response>
        /// <response code="400">Malformed ID</response>
        /// <response code="404">Student not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> GetStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        /// <summary>
        /// Updates a student record.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Students/5
        ///     {
        ///        "firstName": "Peter",
        ///        "lastName": "Hằng",
        ///        "program": "ICT"
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Student ID</param>
        /// <param name="student">Student object with updated info</param>
        /// <returns>Updated student</returns>
        /// <response code="200">Successfully updated</response>
        /// <response code="400">ID mismatch or bad data</response>
        /// <response code="404">Student not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutStudent(Guid id, [Bind("FirstName,LastName,Program")] Student student)
        {
            Student? storedStudent = null;

            try
            {
                storedStudent = _context.Students.Single(s => s.ID == id);
                storedStudent.FirstName = student.FirstName;
                storedStudent.LastName = student.LastName;
                storedStudent.Program = student.Program;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(storedStudent);
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Students
        ///     {
        ///        "firstName": "Peter",
        ///        "lastName": "Hằng",
        ///        "program": "ICT"
        ///     }
        ///
        /// </remarks>
        /// <returns>The newly created student</returns>
        /// <response code="201">Created successfully</response>
        /// <response code="400">Bad input</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> PostStudent([Bind("FirstName,LastName,Program")] Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="202">Student successfully deleted</response>
        /// <response code="400">Bad ID format</response>
        /// <response code="404">Student not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Accepted();
        }

        private bool StudentExists(Guid id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
