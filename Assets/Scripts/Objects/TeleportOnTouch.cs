using UnityEngine;

namespace Objects
{
    public class TeleportOnTouch : MonoBehaviour
    {
        [SerializeField] private Transform teleportDestination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && teleportDestination != null)
            {
                other.GetComponent<CharacterController>().enabled = false;
                other.transform.position = teleportDestination.position;
                other.transform.rotation = teleportDestination.rotation;
                Camera.main.transform.position += teleportDestination.position;
                other.GetComponent<CharacterController>().enabled = true;
            }
        }
    }
}
