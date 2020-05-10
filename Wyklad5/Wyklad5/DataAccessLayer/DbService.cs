using Wyklad5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad5.DataAccessLayer
{
    public interface IDbService
{
    public IEnumerable<Student> FetchStudents();
}
}
