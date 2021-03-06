﻿using System;
using System.Linq;
using Shouldly;
using With.NoCache;
using Xunit;

namespace With.Tests
{
    public class MutableTest
    {
        [Fact]
        public void WhenMutableWithShouldCreateACopyWithANewValue()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };

            var result = mutable.With(x => x.Name, "new name");

            result.ShouldNotBe(mutable);
            result.Id.ShouldBe(mutable.Id);
            result.Date.ShouldBe(mutable.Date);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void WhenMutableWithNoCacheShouldCreateACopyWithANewValue()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };

            var result = mutable.WithNoCache(x => x.Name, "new name");

            result.ShouldNotBe(mutable);
            result.Id.ShouldBe(mutable.Id);
            result.Date.ShouldBe(mutable.Date);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void WhenMutableWithNoCacheShouldCreateACopyWithMultipleValues()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };
            var newdate = mutable.Date.AddDays(1);

            var result = mutable.WithNoCache((x => x.Name, "new name"), (x => x.Date, newdate));

            result.ShouldNotBe(mutable);
            result.Id.ShouldBe(mutable.Id);
            result.Date.ShouldBe(newdate);
            result.Name.ShouldBe("new name");
        }

        [Fact]
        public void BenchmarkNativeMutable()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                new Mutable { Id = mutable.Id, Name = "new name", Date = mutable.Date };
            }
        }

        [Fact]
        public void BenchmarkWithMutable()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                mutable.With(x => x.Name, "new name");
            }
        }

        [Fact]
        public void BenchmarkWithNoCacheMutable()
        {
            var mutable = new Mutable { Id = Guid.NewGuid(), Name = "name", Date = DateTime.Now };
            foreach (var _ in Enumerable.Range(0, Settings.IterationCount))
            {
                mutable.With(x => x.Name, "new name");
            }
        }
    }
}
