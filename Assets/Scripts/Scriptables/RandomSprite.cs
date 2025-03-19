using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "RandomSprite", menuName = "Scriptables/RandomSprite")]
    public class RandomSprite : ScriptableObject
    {
        [SerializeField] private Sprite[] sprites;

        public Sprite GetRandomSprite()
        {
            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogWarning("No sprites available in RandomSprite ScriptableObject.");
                return null;
            }
            return sprites[Random.Range(0, sprites.Length)];
        }
    }
}
