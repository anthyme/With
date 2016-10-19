using System;
using Shouldly;
using Xunit;

namespace With.Tests
{
    public class MutableTest
    {
        [Fact]
        public void WhenMutableWithShouldCreateACopyWithANewValue()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };

            var result = mutable.WithM(x => x.Name, "new name");

            result.ShouldNotBe(mutable);
            result.Id.ShouldBe(mutable.Id);
            result.Date.ShouldBe(mutable.Date);
            result.Name.ShouldBe("new name");
        }

        private class Mutable
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
