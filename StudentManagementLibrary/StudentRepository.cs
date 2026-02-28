using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementLibrary
{
    public class StudentRepository
    {
        private readonly List<Student> _students = new();

        public IReadOnlyCollection<Student> Students => _students;

        public void AddStudent(Student? student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            _students.Add(student);
        }

        public void RemoveStudent(Guid? id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                throw new InvalidOperationException("Student not found");

            _students.Remove(student);
        }

        public Student? FindByName(string? name)
        {
            return _students.FirstOrDefault(s => s.FullName == name);
        }

        public async Task<int> GetStudentsCountAsync()
        {
            await Task.Delay(100);
            return _students.Count;
        }
    }
}
