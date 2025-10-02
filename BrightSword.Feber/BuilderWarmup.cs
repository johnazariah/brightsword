using System;
using System.Collections.Generic;

namespace BrightSword.Feber
{
    /// <summary>
    /// Small helper to warm up compiled delegates produced by builders.
    /// Call Warmup with a collection of actions that access the builder's compiled delegate to force compilation on startup.
    /// </summary>
    public static class BuilderWarmup
    {
        /// <summary>
        /// Execute each warmup action (wrap in try/catch if you don't want exceptions to bubble).
        /// </summary>
        public static void Warmup(IEnumerable<Action> warmupActions)
        {
            foreach (var a in warmupActions)
            {
                try
                {
                    a();
                }
                catch
                {
                    // Swallow exceptions by default — warmup should not crash startup if a builder requires runtime-only context.
                }
            }
        }
    }
}
