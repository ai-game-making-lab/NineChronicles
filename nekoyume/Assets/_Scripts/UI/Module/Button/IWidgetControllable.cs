#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.UI.Module
{
    public interface IWidgetControllable
    {
        bool IsWidgetControllable { get; set; }
        bool HasWidget { get; }
        void SetWidgetType<T>() where T : Widget;
        void ShowWidget(bool ignoreShowAnimation = false);
        void HideWidget(bool ignoreHideAnimation = false);
    }
}

#endif
