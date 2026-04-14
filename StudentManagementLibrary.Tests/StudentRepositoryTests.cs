using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Assertions;
using MiniTestFramework.Attributes;
using MiniTestFramework.Attributes.ClassAttributes;
using MiniTestFramework.Attributes.MethodAttributes;


namespace StudentManagementLibrary.Tests
{
    [TestClassifierClass(Name = "Student repository", IsShared = false)]
    public class StudentRepositoryTests
    {
        private StudentRepository? _repository;
        private Student?_student;

        [BeforeEach]
        public void Setup()
        {
            _repository = new StudentRepository();
            _student = new Student("Иванов Иван", 20);
            _repository.AddStudent(_student);
        }

        [AfterEach]
        public void Cleanup()
        {
            _repository = null;
            _student = null;
        }

        
        [TestClassifierMethod(priority: 1, name:"Student is added")]
        public void AddStudent_IncreasesCount()
        {
            Thread.Sleep(6000);
            _repository?.AddStudent(new Student("Петров Петр", 22));
            Assert.AreEqual(2, _repository?.Students.Count);
        }

        [TestClassifierMethod(priority: 1, name: "Student is removed", timeout: 1000)]
        
        public void RemoveStudent_Removes()
        {
            Thread.Sleep(500);
            _repository?.RemoveStudent(_student?.Id);
            Assert.AreEqual(0, _repository?.Students.Count);
        }

        [TestClassifierMethod(priority: 1, name: "Student to remove not found", timeout: 1000)]
        public void RemoveStudent_NotFound_Throws()
        {
            Thread.Sleep(500);
            Assert.Throws<InvalidOperationException>(() =>
                _repository?.RemoveStudent(Guid.NewGuid()));
        }

        [TestClassifierMethod(priority: 1, name: "Student is found", timeout: 1000)]
        [TestDataMethod("Иванов Иван")]
        public void FindStudent_ReturnsStudent(String name)
        {
            Thread.Sleep(500);
            object? found = _repository?.FindByName(name);
            Assert.IsNotNull(found);
        }

        [TestClassifierMethod(priority: 1, name: "Student grade is updated", timeout: 1000)]
        [TestDataMethod(9)]
        public void UpdateGrade_Works(int grade)
        {
            Thread.Sleep(500);
            _student?.UpdateAverageGrade(grade);
       
            Assert.Greater(_student?.AverageGrade, grade - 1);
            Assert.Less(_student?.AverageGrade, grade + 1);
        }

        [TestClassifierMethod(priority: 1, name: "Student grade is invalid", description: "Grade could be in range", timeout: 1000)]
        [TestDataMethod(20)]
        public void UpdateGrade_Invalid_Throws(int invalidGrade)
        {
            Thread.Sleep(500);
            Assert.Throws<ArgumentException>(() =>
                _student?.UpdateAverageGrade(invalidGrade));
        }

        [TestClassifierMethod(priority: 0, name: "Student count get", timeout: 100)]
        public async Task GetStudentsCountAsync_Works()
        {
            Thread.Sleep(500);
            int count = await (_repository?.GetStudentsCountAsync() ?? Task.FromResult(0));
            Assert.AreEqual(1, count);
        }

        [TestClassifierMethod(priority: 2, name: "Student age is invalid", timeout: 200)]
        public void Student_InvalidAge_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Student("Test", 10));
        }

        [TestClassifierMethod(priority: 2, name: "Student empty name is cancelled", timeout: 500)]
        [TestDataMethod("")]
        public void Student_EmptyName_Throws(string name)
        {
            Thread.Sleep(500);
            Assert.Throws<ArgumentException>(() =>
                new Student(name, 18));
        }
    }
}
