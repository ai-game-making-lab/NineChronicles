#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Model.Item;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.UI.Model;

namespace Nekoyume.Helper
{
    public static class InventoryHelper
    {
        /// <summary>
        /// If all elemental type of items are allowed, there is no need to check each item, so this method will return null.
        /// If not, Return `Predicate` for Dim object Enable.
        /// </summary>
        /// <param name="elementalTypes">Allowable Item elemental types (client mirror)</param>
        /// <returns>Predicate func or null. If it need to dim item, func will return true.</returns>
        public static Predicate<InventoryItem> GetDimmedFuncByElementalTypes(
            List<ElementalType> elementalTypes)
        {
            if (!elementalTypes.SequenceEqual(ElementalRules.GetAllTypes()))
            {
                return item => !elementalTypes.Contains(item.ItemBase.ElementalType.ToView());
            }

            return null;
        }
    }
}

#endif
