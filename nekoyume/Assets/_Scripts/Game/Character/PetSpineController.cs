#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿namespace Nekoyume.Game.Character
{
    public class PetSpineController : SpineController
    {
        protected override bool IsLoopAnimation(string animationName)
        {
            return animationName == nameof(PetAnimation.Type.Idle);
        }
    }
}

#endif
