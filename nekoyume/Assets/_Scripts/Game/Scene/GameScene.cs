#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Cysharp.Threading.Tasks;

namespace Nekoyume.Game.Scene
{
    // TODO: LobbyScene으로 변경
    public class GameScene : BaseScene
    {
        protected override async UniTask LoadSceneAssets()
        {
            await UniTask.CompletedTask;
        }

        protected override async UniTask WaitActionResponse()
        {
            await UniTask.CompletedTask;
        }

        public override void Clear()
        {
        }
    }
}

#endif
