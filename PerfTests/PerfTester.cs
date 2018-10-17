using System;
using System.Diagnostics;
using System.IO;

namespace PerfTests
{
  public static class PerfTester
  {
    public static void TestMethod(Action func1, Action func2, int iterations, TextWriter output)
    {
      TestMethodDisp((iter) => TestMethodImpl(func1, func2, iter), iterations, output);
    }

    public static void TestMethod<T>(Func<T> func1, Func<T> func2, int iterations, TextWriter output)
    {
      TestMethodDisp((iter) => TestMethodImpl(func1, func2, iter), iterations, output);
    }

    private static void TestMethodDisp(Func<int, Statistics> fun, int iterationsCount, TextWriter output)
    {
#if DEBUG
      output.WriteLine("Build mode: Debug");
#else
      output.WriteLine("Build mode: Release");
#endif

      Statistics stat;
      //Tuple<long, long, long, long> times;
      //long time1ms;
      //long time2ms;
      //long mem1;
      //long mem2;
      output.Write("iterations".PadLeft(iterationsCount.ToString().Length, ' '));
      output.Write(':');
      output.Write('\t');
      output.Write("func1");
      output.Write('\t');
      output.Write("func2");
      output.Write('\t');
      output.Write("func2/func1");
      output.Write('\t');
      output.Write("diff");
      output.Write('\t');
      output.Write(nameof(stat.gen0gc1));
      output.Write('\t');
      output.Write(nameof(stat.gen0gc2));
      output.Write('\t');
      output.Write(nameof(stat.gen1gc1));
      output.Write('\t');
      output.Write(nameof(stat.gen1gc2));
      output.WriteLine();

      int currentNumberOfIterations = 1;
      while (currentNumberOfIterations <= iterationsCount)
      {
        stat = fun(currentNumberOfIterations);
        output.Write(currentNumberOfIterations.ToString().PadLeft(iterationsCount.ToString().Length, ' '));
        output.Write(':');
        output.Write('\t');
        output.Write(stat.time1ms);
        output.Write('\t');
        output.Write(stat.time2ms);
        output.Write('\t');
        output.Write((stat.time1ms == 0 ? 0 : (double)stat.time2ms / stat.time1ms).ToString("0.##"));
        output.Write('\t');
        output.Write('\t');
        output.Write((stat.time1ms == 0 ? 0 : ((double)stat.time2ms - (double)stat.time1ms) / (double)stat.time2ms).ToString("0%"));
        output.Write('\t');
        output.Write(stat.gen0gc1);
        output.Write('\t');
        output.Write(stat.gen0gc2);
        output.Write('\t');
        output.Write(stat.gen1gc1);
        output.Write('\t');
        output.Write(stat.gen1gc2);
        output.WriteLine();
        currentNumberOfIterations = currentNumberOfIterations * 10;
      }
    }
    private static Statistics TestMethodImpl<T>(Func<T> func1, Func<T> func2, int iterations)
    {
      Stopwatch sw = new Stopwatch();
      T result = default(T);
      var stat = new Statistics();
      int WARM_UP_ITERATIONS = iterations / 10;

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        result = func1();
      }
      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        result = func2();
      }

      var memBefore0 = GC.CollectionCount(0);
      var memBefore1 = GC.CollectionCount(1);

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        result = func1();
      }
      sw.Stop();
      var memAfter0 = GC.CollectionCount(0);
      var memAfter1 = GC.CollectionCount(1);

      stat.time1ms = sw.ElapsedMilliseconds;
      stat.gen0gc1 = memAfter0 - memBefore0;
      stat.gen1gc1 = memAfter1 - memBefore1;

      memBefore0 = GC.CollectionCount(0);
      memBefore1 = GC.CollectionCount(1);

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        result = func2();
      }
      sw.Stop();
      memAfter0 = GC.CollectionCount(0);
      memAfter1 = GC.CollectionCount(1);

      stat.time2ms = sw.ElapsedMilliseconds;
      stat.gen0gc2 = memAfter0 - memBefore0;
      stat.gen1gc2 = memAfter1 - memBefore1;

      GC.KeepAlive(result);

      return stat;
    }

    private static Statistics TestMethodImpl(Action func1, Action func2, int iterations)
    {
      Stopwatch sw = new Stopwatch();
      var stat = new Statistics();
      int WARM_UP_ITERATIONS = iterations / 10;

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        func1();
      }
      for (int i = 0; i < WARM_UP_ITERATIONS; i++)
      {
        func2();
      }

      var memBefore0 = GC.CollectionCount(0);
      var memBefore1 = GC.CollectionCount(1);

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        func1();
      }
      sw.Stop();
      var memAfter0 = GC.CollectionCount(0);
      var memAfter1 = GC.CollectionCount(1);

      stat.time1ms = sw.ElapsedMilliseconds;
      stat.gen0gc1 = memAfter0 - memBefore0;
      stat.gen1gc1 = memAfter1 - memBefore1;

      memBefore0 = GC.CollectionCount(0);
      memBefore1 = GC.CollectionCount(1);

      sw.Restart();
      for (int i = 0; i < iterations; i++)
      {
        func2();
      }
      sw.Stop();
      memAfter0 = GC.CollectionCount(0);
      memAfter1 = GC.CollectionCount(1);

      stat.time2ms = sw.ElapsedMilliseconds;
      stat.gen0gc2 = memAfter0 - memBefore0;
      stat.gen1gc2 = memAfter1 - memBefore1;

      return stat;
    }
  }
}
