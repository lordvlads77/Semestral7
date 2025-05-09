using UnityEngine;

namespace Utils
{
    public class MoveAfterThreshold : MonoBehaviour
    {
        [SerializeField] GameObject Threhold;
        [SerializeField] GameObject Player;
        [SerializeField] Transform newLocation;

        bool hasBeenMove = false;

        // Update is called once per frame
        void Update()
        {
            if (hasBeenMove) { return; }

            Vector3 PlayerPos = Player.transform.position;
            Vector3 ThresPos = Threhold.transform.position;
            if (PlayerPos.x > ThresPos.x)
            {
                Player.transform.position = newLocation.position;
                hasBeenMove = true;
            }
            if (PlayerPos.z > ThresPos.z)
            {
                Player.transform.position = newLocation.position;
                hasBeenMove = true;
            }


        }
    }
}

