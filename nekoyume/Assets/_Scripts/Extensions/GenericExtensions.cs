#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;

namespace Nekoyume
{
    public static class GenericExtensions
    {
        public static void DisposeAllAndClear<T>(this List<T> list) where T : IDisposable
        {
            foreach (var item in list)
            {
                item.Dispose();
            }

            list.Clear();
        }
    }
}

#endif
