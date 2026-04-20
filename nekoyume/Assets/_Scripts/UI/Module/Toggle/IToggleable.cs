#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.UI.Module
{
    public interface IToggleable
    {
        string Name { get; }
        bool Toggleable { get; set; }
        bool IsToggledOn { get; }
        int GetInstanceID();
        void SetToggleListener(IToggleListener toggleListener);
        void SetToggledOn();
        void SetToggledOff();
    }
}

#endif
