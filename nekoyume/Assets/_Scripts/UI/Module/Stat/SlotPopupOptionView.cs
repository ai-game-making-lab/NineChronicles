#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using TMPro;
using UnityEngine;

namespace Nekoyume.UI.Module
{
    public class SlotPopupOptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI optionText;

        public void Set(string option)
        {
            optionText.text = option;
        }
    }
}

#endif
