using Wyklad5.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad5.Services
{
    public interface IStudentsDbService
    {
        public StudentDetails FetchStudent(string id);
        public Enrollment Enroll(StudentEnroll newStudent);
        public Enrollment Promote(Promotion promo);
        public IEnumerable<StudentDetails> FetchStudents();
        public IEnumerable<EnrollmentDetails> FetchStudentEnrollments(string id);
    }
}

