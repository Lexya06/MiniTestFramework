using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Assertions;
using MiniTestFramework.Attributes;


namespace StudentManagementLibrary.Tests
{
    [TestClass("Тестирование управления студентами")]
    [SharedContext]
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

        [TestMethod(priority: 1)]
        public void AddStudent_IncreasesCount()
        {
            _repository?.AddStudent(new Student("Петров Петр", 22));
            Assert.AreEqual(2, _repository?.Students.Count);
        }

        [TestMethod]
        public void RemoveStudent_Removes()
        {
            _repository?.RemoveStudent(_student?.Id);
            Assert.AreEqual(0, _repository?.Students.Count);
        }

        [TestMethod]
        public void RemoveStudent_NotFound_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _repository?.RemoveStudent(Guid.NewGuid()));
        }

        [TestMethod]
        public void FindStudent_ReturnsStudent()
        {
            object? found = _repository?.FindByName("Иванов Иван");
            Assert.IsNotNull(found);
        }

        [TestMethod]
        public void UpdateGrade_Works()
        {
            _student?.UpdateAverageGrade(9);
       
            Assert.Greater(_student?.AverageGrade, 8);
            Assert.Less(_student?.AverageGrade, 10);
        }

        [TestMethod]
        public void UpdateGrade_Invalid_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                _student?.UpdateAverageGrade(20));
        }

        [TestMethod]
        public async Task GetStudentsCountAsync_Works()
        {
            int count = await (_repository?.GetStudentsCountAsync() ?? Task.FromResult(0));
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void Student_InvalidAge_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Student("Test", 10));
        }

        [TestMethod]
        public void Student_EmptyName_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Student("", 18));
        }
    }
}
