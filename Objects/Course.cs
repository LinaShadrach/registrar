using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Registrar.Objects
{
  public class Course
  {
    public int Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}

    public Course(string name, string description, int id = 0)
    {
      this.Id = id;
      this.Name = name;
      this.Description = description;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool nameEquality = (this.Name == newCourse.Name);
        bool descriptionEquality = (this.Description == newCourse.Description);
        return (nameEquality && descriptionEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, description) OUTPUT INSERTED.id VALUES (@CourseName, @CourseDescription);", conn);

      SqlParameter courseNameParameter = new SqlParameter("@CourseName", this.Name);
      cmd.Parameters.Add(courseNameParameter);
      SqlParameter courseDescriptionParameter = new SqlParameter("@CourseDescription", this.Description);
      cmd.Parameters.Add(courseDescriptionParameter);


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

    public static List<Course> GetAll()
    {
      List<Course> courseList = new List<Course>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string description = rdr.GetString(2);
        Course newCourse = new Course(courseName, description, courseId);
        courseList.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return courseList;
    }

    public static Course Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);
      SqlParameter courseIdParameter = new SqlParameter("@CourseId", id.ToString());
      cmd.Parameters.Add(courseIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundCourseId = 0;
      string foundCourseName = null;
      string foundCourseDescription = null;

      while(rdr.Read())
      {
        foundCourseId = rdr.GetInt32(0);
        foundCourseName = rdr.GetString(1);
        foundCourseDescription = rdr.GetString(2);
      }
      Course foundCourse = new Course(foundCourseName, foundCourseDescription, foundCourseId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundCourse;
    }
    public void Edit(string name, string description)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE courses SET name = @CourseName, description = @Description OUTPUT INSERTED.id, INSERTED.name, INSERTED.description WHERE id = @CourseId;", conn);

      SqlParameter courseIdParameter = new SqlParameter("@CourseId", this.Id);

       SqlParameter courseNameParameter = new SqlParameter("@CourseName", name);

       SqlParameter courseDescriptionParameter = new SqlParameter("@Description", description);


       cmd.Parameters.Add(courseIdParameter);
       cmd.Parameters.Add(courseNameParameter);
       cmd.Parameters.Add(courseDescriptionParameter);


       SqlDataReader rdr = cmd.ExecuteReader();

       while(rdr.Read())
       {
         this.Id = rdr.GetInt32(0);
         this.Name = rdr.GetString(1);
         this.Description= rdr.GetString(2);
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

    public static List<Course> Sort()
    {
      List<Course> allCourses = new List<Course>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses ORDER BY name;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string description = rdr.GetString(2);
        Course newCourse = new Course(courseName, description, courseId);
        allCourses.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allCourses;
    }

    public void AddStudent(Student newStudent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);", conn);
      SqlParameter courseParam = new SqlParameter("@CourseId", this.Id);
      SqlParameter studentParam = new SqlParameter("@StudentId", newStudent.Id);
      cmd.Parameters.Add(courseParam);
      cmd.Parameters.Add(studentParam);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Student> GetStudents()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("SELECT students.* FROM courses JOIN courses_students ON (courses.id = courses_students.course_id) JOIN students ON (courses_students.student_id = students.id) WHERE courses.id = @CourseId;", conn);
      SqlParameter courseParam = new SqlParameter("@CourseId", this.Id.ToString());
      cmd.Parameters.Add(courseParam);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Student> students = new List<Student>{};

      while (rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        DateTime enrollmentDate = rdr.GetDateTime(2);
        Student newStudent = new Student(studentName, enrollmentDate, studentId);
        students.Add(newStudent);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return students;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM courses WHERE id = @CourseId; DELETE FROM courses_students WHERE course_id = @CourseId;", conn);
      SqlParameter courseIdParameter = new SqlParameter("@CourseId", this.Id);
      cmd.Parameters.Add(courseIdParameter);
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
      SqlCommand cmd = new SqlCommand("DELETE FROM courses; DELETE FROM courses_students;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

  }
}
