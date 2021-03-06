using System;
using System.Linq;
using Shouldly;
using With.NoCache;
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

        [Fact]
        public void WhenImmutableWithNoCacheShouldCreateACopyWithANewValue()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);

            var result = immutable.WithNoCache(x => x.Name, "new name");

            result.ShouldNotBe(immutable);
            result.Id.ShouldBe(immutable.Id);
            result.Date.ShouldBe(immutable.Date);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void WhenMutableWithNoCacheShouldCreateACopyWithMultipleValues()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);
            var newdate = immutable.Date.AddDays(1);

            var result = immutable.WithNoCache((x => x.Name, "new name"), (x => x.Date, newdate));

            result.ShouldNotBe(immutable);
            result.Id.ShouldBe(immutable.Id);
            result.Date.ShouldBe(newdate);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void BenchmarkNativeImmutable()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                new Immutable(immutable.Id, "new name", immutable.Date);
            }
        }

        [Fact]
        public void BenchmarkWithImmutable()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                immutable.With(x => x.Name, "new name");
            }
        }

        [Fact]
        public void BenchmarkWithNoCacheImmutable()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                immutable.WithNoCache(x => x.Name, "new name");
            }
        }
    }
}