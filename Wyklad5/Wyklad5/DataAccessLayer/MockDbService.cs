using Wyklad5.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Wyklad5.DataAccessLayer
{
    public class MockDbService : IDbService
{
    private static IEnumerable<Student> _students;

    static MockDbService()
    {
        _students = new List<StudentDto>
            {
                new Student{IdStudent=1, FirstName="Adam", LastName="Kowalski"},
                new Student{IdStudent=2, FirstName="Artur", LastName="Kowal"},
                new Student{IdStudent=3, FirstName="Ewa", LastName="Nowacka"}
            };
    }

    public IEnumerable<Student> FetchStudents()
    {
        return _students;
    }
}
}