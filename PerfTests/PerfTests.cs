using System;
using System.Diagnostics;
using System.IO;

namespace PerfTestsTools
{
  public static class PerfTests
  {
    const int WARM_UP_ITERATIONS = 1000000;

    public static void TestMethod(Action func1, Action func2, int iterations, TextWriter output)
    {
      TestMethodDisp((iter) => TestMethodImpl(func1, func2, iter), iterations, output);
    }

    public static void TestMethod<T>(Func<T> func1, Func<T> func2, int iterations, TextWriter output)
    {
      TestMethodDisp((iter) => TestMethodImpl(func1, func2, iter), iterations, output);
    }

    private static void TestMethodDisp(Func<int, Tuple<long, long>> fun, int iterationsCount, TextWriter output)
    {
#if DEBUG
      output.WriteLine("Build mode: Debug");
#else
      output.WriteLine("Build mode: Release");
#endif

      Tuple<long, long> times;
      long time1ms;
      long time2ms;
      output.Write("iterations".PadLeft(iterationsCount.ToString().Length, ' '));
      output.Write(':');
      output.Write('\t');
      output.Write("func1");
      output.Write('\t');
      output.Write("func2");
      output.Write('\t');
      output.WriteLine("func2/func1");

      int currentNumberOfIterations = iterationsCount;
      while (currentNumberOfIterations > 0)
      {
        times = fun(currentNumberOfIterations);
        time1ms = times.Item1;
        time2ms = times.Item2;
        output.Write(currentNumberOfIterations.ToString().PadLeft(iterationsCount.ToString().Length, ' '));
        output.Write(':');
        output.Write('\t');
        output.Write(time1ms);
        output.Write('\t');
        output.Write(time2ms);
        output.Write('\t');
        output.WriteLine((time1ms == 0 ? 0 : (double)time2ms / time1ms).ToString("0.##"));
        currentNumberOfIterations = currentNumberOfIterations / 10;
      }
    }
    private static Tuple<long, long> TestMethodImpl<T>(Func<T> func1, Func<T> func2, int iterations)
    {
      Stopwatch sw = new Stopwatch();
      T result;
      long time1ms = 0;
      long time2ms = 0;

      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        result = func1();
      }
      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        result = func2();
      }

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        result = func1();
      }
      sw.Stop();

      time1ms = sw.ElapsedMilliseconds;

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        result = func2();
      }
      sw.Stop();

      time2ms = sw.ElapsedMilliseconds;

      return new Tuple<long, long>(time1ms, time2ms);
    }

    private static Tuple<long, long> TestMethodImpl(Action func1, Action func2, int iterations)
    {
      Stopwatch sw = new Stopwatch();
      long time1ms = 0;
      long time2ms = 0;

      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        func1();
      }
      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        func2();
      }

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        func1();
      }
      sw.Stop();

      time1ms = sw.ElapsedMilliseconds;

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        func2();
      }
      sw.Stop();

      time2ms = sw.ElapsedMilliseconds;

      return new Tuple<long, long>(time1ms, time2ms);
    }
  }
}
