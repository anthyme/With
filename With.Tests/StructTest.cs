﻿using Shouldly;
using System;
using System.Linq;
using With.NoCache;
using Xunit;

namespace With.Tests
{
    public class StructTest
    {
        //[Fact]
        //public void WhenImmutableWithShouldCreateACopyWithANewValue()
        //{
        //    var immutable = new StructImmutable(Guid.NewGuid(), "name", DateTime.Now);

        //    var result = immutable.With(x => x.Name, "new name");

        //    result.ShouldNotBe(immutable);
        //    result.Id.ShouldBe(immutable.Id);
        //    result.Date.ShouldBe(immutable.Date);
        //    result.Name.ShouldBe("new name");
        //}

        [Fact]
        public void WhenImmutableWithNoCacheShouldCreateACopyWithANewValue()
        {
            var immutable = new StructImmutable(Guid.NewGuid(), "name", DateTime.Now);

            var result = immutable.WithNoCache(x => x.Name, "new name");

            result.ShouldNotBe(immutable);
            result.Id.ShouldBe(immutable.Id);
            result.Date.ShouldBe(immutable.Date);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void BenchmarkNativeImmutable()
        {
            var immutable = new Immutable(Guid.NewGuid(), "name", DateTime.Now);
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                new StructImmutable(immutable.Id, "new name", immutable.Date);
            }
        }

        //[Fact]
        //public void BenchmarkWithImmutable()
        //{
        //    var immutable = new StructImmutable(Guid.NewGuid(), "name", DateTime.Now);
        //    foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
        //    {
        //        immutable.With(x => x.Name, "new name");
        //    }
        //}

        [Fact]
        public void BenchmarkWithNoCacheImmutable()
        {
            var immutable = new StructImmutable(Guid.NewGuid(), "name", DateTime.Now);
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                immutable.WithNoCache(x => x.Name, "new name");
            }
        }
    }
}