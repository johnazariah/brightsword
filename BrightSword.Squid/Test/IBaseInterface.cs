using System.Collections.Generic;

namespace BrightSword.Squid.Test
{
    public interface IBaseInterface
    {
        IList<decimal> BaseAmounts { get; }

        string HiddenProperty { get; set; }
    }
}
