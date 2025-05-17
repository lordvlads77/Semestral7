using UnityEngine;

namespace Character
{
    public class MoveCharacterHere : MonoBehaviour
    {
        private void OnEnable()
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.transform.position;
        }
    }
}
