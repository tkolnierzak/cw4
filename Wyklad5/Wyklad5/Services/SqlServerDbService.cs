using Wyklad5.Exceptions;
using Wyklad5.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Procedures.PromoProcedure.sql;

namespace Wyklad5.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17571;Integrated Security=True";

        public StudentDetails FetchStudent(string id)
        {
            var s = new StudentDetails();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = connection;
                comm.CommandText = "select s.FirstName, s.LastName, s.IndexNumber, s.BirthDate, e.Semester, st.Name from Student s " +
                    "join Studies st on st.IdStudy = e.IdStudy " +
                    "join Enrollment e on e.IdEnrollment = s.IdEnrollment " +
                    "where s.IndexNumber = @id";
                comm.Parameters.AddWithValue("id", id);
                connection.Open();
                SqlDataReader reader = comm.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                s.FirstName = reader["FirstName"].ToString();
                s.LastName = reader["LastName"].ToString();
                s.Semester = reader["Semester"].ToString();
                s.BirthDate = reader["BirthDate"].ToString();
                s.Name = reader["Name"].ToString();
            }
            return s;
        }

        public IEnumerable<StudentDetails> FetchStudents()
        {
            var studentsDetails = new List<StudentDetails>();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = connection;
                comm.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s " +
                    "join Studies st on st.IdStudy = e.IdStudy join Enrollment e on e.IdEnrollment = s.IdEnrollment ";
                connection.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    var student = new StudentDetails
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Semester = reader["Semester"].ToString(),
                        BirthDate = reader["BirthDate"].ToString(),
                        Name = reader["Name"].ToString()
                    };
                    studentsDetails.Add(student);
                }
            }
            return studentsDetails;
        }

        public IEnumerable<EnrollDetails> FetchStudentEnrollments(string id)
        {
            var enrollmentDetails = new List<EnrollDetails>();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = connection;
                comm.CommandText = "select s.IndexNumber, e.Semester, st.Name, e.StartDate from Student s " +
                    "join Studies st on st.IdStudy = e.IdStudy " +
                    "join Enrollment e on e.IdEnrollment = s.IdEnrollment " +
                    "where s.IndexNumber = @id";
                comm.Parameters.AddWithValue("id", id);
                connection.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    var enrollment = new EnrollDetails
                    {
                        StartDate = reader["StartDate"].ToString(),
                        Semester = reader["Semester"].ToString(),
                        Name = reader["Name"].ToString(),
                    };
                    enrollmentDetails.Add(enrollment);
                }
            }

            return enrollmentDetails;
        }

        public Enrollment Enroll(StudentEnroll studentNew)
        {
            var enroll = new Enrollment();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();
                comm.Transaction = transaction;
                SqlDataReader reader = null;
                try
                {
                    comm.CommandText = "select IdStudy from studies where name = @name ";
                    comm.Parameters.AddWithValue("name", studentNew.Studies);
                    reader = comm.ExecuteReader();
                    if (!reader.Read())
                    {
                        throw new Exception("No studies: " + studentNew.Studies);
                    }
                    if (FetchStudent(studentNew.IndexNumber) != null)
                    {
                        throw new Exception("Choose different index " + studentNew.IndexNumber);
                    }
                    var idStudy = reader["IdStudy"].ToString();
                    reader.Close();
                    comm.CommandText = "select * from Enrollment where Semester = 1 and IdStudy = @idStudy ";
                    comm.Parameters.AddWithValue("idStudy", idStudy);
                    reader = comm.ExecuteReader();
                    
                    string startDate;
                    string idEnrollment;
                    if (reader.Read())
                    {
                        startDate = reader["StartDate"].ToString();
                        idEnrollment = reader["IdEnrollment"].ToString();
                    }
                    else
                    {
                        reader.Close();
                        comm.CommandText = "select max(IdEnrollment) as MaxIdEnrollment from Enrollment ";
                        reader.Read();
                        idEnrollment = (int.Parse(reader["currentMax"].ToString()) + 1).ToString();
                        startDate = "2020-03-29";
                        reader.Close();
                        comm.CommandText = "insert into Enrollment(Semester, IdEnrollment, IdStudy, StartDate) values(@Semester, @newId, @IdStudy, @StartDate) ";
                        comm.Parameters.AddWithValue("Semester", 1);
                        comm.Parameters.AddWithValue("newId", idEnrollment);
                        comm.Parameters.AddWithValue("IdStudy", idStudy);
                        comm.Parameters.AddWithValue("StartDate", startDate);
                        comm.ExecuteNonQuery();
                    }
                    reader.Close();

                    comm.CommandText = "insert into Student (IndexNumber, FirstName, LastName, IdEnrollment, BirthDate) values(@IndexNumber, @FirstName, @LastName, @IdEnrollment, @BirthDate) ";
                    comm.Parameters.AddWithValue("IndexNumber", studentNew.IndexNumber);
                    comm.Parameters.AddWithValue("FirstName", studentNew.FirstName);
                    comm.Parameters.AddWithValue("LastName", studentNew.LastName);
                    comm.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    comm.Parameters.AddWithValue("BirthDate", studentNew.BirthDate);
                    comm.ExecuteNonQuery();
                    enroll = new Enrollment()
                    {
                        IdEnrollment = idEnrollment,
                        Semester = "1",
                        StartDate = startDate,
                        IdStudy = idStudy
                    };
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    if (reader != null)
                    {
                        reader.Close();
                        transaction.Rollback();
                    }
                    throw e;
                }
            }
            return enroll;
        }

        public Enrollment Promote(Promotion promo)
        {
            var enroll = new Enrollment();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();
                comm.Transaction = transaction;
                SqlDataReader reader = null;
                try
                {
                    comm.CommandText = "select IdStudy from studies where name = @studies ";
                    comm.Parameters.AddWithValue("studies", promo.Studies);
                    reader = comm.ExecuteReader();
                    if (!reader.Read())
                    {
                        throw new Exception("No studies: " + promo.Studies);
                    }
                    reader.Close();

                    comm.CommandText = "select e.IdEnrollment, e.StartDate, e.Semester, s.IdStudy from enrollment e join studies s on e.IdStudy = s.IdStudy " +
                        "where e.semester = @semester_2 and s.name = @studies_2 ";
                    comm.Parameters.AddWithValue("semester_2", promo.Semester);
                    comm.Parameters.AddWithValue("studies_2", promo.Studies);
                    reader = comm.ExecuteReader();
                    if (!reader.Read())
                    {
                        throw new Exception("No semester: " + promo.Semester);
                    }
                    else
                    {
                        reader.Close();
                        comm.CommandText = "exec promoProcedure @Studies_3, @Semester_3";
                        comm.Parameters.AddWithValue("Semester_3", promo.Semester);
                        comm.Parameters.AddWithValue("Studies_3", promo.Studies);
                        reader = comm.ExecuteReader();
                        if (reader.Read())
                        {
                            enroll = new Enrollment()
                            {
                                IdStudy = reader["IdStudy"].ToString(),
                                IdEnrollment = reader["IdEnrollment"].ToString(),
                                Semester = reader["Semester"].ToString(),
                                StartDate = reader["StartDate"].ToString()
                            };
                        }
                        reader.Close();
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    if (reader != null)
                    {
                        reader.Close();
                        transaction.Rollback();
                    }
                    throw e;
                }
            }
            return enroll;
        }
    }
}
