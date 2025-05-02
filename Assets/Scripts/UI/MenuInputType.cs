using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [System.Flags]
    [System.Serializable]
    public enum MenuInputType : byte
    {
        NONE = 0,

        VERTICAL_DOWN = 0b0000_0001,
        VERTICAL_UP = 0b0000_0010,
        HORIZONTAL_LEFT = 0b0000_0100,
        HORIZONTAL_RIGHT = 0b0000_1000,
        ACCEPTED = 0b0001_0000,

        ALL = VERTICAL_DOWN | VERTICAL_UP | HORIZONTAL_LEFT | HORIZONTAL_RIGHT | ACCEPTED,

        ANY_VERTICAL = VERTICAL_UP | VERTICAL_DOWN,
        ANY_HORIZONTAL = HORIZONTAL_RIGHT | HORIZONTAL_LEFT,
        VERTICAL_OR_HORIZONTAL = ANY_VERTICAL | ANY_HORIZONTAL,
    }

    public static class MenuInputTypeUtils
    {
        public static bool haveAnyMatchingBits(MenuInputType leftSide, MenuInputType rightSide)
        {
            return ((int)leftSide & (int)rightSide) > 0;
        }

        public static bool isBitSet(int bit, MenuInputType rightSide)
        {
            return ((1 << bit) & (int)rightSide) > 0;
        }

    }

}

