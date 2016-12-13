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
      newStudent.Edit("Jim Bob", testDate);

      Student newerStudent = new Student("Jim Bob", testDate);

      Assert.Equal(newStudent, newerStudent);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();

    }

  }
}
