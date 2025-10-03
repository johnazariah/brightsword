using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BrightSword.Squid.Behaviours
{
    public interface IBehaviour
    {
        IEnumerable<Func<TypeBuilder, TypeBuilder>> Operations { get; }
    }
}
