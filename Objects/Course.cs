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
    public int MajorId {get; set;}


    public Course(string name, string description, int major = 0, int id = 0)
    {
      this.Id = id;
      this.Name = name;
      this.Description = description;
      this.MajorId = major;
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
        bool majorEquality = (this.MajorId == newCourse.MajorId);
        return (nameEquality && descriptionEquality && majorEquality);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, description, major_id) OUTPUT INSERTED.id VALUES (@CourseName, @CourseDescription, @MajorId);", conn);

      SqlParameter courseNameParameter = new SqlParameter("@CourseName", this.Name);
      cmd.Parameters.Add(courseNameParameter);
      SqlParameter courseDescriptionParameter = new SqlParameter("@CourseDescription", this.Description);
      cmd.Parameters.Add(courseDescriptionParameter);
      SqlParameter courseMajorParameter = new SqlParameter("@MajorId", this.MajorId);
      cmd.Parameters.Add(courseMajorParameter);

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
        int majorId = rdr.GetInt32(4);

        Course newCourse = new Course(courseName, description, majorId, courseId);
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
      int foundCourseMajorId = 0;

      while(rdr.Read())
      {
        foundCourseId = rdr.GetInt32(0);
        foundCourseName = rdr.GetString(1);
        foundCourseDescription = rdr.GetString(2);
        foundCourseMajorId = rdr.GetInt32(4);

      }
      Course foundCourse = new Course(foundCourseName, foundCourseDescription, foundCourseMajorId, foundCourseId);

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
    public void Edit(string name, string description, int majorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE courses SET name = @CourseName, description = @Description, major_id = @MajorID OUTPUT INSERTED.id, INSERTED.name, INSERTED.description, INSERTED.major_id WHERE id = @CourseId;", conn);

      SqlParameter courseIdParameter = new SqlParameter("@CourseId", this.Id);
       SqlParameter courseNameParameter = new SqlParameter("@CourseName", name);
       SqlParameter courseDescriptionParameter = new SqlParameter("@Description", description);
       SqlParameter courseMajorIdParameter = new SqlParameter("@MajorID", majorId);

       cmd.Parameters.Add(courseIdParameter);
       cmd.Parameters.Add(courseNameParameter);
       cmd.Parameters.Add(courseDescriptionParameter);
       cmd.Parameters.Add(courseMajorIdParameter);

       SqlDataReader rdr = cmd.ExecuteReader();

       while(rdr.Read())
       {
         this.Id = rdr.GetInt32(0);
         this.Name = rdr.GetString(1);
         this.Description= rdr.GetString(2);
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
        int majorId = rdr.GetInt32(4);
        Course newCourse = new Course(courseName, description, majorId, courseId);
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
      SqlParameter courseParam = new SqlParameter("@CourseId", this.Id.ToString());
      SqlParameter studentParam = new SqlParameter("@StudentId", newStudent.Id.ToString());
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
