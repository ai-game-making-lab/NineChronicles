#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.UI
{
    public class UINavigator
    {
#region Singleton

        private static class Singleton
        {
            public static readonly UINavigator Value = new();
        }

        public static UINavigator Instance => Singleton.Value;

#endregion

        public enum NavigationType
        {
            None,
            Back,
            Main,
            Exit,
            Quit
        }
    }
}

#endif
