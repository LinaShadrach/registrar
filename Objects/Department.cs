// using System;
// using System.Collections.Generic;
// using System.Data.SqlClient;
//
// namespace Registrar.Objects
// {
//   public class Department
//   {
//     public int Id {get; set;}
//     public string Name {get; set;}
//
//     public Department(string name, int id = 0)
//     {
//       this.Id = id;
//       this.Name = name;
//       _enrollmentDate = enrollmentDate;
//     }
//
//     public override bool Equals(System.Object otherDepartment)
//     {
//       if (!(otherDepartment is Department))
//       {
//         return false;
//       }
//       else
//       {
//         Department newDepartment = (Department) otherDepartment;
//         bool nameEquality = (this.Name == newDepartment.Name);
//         bool enrollmentDateEquality = (this.GetEnrollmentDate() == newDepartment.GetEnrollmentDate());
//         return (nameEquality && enrollmentDateEquality);
//       }
//     }
//     public override int GetHashCode()
//     {
//       return this.Name.GetHashCode();
//     }
//
//     public DateTime GetEnrollmentDate()
//     {
//       return _enrollmentDate;
//     }
//     public void SetEnrollmentDate(DateTime enrollmentDate)
//     {
//       _enrollmentDate = enrollmentDate;
//     }
//     public void AddCourse(Course newCourse)
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//       SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @DepartmentId);", conn);
//       SqlParameter studentParam = new SqlParameter("@DepartmentId", this.Id.ToString());
//       SqlParameter courseParam = new SqlParameter("@CourseId", newCourse.Id.ToString());
//       cmd.Parameters.Add(courseParam);
//       cmd.Parameters.Add(studentParam);
//       cmd.ExecuteNonQuery();
//
//       if (conn != null)
//       {
//         conn.Close();
//       }
//     }
//
//     public List<Course> GetCourses()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//       SqlCommand cmd = new SqlCommand("SELECT courses.* FROM students JOIN courses_students ON (students.id = courses_students.student_id) JOIN courses ON (courses_students.course_id = courses.id) WHERE students.id = @DepartmentId;", conn);
//       SqlParameter courseParam = new SqlParameter("@DepartmentId", this.Id.ToString());
//       cmd.Parameters.Add(courseParam);
//       SqlDataReader rdr = cmd.ExecuteReader();
//
//       List<Course> courses = new List<Course>{};
//
//       while (rdr.Read())
//       {
//         int courseId = rdr.GetInt32(0);
//         string courseName = rdr.GetString(1);
//         string courseDescription = rdr.GetString(2);
//         Course newCourse = new Course(courseName, courseDescription, courseId);
//         courses.Add(newCourse);
//       }
//
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//
//       return courses;
//     }
//     public void Save()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@DepartmentName, @DepartmentEnrollmentDate);", conn);
//
//       SqlParameter studentNameParameter = new SqlParameter("@DepartmentName", this.Name);
//       cmd.Parameters.Add(studentNameParameter);
//       SqlParameter studentEnrollmentDateParameter = new SqlParameter("@DepartmentEnrollmentDate", this.GetEnrollmentDate());
//       cmd.Parameters.Add(studentEnrollmentDateParameter);
//
//
//       SqlDataReader rdr = cmd.ExecuteReader();
//
//       while(rdr.Read())
//       {
//         this.Id = rdr.GetInt32(0);
//       }
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//     }
//
//     public static List<Department> GetAll()
//     {
//       List<Department> studentList = new List<Department>{};
//
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("SELECT * FROM students;", conn);
//       SqlDataReader rdr = cmd.ExecuteReader();
//
//       while(rdr.Read())
//       {
//         int studentId = rdr.GetInt32(0);
//         string studentName = rdr.GetString(1);
//         DateTime enrollmentDate = rdr.GetDateTime(2);
//         Department newDepartment = new Department(studentName, enrollmentDate, studentId);
//         studentList.Add(newDepartment);
//       }
//
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//
//       return studentList;
//     }
//
//     public static Department Find(int id)
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @DepartmentId;", conn);
//       SqlParameter studentIdParameter = new SqlParameter("@DepartmentId", id.ToString());
//       cmd.Parameters.Add(studentIdParameter);
//       SqlDataReader rdr = cmd.ExecuteReader();
//
//       int foundDepartmentId = 0;
//       string foundDepartmentName = null;
//       DateTime foundDepartmentEnrollmentDate = new DateTime();
//
//       while(rdr.Read())
//       {
//         foundDepartmentId = rdr.GetInt32(0);
//         foundDepartmentName = rdr.GetString(1);
//         foundDepartmentEnrollmentDate = rdr.GetDateTime(2);
//       }
//       Department foundDepartment = new Department(foundDepartmentName, foundDepartmentEnrollmentDate, foundDepartmentId);
//
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//
//       return foundDepartment;
//     }
//     public void Edit(string name, DateTime enrollmentDate)
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("UPDATE students SET name = @DepartmentName, enrollment_date = @EnrollmentDate OUTPUT INSERTED.id, INSERTED.name, INSERTED.enrollment_date WHERE id = @DepartmentId;", conn);
//
//       SqlParameter studentIdParameter = new SqlParameter("@DepartmentId", this.Id);
//
//        SqlParameter studentNameParameter = new SqlParameter("@DepartmentName", name);
//
//        SqlParameter studentEnrollmentDateParameter = new SqlParameter("@EnrollmentDate", enrollmentDate);
//
//
//        cmd.Parameters.Add(studentIdParameter);
//        cmd.Parameters.Add(studentNameParameter);
//        cmd.Parameters.Add(studentEnrollmentDateParameter);
//
//
//        SqlDataReader rdr = cmd.ExecuteReader();
//
//        while(rdr.Read())
//        {
//          this.Id = rdr.GetInt32(0);
//          this.Name = rdr.GetString(1);
//          this.SetEnrollmentDate(rdr.GetDateTime(2));
//        }
//        if (rdr != null)
//        {
//          rdr.Close();
//        }
//        if (conn != null)
//        {
//          conn.Close();
//        }
//      }
//
//     public static List<Department> Sort()
//     {
//       List<Department> allDepartments = new List<Department>{};
//
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("SELECT * FROM students ORDER BY name;", conn);
//       SqlDataReader rdr = cmd.ExecuteReader();
//
//       while(rdr.Read())
//       {
//         int studentId = rdr.GetInt32(0);
//         string studentName = rdr.GetString(1);
//         DateTime enrollmentDate = rdr.GetDateTime(2);
//         Department newDepartment = new Department(studentName, enrollmentDate, studentId);
//         allDepartments.Add(newDepartment);
//       }
//
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//
//       return allDepartments;
//     }
//     public void Delete()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//       SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @DepartmentId; DELETE FROM students_courses WHERE student_id = @DepartmentId", conn);
//       SqlParameter studentIdParameter = new SqlParameter("@DepartmentId", this.Id);
//       cmd.Parameters.Add(studentIdParameter);
//       cmd.ExecuteNonQuery();
//
//       if(conn!=null)
//       {
//         conn.Close();
//       }
//     }
//     public static void DeleteAll()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//       SqlCommand cmd = new SqlCommand("DELETE FROM students; DELETE FROM courses_students;", conn);
//       cmd.ExecuteNonQuery();
//       conn.Close();
//     }
//
//   }
// }
