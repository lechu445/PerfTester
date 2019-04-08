using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PerfTester
{
  public class TestClass
  {
    public static bool EqualsIgnoreCase(string that, char other)
    {
      return that != null && that.Length == 1 && (that[0] == other || that[0] == char.ToUpperInvariant(other) || that[0] == char.ToLowerInvariant(other));
    }

    public static bool EqualsIgnoreCase(string that, string other)
    {
      return string.Equals(that, other, StringComparison.OrdinalIgnoreCase);
    }

    public string Left;
    public string RightString;
    public char RightChar;

    [Params("X -> X", "X -> x", "x -> X")]
    public string Input { get; set; }

    [IterationSetup]
    public void Setup()
    {
      this.Left = Input[0].ToString();
      this.RightString = Input[Input.Length - 1].ToString();
      this.RightChar = Input[Input.Length - 1];
    }

    [Benchmark]
    public void Char()
    {
      EqualsIgnoreCase(Left, RightChar);
    }

    [Benchmark]
    public void String()
    {
      EqualsIgnoreCase(Left, RightString);
    }
  }
}
