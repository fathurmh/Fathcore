using System.Collections.Generic;
using System.Linq;

namespace Fathcore.Tests.Fakes
{
    internal class FakeEntityGenerator
    {
        public static List<Address> AddressesOnly
            => new List<Address>()
            {
                new Address() { Street = "Underwater Street" },
                new Address() { Street = "Not My Church Street" },
                new Address() { Street = "Maple Syrup Street" },
                new Address() { Street = "Dip Wick Drive" },
                new Address() { Street = "Bearded Clam Passage" },
            };

        public static List<Student> StudentsOnly
            => new List<Student>()
            {
                new Student() { Name = "John Doe" },
                new Student() { Name = "Jane Doe" },
                new Student() { Name = "Fathcore" },
                new Student() { Name = "Fulan" },
                new Student() { Name = "Fulana" },
            };

        public static List<Classroom> ClassroomsOnly
            => new List<Classroom>()
            {
                new Classroom() { Code = "01-A" },
                new Classroom() { Code = "01-B" },
                new Classroom() { Code = "02-A" },
                new Classroom() { Code = "02-A1" },
                new Classroom() { Code = "02-B" },
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
