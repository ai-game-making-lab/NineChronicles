#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    [RequireComponent(typeof(Button))]
    public class OpenUrlButton : MonoBehaviour
    {
        [SerializeField]
        private string url;

        private void Awake()
        {
            GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
            {
                if (string.IsNullOrEmpty(url))
                {
                    return;
                }

                Helper.Util.OpenURL(url);
            }).AddTo(gameObject);
        }
    }
}

#endif
