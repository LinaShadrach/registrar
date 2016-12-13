using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Registrar.Objects
{
  public class Student
  {
    public int Id {get; set;}
    public string Name {get; set;}
    private DateTime _enrollmentDate = new DateTime();

    public Student(string name, DateTime enrollmentDate, int id = 0)
    {
      this.Id = id;
      this.Name = name;
      _enrollmentDate = enrollmentDate;
    }

    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;
        bool nameEquality = (this.Name == newStudent.Name);
        bool enrollmentDateEquality = (this.GetEnrollmentDate() == newStudent.GetEnrollmentDate());
        return (nameEquality && enrollmentDateEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public DateTime GetEnrollmentDate()
    {
      return _enrollmentDate;
    }
    public void SetEnrollmentDate(DateTime enrollmentDate)
    {
      _enrollmentDate = enrollmentDate;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@StudentName, @StudentEnrollmentDate);", conn);

      SqlParameter studentNameParameter = new SqlParameter("@StudentName", this.Name);
      cmd.Parameters.Add(studentNameParameter);
      SqlParameter studentEnrollmentDateParameter = new SqlParameter("@StudentEnrollmentDate", this.GetEnrollmentDate());
      cmd.Parameters.Add(studentEnrollmentDateParameter);


      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static List<Student> GetAll()
    {
      List<Student> studentList = new List<Student>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        DateTime enrollmentDate = rdr.GetDateTime(2);
        Student newStudent = new Student(studentName, enrollmentDate, studentId);
        studentList.Add(newStudent);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return studentList;
    }

    public static Student Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);
      SqlParameter studentIdParameter = new SqlParameter("@StudentId", id.ToString());
      cmd.Parameters.Add(studentIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundStudentId = 0;
      string foundStudentName = null;
      DateTime foundStudentEnrollmentDate = new DateTime();

      while(rdr.Read())
      {
        foundStudentId = rdr.GetInt32(0);
        foundStudentName = rdr.GetString(1);
        foundStudentEnrollmentDate = rdr.GetDateTime(2);
      }
      Student foundStudent = new Student(foundStudentName, foundStudentEnrollmentDate, foundStudentId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundStudent;
    }
    public void Edit(string name, DateTime enrollmentDate)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE students SET name = @StudentName, enrollment_date = @EnrollmentDate OUTPUT INSERTED.id, INSERTED.name, INSERTED.enrollment_date WHERE id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter("@StudentId", this.Id);

       SqlParameter studentNameParameter = new SqlParameter("@StudentName", name);

       SqlParameter studentEnrollmentDateParameter = new SqlParameter("@EnrollmentDate", enrollmentDate);


       cmd.Parameters.Add(studentIdParameter);
       cmd.Parameters.Add(studentNameParameter);
       cmd.Parameters.Add(studentEnrollmentDateParameter);


       SqlDataReader rdr = cmd.ExecuteReader();

       while(rdr.Read())
       {
         this.Id = rdr.GetInt32(0);
         this.Name = rdr.GetString(1);
         this.SetEnrollmentDate(rdr.GetDateTime(2));
       }
       if (rdr != null)
       {
         rdr.Close();
       }
       if (conn != null)
       {
         conn.Close();
       }
     }

    public static List<Student> Sort()
    {
      List<Student> allStudents = new List<Student>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students ORDER BY name;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        DateTime enrollmentDate = rdr.GetDateTime(2);
        Student newStudent = new Student(studentName, enrollmentDate, studentId);
        allStudents.Add(newStudent);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allStudents;
    }
    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @StudentId; DELETE FROM students_courses WHERE student_id = @StudentId", conn);
      SqlParameter studentIdParameter = new SqlParameter("@StudentId", this.Id);
      cmd.Parameters.Add(studentIdParameter);
      cmd.ExecuteNonQuery();

      if(conn!=null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students; DELETE FROM courses_students;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

  }
}
