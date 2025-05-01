using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [System.Flags]
    public enum MenuInputType : byte
    {
        NONE = 0,            // 0b0000
        VERTICAL = 1 << 0,   // 0b0001
        HORIZONTAL = 1 << 1, // 0b0010
        ACCEPTED = 1 << 2,   // 0b0100
        ALL_TYPES = VERTICAL | HORIZONTAL | ACCEPTED,      // 0b0111
        VERTICAL_OR_HORIZONTAL = VERTICAL | HORIZONTAL,    // 0b0011
    }

    public static class MenuInputTypeUtils
    {
        public static bool haveAnyMatchingBits(MenuInputType leftSide, MenuInputType rightSide)
        {
            return ((int)leftSide & (int)rightSide) > 0;
        }

        public static bool doesSpecificBitMatch(int bit, MenuInputType rightSide)
        {
            return ((1 << bit) & (int)rightSide) > 0;
        }

    }

}

