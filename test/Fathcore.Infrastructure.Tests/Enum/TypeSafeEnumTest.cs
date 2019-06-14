using System.Collections.Generic;
using System.Linq;
using Fathcore.Infrastructure.Enum;
using Xunit;

namespace Fathcore.Infrastructure.Tests.Enum
{
    public class TypeSafeEnumTest
    {
        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 1, 3 })]
        public void TypeSafeEnum_PropertyCheck(Status status, short id, string name, string description, string customProperty)
        {
            Assert.Equal(id, status.Id);
            Assert.Equal(name, status.Name);
            Assert.Equal(description, status.Description);
            Assert.Equal(customProperty, status.CustomProperty);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 2, 3 })]
        public void TypeSafeEnum_PropertyCheck_CyclicCastToBaseType(Status status, short id, string name, string description)
        {
            var typeSafeEnum = (TypeSafeEnum<Status, short>)status;
            Assert.Equal(id, typeSafeEnum.Id);
            Assert.Equal(name, typeSafeEnum.Name);
            Assert.Equal(description, typeSafeEnum.Description);

            var typeSafeEnum2 = (ITypeSafeEnum<short>)typeSafeEnum;
            Assert.Equal(id, typeSafeEnum2.Id);
            Assert.Equal(name, typeSafeEnum2.Name);
            Assert.Equal(description, typeSafeEnum2.Description);

            var typeSafeEnum3 = (TypeSafeEnum<Status>)typeSafeEnum2;
            Assert.Equal(name, typeSafeEnum3.Name);
            Assert.Equal(description, typeSafeEnum3.Description);

            var typeSafeEnum4 = (TypeSafeEnum)typeSafeEnum2;
            Assert.Equal(name, typeSafeEnum4.Name);
            Assert.Equal(description, typeSafeEnum4.Description);

            var typeSafeEnum5 = (ITypeSafeEnum)typeSafeEnum4;
            Assert.Equal(name, typeSafeEnum5.Name);
            Assert.Equal(description, typeSafeEnum5.Description);

            var status2 = (Status)typeSafeEnum5;
            Assert.Equal(id, status2.Id);
            Assert.Equal(name, status2.Name);
            Assert.Equal(description, status2.Description);
            Assert.Equal(status, status2);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 3, 3 })]
        public void TypeSafeEnum_ToString_ShouldReturn_Name(Status status, string name)
        {
            Assert.Equal(name, status.ToString());
        }

        [Fact]
        public void TypeSafeEnum_GetNames_ShouldReturn_Names()
        {
            var names = Status.GetNames();
            Assert.Contains("Pending", names);
            Assert.Contains("Started", names);
            Assert.Contains("Finished", names);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 4, 4 })]
        public void TypeSafeEnum_GetValueById_ShouldReturn_Status(Status status, short id)
        {
            Assert.Equal(status, Status.GetValue(id));
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 3, 4 })]
        public void TypeSafeEnum_GetValueByName_ShouldReturn_Status(Status status, string name)
        {
            Assert.Equal(status, Status.GetValue(name));
        }

        [Fact]
        public void TypeSafeEnum_GetValues_ShouldReturn_Values()
        {
            var status = Status.GetValues();
            Assert.Contains(Status.Pending, status);
            Assert.Contains(Status.Started, status);
            Assert.Contains(Status.Finished, status);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 4, 3 })]
        public void TypeSafeEnum_FromValue_CastToId(Status status, short id)
        {
            Assert.Equal(id, (short)status);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 3, 3 })]
        public void TypeSafeEnum_FromValue_CastToString(Status status, string name)
        {
            Assert.Equal(name, (string)status);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 4, 4 })]
        public void TypeSafeEnum_FromId_CastToValue(Status status, short id)
        {
            Assert.Equal(status, (Status)id);
        }

        [Theory]
        [MemberData(nameof(Data), parameters: new object[] { 3, 4 })]
        public void TypeSafeEnum_FromName_CastToValue(Status status, string name)
        {
            Assert.Equal(status, (Status)name);
        }

        [Fact]
        public void TypeSafeEnum_CanCompared()
        {
            var statusPending = Status.Pending;
            var statusStarted = Status.Started;
            var statusFinished = Status.Finished;

            Assert.True(statusPending == Status.Pending);
            Assert.True(statusStarted == Status.Started);
            Assert.True(statusFinished == Status.Finished);

            Assert.True(statusPending.Equals(Status.Pending));
            Assert.True(statusStarted.Equals(Status.Started));
            Assert.True(statusFinished.Equals(Status.Finished));
        }

        public static IEnumerable<object[]> Data(int type, int take)
        {
            var list = new List<object[]>();
            switch (type)
            {
                case 1:
                {
                    list = new List<object[]>
                    {
                        new object[] { Status.Pending, 0, "Pending", "Pending State.", "Custom Pending." },
                        new object[] { Status.Started, 1, "Started", "Started State.", "Custom Started." },
                        new object[] { Status.Finished, 2, "Finished", "Finished State.", "Custom Finished." },
                    };
                }
                break;
                case 2:
                {
                    list = new List<object[]>
                    {
                        new object[] { Status.Pending, 0, "Pending", "Pending State." },
                        new object[] { Status.Started, 1, "Started", "Started State." },
                        new object[] { Status.Finished, 2, "Finished", "Finished State." },
                    };
                }
                break;
                case 3:
                {
                    list = new List<object[]>
                    {
                        new object[] { Status.Pending, "Pending" },
                        new object[] { Status.Started, "Started" },
                        new object[] { Status.Finished, "Finished" },
                        new object[] { null, "Finisheds" },
                    };
                }
                break;
                case 4:
                {
                    list = new List<object[]>
                    {
                        new object[] { Status.Pending, 0 },
                        new object[] { Status.Started, 1 },
                        new object[] { Status.Finished, 2 },
                        new object[] { null, 3 },
                    };
                }
                break;
                default:
                    break;
            }
            return list.Take(take);
        }

        public sealed class Status : TypeSafeEnum<Status, short>, ITypeSafeEnum<short>
        {
            public string CustomProperty { get; }
            public static Status Pending { get; } = new Status(0, "Pending", "Pending State.", "Custom Pending.");
            public static Status Started { get; } = new Status(1, "Started", "Started State.", "Custom Started.");
            public static Status Finished { get; } = new Status(2, "Finished", "Finished State.", "Custom Finished.");

            private Status(short id, string name, string description, string customProperty) : base(id, name, description)
            {
                CustomProperty = customProperty;
            }
        }
    }
}
