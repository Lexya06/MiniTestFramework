using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementLibrary
{
    public class Student
    {
        public Guid Id { get; }
        public string FullName { get; }
        public int Age { get; }
        public double AverageGrade { get; private set; }

        public Student(string fullName, int age)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            if (age < 16 || age > 100)
                throw new ArgumentException("Invalid age");

            Id = Guid.NewGuid();
            FullName = fullName;
            Age = age;
        }

        public void UpdateAverageGrade(double grade)
        {
            if (grade < 0 || grade > 10)
                throw new ArgumentException("Grade must be between 0 and 10");
            AverageGrade = grade;
        }
    }

}
