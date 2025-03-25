using UnityEngine;

namespace Utils
{
    public class GenericRotator : MonoBehaviour
    {
        [SerializeField] private bool rotateX = false;
        [SerializeField] private bool rotateY = false;
        [SerializeField] private bool rotateZ = false;
        [SerializeField, Range(0.001f, 10)] private float speed = 1f;
        [SerializeField] private bool lookAtPlayer = false;
        [SerializeField] private bool lookAtCamera = false;
        [SerializeField] private Transform target;

        private void Awake()
        {
            if (lookAtCamera && Camera.main != null)
                target = Camera.main.transform;
            else if (lookAtPlayer)
                target = GameObject.FindGameObjectWithTag("Player").transform;
            if (target == null)
                target = transform;
        }

        private void FixedUpdate()
        {
            if (target == transform || !isActiveAndEnabled) return;
            Vector3 direction = target.position - transform.position;
            Vector3 rotationAxis = new Vector3(
                rotateX? direction.x : 0,
                rotateY? direction.y : 0,
                rotateZ? direction.z : 0
            ).normalized;
            if (rotationAxis == Vector3.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(rotationAxis);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
        
    }
}
