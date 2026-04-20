#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Game;
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Stats;
using UnityEngine;

namespace Nekoyume.UI.Module
{
    public class BuffLayout : MonoBehaviour
    {
        public GameObject iconPrefab;
        private readonly HashSet<BuffView> AddedBuffs = new();
        public IReadOnlyDictionary<int, BuffView> buffData = new Dictionary<int, BuffView>();

        private Transform _buffParent;
        [SerializeField] private List<BuffIcon> pool = new(20);

        public bool IsBuffAdded(StatType statType)
        {
            return AddedBuffs.Any(view => view.IsStatBuff && view.StatType == statType);
        }

        public bool HasBuff(StatType statType)
        {
            return buffData.Values.Any(view => view.IsStatBuff && view.StatType == statType);
        }

        public void Awake()
        {
            _buffParent = transform;
        }

        private void OnDisable()
        {
            foreach (var icon in pool)
            {
                icon.Hide();
            }
        }

        public void SetBuff(IReadOnlyDictionary<int, BuffView> buffs, TableSheets tableSheets, bool vfx)
        {
            foreach (var icon in pool.Where(icon => icon.gameObject.activeSelf))
            {
                icon.Hide();
            }

            if (buffs is null)
            {
                return;
            }

            AddedBuffs.Clear();
            foreach (var buff in buffs)
            {
                if (!buffData.ContainsKey(buff.Key) ||
                    buffData[buff.Key].RemainedDuration < buffs[buff.Key].RemainedDuration)
                {
                    AddedBuffs.Add(buff.Value);
                }
            }

            buffData = buffs;

            var ordered = buffs.Values
                .Where(buff => buff.RemainedDuration > 0)
                .OrderBy(buff => buff.Id);

            foreach (var buff in ordered)
            {
                var icon = GetDisabledIcon();
                icon.Show(buff, AddedBuffs.Contains(buff), tableSheets, vfx);
            }
        }

        private BuffIcon GetDisabledIcon()
        {
            var icon = pool.First(i => !i.gameObject.activeSelf);
            return icon;
        }
    }
}

#endif
