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
    public int MajorId {get; set;}

    public Student(string name, DateTime enrollmentDate, int major = 0, int id = 0)
    {
      this.Id = id;
      this.Name = name;
      this.MajorId = major;
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
    public void AddCourse(Course newCourse)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);", conn);
      SqlParameter studentParam = new SqlParameter("@StudentId", this.Id.ToString());
      SqlParameter courseParam = new SqlParameter("@CourseId", newCourse.Id.ToString());
      cmd.Parameters.Add(courseParam);
      cmd.Parameters.Add(studentParam);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Course> GetCourses()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT courses.* FROM students JOIN courses_students ON (students.id = courses_students.student_id) JOIN courses ON (courses_students.course_id = courses.id) WHERE students.id = @StudentId;", conn);
      SqlParameter courseParam = new SqlParameter("@StudentId", this.Id.ToString());
      cmd.Parameters.Add(courseParam);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Course> courses = new List<Course>{};

      while (rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseDescription = rdr.GetString(2);
        int majorId = rdr.GetInt32(4);
        Course newCourse = new Course(courseName, courseDescription, majorId, courseId);
        courses.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return courses;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date, major_id) OUTPUT INSERTED.id VALUES (@StudentName, @StudentEnrollmentDate, @MajorId);", conn);

      SqlParameter studentNameParameter = new SqlParameter("@StudentName", this.Name);
      cmd.Parameters.Add(studentNameParameter);
      SqlParameter studentEnrollmentDateParameter = new SqlParameter("@StudentEnrollmentDate", this.GetEnrollmentDate());
      cmd.Parameters.Add(studentEnrollmentDateParameter);
      SqlParameter majorParam = new SqlParameter("@MajorId", this.MajorId);
      cmd.Parameters.Add(majorParam);


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
        int majorId = rdr.GetInt32(3);
        Student newStudent = new Student(studentName, enrollmentDate, majorId, studentId);
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
      int foundMajorId = 0;

      while(rdr.Read())
      {
        foundStudentId = rdr.GetInt32(0);
        foundStudentName = rdr.GetString(1);
        foundStudentEnrollmentDate = rdr.GetDateTime(2);
        foundMajorId = rdr.GetInt32(3);
      }
      Student foundStudent = new Student(foundStudentName, foundStudentEnrollmentDate, foundMajorId, foundStudentId);

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
    public void Edit(string name, DateTime enrollmentDate, int majorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE students SET name = @StudentName, enrollment_date = @EnrollmentDate, major_id = @MajorId OUTPUT INSERTED.id, INSERTED.name, INSERTED.enrollment_date, INSERTED.major_id WHERE id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter("@StudentId", this.Id);

       SqlParameter studentNameParameter = new SqlParameter("@StudentName", name);

       SqlParameter studentEnrollmentDateParameter = new SqlParameter("@EnrollmentDate", enrollmentDate);

       SqlParameter majorParam = new SqlParameter("@MajorId", majorId);

       cmd.Parameters.Add(studentIdParameter);
       cmd.Parameters.Add(studentNameParameter);
       cmd.Parameters.Add(studentEnrollmentDateParameter);
       cmd.Parameters.Add(majorParam);

       SqlDataReader rdr = cmd.ExecuteReader();

       while(rdr.Read())
       {
         this.Id = rdr.GetInt32(0);
         this.Name = rdr.GetString(1);
         this.SetEnrollmentDate(rdr.GetDateTime(2));
         this.MajorId = rdr.GetInt32(3);
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
        int majorId = rdr.GetInt32(3);
        Student newStudent = new Student(studentName, enrollmentDate, majorId, studentId);
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
