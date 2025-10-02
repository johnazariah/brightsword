using System;
using System.Threading;
using BrightSword.SwissKnife.Properties;

namespace BrightSword.SwissKnife
{
    public static class SequentialGuid
    {
        private static int _a;
        private static short _b;
        private static short _c;
        private static long _forwardCounter;
        private static long _reverseCounter = DateTime.MaxValue.Ticks;
    private static readonly SemaphoreSlim _lock = new(1);

        private static void ResetForwardCounter()
        {
            _lock.Wait();
            try
            {
                _forwardCounter = Math.Max(DateTime.Now.Ticks, _forwardCounter) + 1;
            }
            finally
            {
                _lock.Release();
            }
        }

        private static void ResetReverseCounter()
        {
            _lock.Wait();
            try
            {
                var ticks1 = DateTime.MaxValue.Ticks;
                var ticks2 = DateTime.Now.Ticks;
                _reverseCounter = Math.Min(ticks1 - ticks2, _reverseCounter) - 1;
            }
            finally
            {
                _lock.Release();
            }
        }

        private static void ResetCounters()
        {
            ResetForwardCounter();
            ResetReverseCounter();
        }

        static SequentialGuid()
        {
            InitializeFromSettings();
            _ = new Timer(_ => ResetCounters(), null, 0, 1000);
            ResetForwardCounter();
            ResetReverseCounter();
        }

        public static Guid NewSequentialGuid(int a = -1, short b = -1, short c = -1)
        {
            _lock.Wait();
            try
            {
                _ = Interlocked.Increment(ref _forwardCounter);
                return new Guid(a == -1 ? _a : a, b == -1 ? _b : b, c == -1 ? _c : c, _forwardCounter.GetReversedBytes());
            }
            finally
            {
                _lock.Release();
            }
        }

        public static Guid NewReverseSequentialGuid(int a = -1, short b = -1, short c = -1)
        {
            _lock.Wait();
            try
            {
                _ = Interlocked.Decrement(ref _reverseCounter);
                return new Guid(a == -1 ? _a : a, b == -1 ? _b : b, c == -1 ? _c : c, _reverseCounter.GetReversedBytes());
            }
            finally
            {
                _ = _lock.Release();
            }
        }

        private static void SaveSettings()
        {
            Settings.Default.Realm_UniqueID = (uint)_a;
            Settings.Default.Server_UniqueID = (ushort)_b;
            Settings.Default.Application_UniqueID = (ushort)_c;
            Settings.Default.Save();
        }

        private static void InitializeFromSettings()
        {
            _lock.Wait();
            try
            {
                var random = new Random();
                _a = (int)Settings.Default.Realm_UniqueID;
                while (_a == 0)
                {
                    _a = random.Next();
                }

                _b = (short)Settings.Default.Server_UniqueID;
                while (_b == 0)
                {
                    _b = (short)random.Next(short.MaxValue);
                }

                _c = (short)Settings.Default.Application_UniqueID;
                while (_c == 0)
                {
                    _c = (short)random.Next(short.MaxValue);
                }

                SaveSettings();
            }
            finally
            {
                _ = _lock.Release();
            }
        }

        public static void Initialize(uint a, ushort b, ushort c)
        {
            _lock.Wait();
            try
            {
                _a = (int)a;
                _b = (short)b;
                _c = (short)c;
                SaveSettings();
            }
            finally
            {
                _ = _lock.Release();
            }
        }
    }
}
