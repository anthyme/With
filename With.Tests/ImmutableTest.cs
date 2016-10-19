using System;
using Shouldly;
using Xunit;

namespace With.Tests
{
    public class ImmutableTest
    {
        [Fact]
        public void WhenImmutableWithShouldCreateACopyWithANewValue()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);

            var result = immutable.With(x => x.Name, "new name");

            result.ShouldNotBe(immutable);
            result.Id.ShouldBe(immutable.Id);
            result.Date.ShouldBe(immutable.Date);
            result.Name.ShouldBe("new name");
        }

        private class Immutable
        {
            public Immutable(Guid id, string name, DateTime date)
            {
                Id = id;
                Name = name;
                Date = date;
            }
            public Guid Id { get; }
            public string Name { get; }
            public DateTime Date { get; }
        }
    }
}