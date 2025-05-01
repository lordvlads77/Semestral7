using UnityEngine;
using Color = UnityEngine.Color;

namespace Utils
{
    public static class StringUtil
    {
        public static string addSizeTagToString(string input, int size)
        {
            string str_size = size.ToString();

            return $"<size={str_size}> {input} </size>";
        }

        public static string addColorToString(string input, Color color)
        {
            string color_str = ColorUtility.ToHtmlStringRGBA(color);

            return $"<color=#{color_str}> {input} </color>";
        }
    }
}