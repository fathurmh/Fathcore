using System;
using System.Collections.Generic;
using System.Linq;

namespace Fathcore.Tests.Fakes
{
    public class FakeEntities : List<FakeEntity>
    {
        public FakeEntities()
        {
            Add(new FakeEntity() { EntityCode = "Entity Code 1", EntityName = "Entity Name 1", FakeChildEntities = new FakeChildEntities().Skip(0).Take(3).ToList() });
            Add(new FakeEntity() { EntityCode = "Entity Code 2", EntityName = "Entity Name 2", FakeChildEntities = new FakeChildEntities().Skip(3).Take(2).ToList() });
            Add(new FakeEntity() { EntityCode = "Entity Code 3", EntityName = "Entity Name 3", FakeChildEntities = new FakeChildEntities().Skip(5).Take(1).ToList() });
            Add(new FakeEntity() { EntityCode = "Entity Code 4", EntityName = "Entity Name 4", FakeChildEntities = new FakeChildEntities().Skip(6).Take(1).ToList() });
            Add(new FakeEntity() { EntityCode = "Entity Code 5", EntityName = "Entity Name 5", FakeChildEntities = new FakeChildEntities().Skip(7).Take(1).ToList() });
        }
    }
    
    public class FakeChildEntities : List<FakeChildEntity>
    {
        public FakeChildEntities()
        {
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 1", ChildEntityName = "Child Entity Name 1" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 2", ChildEntityName = "Child Entity Name 2" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 3", ChildEntityName = "Child Entity Name 3" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 4", ChildEntityName = "Child Entity Name 4" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 5", ChildEntityName = "Child Entity Name 5" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 6", ChildEntityName = "Child Entity Name 6" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 7", ChildEntityName = "Child Entity Name 7" });
            Add(new FakeChildEntity() {ChildEntityCode = "Child Entity Code 8", ChildEntityName = "Child Entity Name 8" });
        }
    }
}
