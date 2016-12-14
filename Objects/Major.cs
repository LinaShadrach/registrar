using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Registrar.Objects
{
  public class Major
  {
    public int Id {get; set;}
    public string Name {get; set;}
    public int MajorId {get; set;}


    public Major(string name = "biology", int id = 0)
    {
      this.Id = id;
      this.Name = name;
    }

    public override bool Equals(System.Object otherMajor)
    {
      if (!(otherMajor is Major))
      {
        return false;
      }
      else
      {
        Major newMajor = (Major) otherMajor;
        bool nameEquality = (this.Name == newMajor.Name);
        return (nameEquality);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO majors (name) OUTPUT INSERTED.id VALUES (@MajorName);", conn);

      SqlParameter majorNameParameter = new SqlParameter("@MajorName", this.Name);
      cmd.Parameters.Add(majorNameParameter);

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

    public static List<Major> GetAll()
    {
      List<Major> majorList = new List<Major>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM majors;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int majorId = rdr.GetInt32(0);
        string majorName = rdr.GetString(1);

        Major newMajor = new Major(majorName, majorId);
        majorList.Add(newMajor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return majorList;
    }

    public List<Course> GetCourses()
    {
      List<Course> allCourses = Course.GetAll();
      IEnumerable<Course> foundCourses = allCourses.Where(c => c.MajorId == this.MajorId);
      return foundCourses.ToList();
    }
    public List<Student> GetStudents()
    {
      List<Student> allStudents = Student.GetAll();
      IEnumerable<Student> foundStudents = allStudents.Where(c => c.MajorId == this.MajorId);
      return foundStudents.ToList();
    }

    public static Major Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM majors WHERE id = @MajorId;", conn);
      SqlParameter majorIdParameter = new SqlParameter("@MajorId", id.ToString());
      cmd.Parameters.Add(majorIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundMajorId = 0;
      string foundMajorName = null;

      while(rdr.Read())
      {
        foundMajorId = rdr.GetInt32(0);
        foundMajorName = rdr.GetString(1);

      }
      Major foundMajor = new Major(foundMajorName, foundMajorId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundMajor;
    }
    public void Edit(string name)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE majors SET name = @MajorName OUTPUT INSERTED.id, INSERTED.name WHERE id = @MajorId;", conn);

      SqlParameter majorIdParameter = new SqlParameter("@MajorId", this.Id);
       SqlParameter majorNameParameter = new SqlParameter("@MajorName", name);

       cmd.Parameters.Add(majorIdParameter);
       cmd.Parameters.Add(majorNameParameter);

       SqlDataReader rdr = cmd.ExecuteReader();

       while(rdr.Read())
       {
         this.Id = rdr.GetInt32(0);
         this.Name = rdr.GetString(1);
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

    public static List<Major> Sort()
    {
      List<Major> allMajors = new List<Major>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM majors ORDER BY name;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int majorId = rdr.GetInt32(0);
        string majorName = rdr.GetString(1);
        Major newMajor = new Major(majorName, majorId);
        allMajors.Add(newMajor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allMajors;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM majors WHERE id = @MajorId;", conn);
      SqlParameter majorIdParameter = new SqlParameter("@MajorId", this.Id);
      cmd.Parameters.Add(majorIdParameter);
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
      SqlCommand cmd = new SqlCommand("DELETE FROM majors;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

  }
}
