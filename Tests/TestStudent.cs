using Xunit;
using System;
using System.Collections.Generic;
using Registrar.Objects;
using System.Data;
using System.Data.SqlClient;

namespace  Registrar
{
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=roster_test;Integrated Security=SSPI;";
    }
    public DateTime testDate = new DateTime(2017, 1, 1);
    [Fact]
    public void GetAll_SavesMultipleStudents_true()
    {
      //Arrange
      List<Student> expectedStudents = new List<Student>{};
      Student student1 = new Student("Jo Bob", testDate);
      student1.Save();
      expectedStudents.Add(student1);
      Student student2 = new Student("Irena", testDate);
      student2.Save();
      expectedStudents.Add(student2);

      //Act
      List<Student> actualStudents = Student.GetAll();
      //Assert
      Assert.Equal(expectedStudents, actualStudents);
    }

    [Fact]
    public void Test_Find_FindsStudentInDb()
    {
      Student newStudent = new Student("Jo Bob", testDate);
      newStudent.Save();

      Student foundStudent = Student.Find(newStudent.Id);
      Assert.Equal(newStudent, foundStudent);
    }

    [Fact]
    public void Test_Edit_EditsStudentInDb()
    {
      Student newStudent = new Student("Jo Bob", testDate);
      newStudent.Save();
      newStudent.Edit("Jim Bob", testDate, 1);

      Student newerStudent = new Student("Jim Bob", testDate);

      Assert.Equal(newStudent, newerStudent);
    }
    [Fact]
    public void Add_AddsCourseToStudent()
    {
      Course newCourse = new Course("Anatomy and Physiology 201", "A&P Course designed to prepare students for a nursing program.");
      newCourse.Save();
      Student newStudent = new Student("Jo Bob", testDate);
      newStudent.Save();

      newStudent.AddCourse(newCourse);

      List<Course> foundCourseList = newStudent.GetCourses();
      Course foundCourse = foundCourseList[0];
      Assert.Equal(newCourse, foundCourse);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();

    }

  }
}
