using UnityEngine;

namespace Objects
{
    public class TeleportOnTouch : MonoBehaviour
    {
        [SerializeField] private Transform target;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.transform.position = target.position;
        }
    }
}
