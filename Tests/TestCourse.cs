using Xunit;
using System;
using System.Collections.Generic;
using Registrar.Objects;
using System.Data;
using System.Data.SqlClient;

namespace  Registrar
{
  public class CourseTest : IDisposable
  {
    public CourseTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=roster_test;Integrated Security=SSPI;";
    }

    public DateTime testDate = new DateTime(2017, 1, 1);
    [Fact]
    public void GetAll_SavesMultipleCourses_true()
    {
      //Arrange
      List<Course> expectedCourses = new List<Course>{};
      Course course1 = new Course("Pie making", "Learn to make sweet and savory pies!");
      course1.Save();
      expectedCourses.Add(course1);
      Course course2 = new Course("Tea making", "Learn to make medicinal teas!");
      course2.Save();
      expectedCourses.Add(course2);

      //Act
      List<Course> actualCourses = Course.GetAll();
      //Assert
      Assert.Equal(expectedCourses, actualCourses);
    }

    [Fact]
    public void Test_Find_FindsCourseInDb()
    {
      Course newCourse = new Course("Anatomy and Physiology 201", "A&P Course designed to prepare students for a nursing program.");
      newCourse.Save();

      Course foundCourse = Course.Find(newCourse.Id);
      Assert.Equal(newCourse, foundCourse);
    }

    [Fact]
    public void Test_Edit_EditsCourseInDb()
    {
      Course newCourse = new Course("Anatomy and Physiology 201", "A&P Course designed to prepare students for a nursing program.");
      newCourse.Save();
      newCourse.Edit("Anatomy and Physiology 202", "This course is, like, way harder for realz.", 1);

      Course newerCourse = new Course("Anatomy and Physiology 202", "This course is, like, way harder for realz.", 1);

      Assert.Equal(newCourse, newerCourse);
    }

    [Fact]
    public void Test_Add_AddsStudentToCourse()
    {
      Course newCourse = new Course("Anatomy and Physiology 201", "A&P Course designed to prepare students for a nursing program.");
      newCourse.Save();
      Student newStudent = new Student("Jo Bob", testDate);
      newStudent.Save();

      newCourse.AddStudent(newStudent);

      List<Student> foundStudentList = newCourse.GetStudents();
      Student foundStudent = foundStudentList[0];
      Assert.Equal(newStudent, foundStudent);
    }
    [Fact]
    public void Major_GetsCoursesInMajor_true()
    {
      Course newCourse = new Course("Anatomy and Physiology 201", "A&P Course designed to prepare students for a nursing program.", 0);
      newCourse.Save();
      Major major = new Major();
      List<Course> expectedCourses = new List<Course>{newCourse};

      List<Course> courses = major.GetCourses();

      Assert.Equal(expectedCourses, courses);
    }
    [Fact]
    public void Major_GetsStudentsInMajor_true()
    {
      Student newStudent = new Student("Jo Bob", testDate, 0);
      newStudent.Save();
      Major major = new Major();
      List<Student> expectedStudents = new List<Student>{newStudent};

      List<Student> students = major.GetStudents();

      Assert.Equal(expectedStudents, students);
    }
    public void Dispose()
    {
      Course.DeleteAll();
      Student.DeleteAll();
    }

  }
}
