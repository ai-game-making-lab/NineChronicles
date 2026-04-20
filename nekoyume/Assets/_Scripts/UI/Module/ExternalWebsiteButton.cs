#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    [RequireComponent(typeof(Button))]
    public class ExternalWebsiteButton : MonoBehaviour
    {
        [SerializeField]
        private string websiteLink;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            Helper.Util.OpenURL(websiteLink);
        }
    }
}

#endif
