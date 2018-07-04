using System;
using System.Globalization;

namespace PerfTester
{
  class Program
  {
    static void Main(string[] args)
    {
      const string pattern = "yyyy-MM-dd HH:mm:ss";
      const string dt = "2017-02-14 22:24:12";
      var culture = CultureInfo.InvariantCulture;

      PerfTests.PerfTester.TestMethod(
        func1: () => DateTime.Parse(dt, culture),
        func2: () => DateTime.ParseExact(dt, pattern, culture),
        iterations: 1000000,
        output: Console.Out);

      Console.ReadKey();
    }
  }
}