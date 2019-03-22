using System.Collections.Generic;
using System.Linq;

namespace Fathcore.Tests.Fakes
{
    internal class FakeEntityGenerator
    {
        public static List<Address> AddressesOnly
            => new List<Address>()
            {
                new Address() { Street = "First Book Title" },
                new Address() { Street = "Second Book Title" },
                new Address() { Street = "Third Book Title" },
                new Address() { Street = "Fourth Book Title" },
                new Address() { Street = "Fifth Book Title" },
            };

        public static List<Student> StudentsOnly
            => new List<Student>()
            {
                new Student() { Name = "First Book" },
                new Student() { Name = "Second Book" },
                new Student() { Name = "Third Book" },
                new Student() { Name = "Fourth Book" },
                new Student() { Name = "Fifth Book" },
            };

        public static List<Classroom> ClassroomsOnly
            => new List<Classroom>()
            {
                new Classroom() { Code = "First Author" },
                new Classroom() { Code = "Second Author" },
                new Classroom() { Code = "Third Author" },
                new Classroom() { Code = "Fourth Author" },
                new Classroom() { Code = "Fifth Author" },
            };

        public static List<Address> Addresses
            => new List<Address>()
            {
                new Address() { Street = AddressesOnly[0].Street, Student = StudentsOnly[0] },
                new Address() { Street = AddressesOnly[1].Street, Student = StudentsOnly[1] },
                new Address() { Street = AddressesOnly[2].Street, Student = StudentsOnly[2] },
                new Address() { Street = AddressesOnly[3].Street, Student = StudentsOnly[3] },
                new Address() { Street = AddressesOnly[4].Street, Student = StudentsOnly[4] }
            };

        public static List<Student> Students
            => new List<Student>()
            {
                new Student() { Name = StudentsOnly[0].Name, Address = AddressesOnly[0] },
                new Student() { Name = StudentsOnly[1].Name, Address = AddressesOnly[1] },
                new Student() { Name = StudentsOnly[2].Name, Address = AddressesOnly[2] },
                new Student() { Name = StudentsOnly[3].Name, Address = AddressesOnly[3] },
                new Student() { Name = StudentsOnly[4].Name, Address = AddressesOnly[4] }
            };

        public static List<Classroom> Classrooms
            => new List<Classroom>()
            {
                new Classroom() { Code = ClassroomsOnly[0].Code, Students = Students.Skip(0).Take(2).ToList() },
                new Classroom() { Code = ClassroomsOnly[1].Code, Students = Students.Skip(1).Take(3).ToList() },
                new Classroom() { Code = ClassroomsOnly[2].Code, Students = Students.Skip(3).Take(2).ToList() },
                new Classroom() { Code = ClassroomsOnly[3].Code, Students = Students.Skip(0).Take(3).ToList() },
                new Classroom() { Code = ClassroomsOnly[4].Code, Students = Students.Skip(2).Take(3).ToList() },
            };
    }
}
