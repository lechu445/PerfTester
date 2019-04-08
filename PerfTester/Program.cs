using BenchmarkDotNet.Attributes;
using BenchmarkDotNet;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace PerfTester
{
  public class Program
  {
    static void Main(string[] args)
    {
      BenchmarkDotNet.Running.BenchmarkRunner.Run<TestClass>(BenchmarkDotNet.Configs.DefaultConfig.Instance);
    }
  }
}