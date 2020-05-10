using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad5.ModelsDTO
{
    public class StudentDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Semester { get; set; } //Enrollment
        public string BirthDate { get; set; }
        public string Name { get; set; } //Studies

    }
}
