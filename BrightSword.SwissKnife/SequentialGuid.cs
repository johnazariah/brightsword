using System;
using System.Threading;
using BrightSword.SwissKnife.Properties;

namespace BrightSword.SwissKnife;

public static class SequentialGuid
{
    private static int _a;
    private static short _b;
    private static short _c;
    private static long _forwardCounter;
    private static long _reverseCounter = DateTime.MaxValue.Ticks;
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

    private static void ResetForwardCounter()
    {
        SequentialGuid._lock.Wait();
        try
        {
            SequentialGuid._forwardCounter = Math.Max(DateTime.Now.Ticks, SequentialGuid._forwardCounter) + 1L;
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }

    private static void ResetReverseCounter()
    {
        SequentialGuid._lock.Wait();
        try
        {
            DateTime dateTime = DateTime.MaxValue;
            long ticks1 = dateTime.Ticks;
            dateTime = DateTime.Now;
            long ticks2 = dateTime.Ticks;
            SequentialGuid._reverseCounter = Math.Min(ticks1 - ticks2, SequentialGuid._reverseCounter) - 1L;
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }

    private static void ResetCounters(object arg = null)
    {
        SequentialGuid.ResetForwardCounter();
        SequentialGuid.ResetReverseCounter();
    }

    static SequentialGuid()
    {
        SequentialGuid.InitializeFromSettings();
        Timer timer = new Timer(new TimerCallback(SequentialGuid.ResetCounters), (object)null, 0, 1000);
        SequentialGuid.ResetForwardCounter();
        SequentialGuid.ResetReverseCounter();
    }

    public static Guid NewSequentialGuid(int a = -1, short b = -1, short c = -1)
    {
        SequentialGuid._lock.Wait();
        try
        {
            Interlocked.Increment(ref SequentialGuid._forwardCounter);
            return new Guid(a == -1 ? SequentialGuid._a : a, b == (short)-1 ? SequentialGuid._b : b, c == (short)-1 ? SequentialGuid._c : c, SequentialGuid._forwardCounter.GetReversedBytes());
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }

    public static Guid NewReverseSequentialGuid(int a = -1, short b = -1, short c = -1)
    {
        SequentialGuid._lock.Wait();
        try
        {
            Interlocked.Decrement(ref SequentialGuid._reverseCounter);
            return new Guid(a == -1 ? SequentialGuid._a : a, b == (short)-1 ? SequentialGuid._b : b, c == (short)-1 ? SequentialGuid._c : c, SequentialGuid._reverseCounter.GetReversedBytes());
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }

    private static void SaveSettings()
    {
        Settings.Default.Realm_UniqueID = (uint)SequentialGuid._a;
        Settings.Default.Server_UniqueID = (ushort)SequentialGuid._b;
        Settings.Default.Application_UniqueID = (ushort)SequentialGuid._c;
        Settings.Default.Save();
    }

    private static void InitializeFromSettings()
    {
        SequentialGuid._lock.Wait();
        try
        {
            Random random = new Random();
            SequentialGuid._a = (int)Settings.Default.Realm_UniqueID;
            while (SequentialGuid._a == 0)
                SequentialGuid._a = random.Next();
            SequentialGuid._b = (short)Settings.Default.Server_UniqueID;
            while (SequentialGuid._b == (short)0)
                SequentialGuid._b = (short)random.Next((int)short.MaxValue);
            SequentialGuid._c = (short)Settings.Default.Application_UniqueID;
            while (SequentialGuid._c == (short)0)
                SequentialGuid._c = (short)random.Next((int)short.MaxValue);
            SequentialGuid.SaveSettings();
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }

    public static void Initialize(uint a, ushort b, ushort c)
    {
        SequentialGuid._lock.Wait();
        try
        {
            SequentialGuid._a = (int)a;
            SequentialGuid._b = (short)b;
            SequentialGuid._c = (short)c;
            SequentialGuid.SaveSettings();
        }
        finally
        {
            SequentialGuid._lock.Release();
        }
    }
}
