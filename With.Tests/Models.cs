using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace With.Tests
{
    class Immutable
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

    class Mutable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    static class Settings
    {
        public const int IterationCount = 1000000;
    }
}
