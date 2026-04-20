#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Items;
using UnityEngine;

namespace Nekoyume.UI.Module
{
    public class SweepItem : MonoBehaviour
    {
        [SerializeField]
        private SweepItemView view;

        /// <summary>
        /// Snapshot-facing facade. Callers that still hold a lib9c <c>ItemBase</c> should project
        /// via <c>ItemSnapshotMapper.ToPolySnapshot()</c> at the call site; this component no
        /// longer depends on <c>Nekoyume.Model.Item</c> types.
        /// </summary>
        public void Set(IItemSnapshot snapshot, int count)
        {
            view.Set(snapshot, count);
        }
    }
}

#endif
