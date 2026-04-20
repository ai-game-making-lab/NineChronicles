#if LIB9C_RESTORED // stubbed out after lib9c deletion
using TMPro;
using UnityEngine;

namespace Nekoyume.UI.Module
{
    public class StakingBuffBenefitsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI contentText;

        public void Set(string description)
        {
            contentText.text = description;
        }
    }
}

#endif
