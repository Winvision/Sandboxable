using System;

namespace Sandboxable.Hyak.Common
{
    public interface ILazyCollection
    {
        bool IsInitialized
        {
            get;
        }
    }
}
