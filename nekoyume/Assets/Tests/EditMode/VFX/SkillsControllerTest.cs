using System;
using Nekoyume.Game.Util;
using Nekoyume.Game.VFX.Skill;
using Nekoyume.Model.Skill;
using Nekoyume.SingleClient.Models.Elemental;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.VFX
{
    public class SkillControllerTest
    {
        private GameObject _gameObject;
        private SkillController _skillController;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject();
            _skillController = new SkillController();
            _skillController.Initialize(new FakeObjectPool(_gameObject.transform));
        }

        [Test]
        public void GetSkillCastingVFXTest()
        {
            foreach (var elementalType in (ElementalType[]) Enum.GetValues(typeof(ElementalType)))
            {
                var vfx = _skillController.Get(Vector3.zero, elementalType);
                Assert.IsNotNull(vfx);
            }
        }

        [Test]
        public void GetBlowCastingVFXTest()
        {
            foreach (var elementalType in (ElementalType[]) Enum.GetValues(typeof(ElementalType)))
            {
                if (elementalType == ElementalType.Normal)
                {
                    continue;
                }

                var vfx = _skillController.GetBlowCasting(
                    Vector3.zero,
                    SkillCategory.BlowAttack,
                    elementalType);
                Assert.IsNotNull(vfx);
            }
        }

        private sealed class FakeObjectPool : IObjectPool
        {
            private readonly Transform _parent;

            public FakeObjectPool(Transform parent)
            {
                _parent = parent;
            }

            public GameObject Add(GameObject prefab, int count)
            {
                return prefab;
            }

            public T Get<T>(Vector3 position) where T : MonoBehaviour
            {
                return Create(position).GetComponent<T>();
            }

            public bool Remove<T>(GameObject go)
            {
                UnityEngine.Object.DestroyImmediate(go);
                return true;
            }

            public void ReleaseAll()
            {
            }

            public GameObject Get(string objName, bool create, Vector3 position = default)
            {
                return Create(position);
            }

            private GameObject Create(Vector3 position)
            {
                var go = new GameObject("skill-vfx");
                go.transform.SetParent(_parent);
                go.transform.position = position;
                go.AddComponent<ParticleSystem>();
                go.AddComponent<SkillCastingVFX>();
                return go;
            }
        }
    }
}
