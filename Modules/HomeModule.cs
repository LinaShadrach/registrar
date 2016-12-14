using Nancy;
using System.Collections.Generic;
using System;
using Registrar.Objects;

namespace Registrar
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        return View["index.cshtml"];
      };
      Get["/courses"] = _ => {
        List<Course> allCourses = Course.GetAll();
        return View["courses.cshtml", allCourses];
      };
      Get["/majors"] = _ => {
        List<Major> allMajors = Major.GetAll();
        return View["majors.cshtml", allMajors];
      };
      Get["/students"] = _ => {
        List<Student> allStudents = Student.GetAll();
        return View["students.cshtml", allStudents];
      };
      Get["/students/form"] = _ => {
        List<Major> allMajors = Major.GetAll();
        return View["student_form.cshtml", allMajors];
      };
      Post["/student/form/submit"] = _ => {
        DateTime enrollmentDate = (DateTime) Request.Form["enrollment-date"];
        Student newStudent = new Student(Request.Form["student-name"], enrollmentDate, Request.Form["major-name"]);
        newStudent.Save();
        return View["index.cshtml"];
      };
      Get["/student/{id}"] = parameters => {
        Dictionary<string, object> myDict = new Dictionary<string, object>();
        Student foundStudent = Student.Find(parameters.id);
        string majorName = (Major.Find(foundStudent.MajorId)).Name;
        List<Major> allMajors = Major.GetAll();
        myDict.Add("student", foundStudent);
        myDict.Add("majors", allMajors);
        myDict.Add("majorName", majorName);
        return View["student_details.cshtml", myDict];
      };
      Patch["/student/form/edit/{id}"] = parameters =>{
        Dictionary<string, object> myDict = new Dictionary<string, object>();
        Student foundStudent = Student.Find(parameters.id);
        foundStudent.Edit(Request.Form["student-name"], Request.Form["enrollment-date"], Request.Form["major-name"]);
        string majorName = (Major.Find(foundStudent.MajorId)).Name;
        List<Major> allMajors = Major.GetAll();
        myDict.Add("student", foundStudent);
        myDict.Add("majors", allMajors);
        myDict.Add("majorName", majorName);
        return View["student_details.cshtml", myDict];
      };
    }
  }
}
