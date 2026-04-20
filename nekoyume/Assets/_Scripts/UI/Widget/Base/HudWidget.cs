#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.EnumType;

namespace Nekoyume.UI
{
    public class HudWidget : Widget
    {
        public override WidgetType WidgetType => WidgetType.Hud;

        protected override void Awake()
        {
            base.Awake();

            CloseWidget = null;
        }

        public override void Close(bool ignoreCloseAnimation = false)
        {
            Destroy(gameObject);
        }
    }
}

#endif
