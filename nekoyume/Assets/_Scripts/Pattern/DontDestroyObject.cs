#if LIB9C_RESTORED // stubbed out after lib9c deletion
using UnityEngine;

namespace Nekoyume
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

#endif
