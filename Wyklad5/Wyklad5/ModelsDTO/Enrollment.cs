using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad5.ModelsDTO
{
    public class Enrollment
    {
        public string IdEnrollment { get; set; }
        public string IdStudy { get; set; } //Studies
        public string StartDate { get; set; }
        public string Semester { get; set; }
    }
}
