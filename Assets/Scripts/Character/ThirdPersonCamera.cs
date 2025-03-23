using System;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Character
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        private Input.Actions _input;
        
        public Camera cam;
        public CameraTypes type = CameraTypes.FreeLook;

        [SerializeField][Range(0.1f, 2.0f)]
        private float sensitivity;
        [SerializeField]
        private bool invertXAxis;
        [SerializeField] 
        private bool invertYAxis;

        public Transform lookAt;
        private Transform _trueLookAt;
        
        public Transform lockTarget;
        //private bool isLocked;


        private double _theta = Math.PI / 2;
        private float _tTheta = 0.5f;
        private double _alpha = -Math.PI / 2;
    
        [Serializable]
        public struct CameraSettings
        {
            [SerializeField] [Range(-1.0f, 1.0f)]
            private float offsetX;
            [SerializeField] [Range(-1.0f, 1.0f)]
            private float offsetY;
            [SerializeField] [Range(0.0f, 90.0f)]
            private float maxVerticalAngle;
            [SerializeField] [Range(0.0f, 90.0f)]
            private float minVerticalAngle;
            [SerializeField]
            private float cameraDistance;

            public float GetCameraDistance()
            {
                return cameraDistance;
            }

            public Vector2 GetOffset() {return new Vector2(offsetX, offsetY);}
        
            public Vector2 GetLimitVerticalAnglesRadians(){ return new Vector2(maxVerticalAngle * (float)Math.PI / 180.0f, (minVerticalAngle * (float)Math.PI / 180.0f));}
        }
        
        [SerializeField]
        private CameraSettings settings;

        public CameraSettings GetCameraSettings()
        {
            return settings;
        }
    
        private void Awake()
        {
            _input = Input.Actions.Instance;
            if (_input == null) _input = gameObject.GetComponent<Input.Actions>();
            if (_input == null) _input = gameObject.AddComponent<Input.Actions>();
            if (!cam) cam = Camera.main;
            if (type == CameraTypes.Locked && cam) {
                cam.transform.parent = transform;
            }
            Cursor.visible = false;
            _trueLookAt = transform.Find("LookAtTransform");
            if(!_trueLookAt) _trueLookAt = new GameObject("LookAtTransform").transform;
            if (!lookAt) {
                lookAt = GameObject.FindWithTag("Player").transform;
            }
        }

        
        void Update()
        {
            if (_input.ZTarget && !lockTarget)
            {
                FindClosestTarget();
            }
        }

        void FindClosestTarget()
        {
            float maxDistance = 10f; // Ajusta según necesites
            Collider[] enemies = Physics.OverlapSphere(transform.position, maxDistance, LayerMask.GetMask("Enemy"));
    
            if (enemies.Length > 0)
            {
                lockTarget = enemies[0].transform; // Puedes mejorar esto para elegir el más cercano
                //isLocked = true;
            }
        }
        private void LateUpdate() // FixedUpdate → LateUpdate (This prevents jittering / choppy movement)
        {
            OrbitSphericalCoords();
        }

        private void OrbitSphericalCoords()
        {
            // Read input
            float h = _input.Camera.x;
            float v = _input.Camera.y;

            // Settings
            h = (invertXAxis)? h : (-h);
            v = (invertYAxis)? (-v) : v;

            // Orbit the camera around the character
            if (h != 0)
            {   // Horizontal movement 
                _alpha += h * sensitivity * Time.deltaTime; 
            }
            if (v != 0)
            {   // Vertical movement
                Vector2 limitAnglesRads = settings.GetLimitVerticalAnglesRadians();
                float maxAngle = limitAnglesRads.x;
                float minAngle = limitAnglesRads.y;

                maxAngle = ((float)Math.PI / 2) - maxAngle;
                minAngle = ((float)Math.PI / 2) + minAngle;
            
                _tTheta += v * sensitivity * Time.deltaTime;
                _tTheta = Mathf.Clamp(_tTheta, 0, 1);
                _theta = Mathf.Lerp(maxAngle, minAngle, _tTheta);
            }
        
            float x = lookAt.transform.position.x + (float) (settings.GetCameraDistance() * Math.Sin(_theta) * Math.Cos(_alpha));
            float y = lookAt.transform.position.y + (float) (settings.GetCameraDistance() * Math.Cos(_theta));
            float z = lookAt.transform.position.z + (float) (settings.GetCameraDistance() * Math.Sin(_theta) * Math.Sin(_alpha));
        
            Vector3 newCameraPosition = new Vector3(x, y , z);
            Vector3 offsetCameraPosition = newCameraPosition + settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
            cam.transform.position = offsetCameraPosition;
            _trueLookAt.transform.position = lookAt.transform.position + +settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
            if (_input.ZTarget && lockTarget)
            {
                cam.transform.LookAt(lockTarget.position);
            } 
            else
            {
                lockTarget = null;
                cam.transform.LookAt(_trueLookAt);
            }
        }

        public void SetCameraToOrigin()
        {
            double originTheta = Math.PI / 2;
            double originAlpha = -Math.PI / 2;
            if(!_trueLookAt) _trueLookAt = transform.Find("LookAtTransform");
            float camDistance = settings.GetCameraDistance();
            if (lookAt)
            {
                Vector3 newCameraPosition = lookAt.transform.position +
                                            new Vector3(camDistance * (float)(Math.Sin(originTheta) * Math.Cos(originAlpha)),
                                                camDistance * (float)(Math.Cos(originTheta)),
                                                camDistance * (float)(Math.Sin(originTheta) * Math.Sin(originAlpha)));
                Vector3 offsetCameraPosition = newCameraPosition + settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
                cam.transform.position = offsetCameraPosition;
                _trueLookAt.transform.position = lookAt.transform.position + +settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
                cam.transform.LookAt(_trueLookAt);
            }
        }

        public Camera GetCamera() { return cam ? cam : Camera.main; }

        private void OnDrawGizmosSelected()
        {
            if (lookAt) {
                Vector2 limitAnglesRads = settings.GetLimitVerticalAnglesRadians();
                float maxAngle = limitAnglesRads.x;
                float minAngle = limitAnglesRads.y;

                maxAngle = ((float)Math.PI / 2) - maxAngle;
                minAngle = ((float)Math.PI / 2) + minAngle;

                float minCircleRadius = (float)Math.Cos(minAngle - Math.PI / 2) * settings.GetCameraDistance();
                float minCircleY = (float)(settings.GetCameraDistance() * Math.Cos(minAngle));
                Handles.color = Color.red;
                Handles.DrawWireDisc(lookAt.transform.position + new Vector3(0, minCircleY, 0), Vector3.up, minCircleRadius);

                float maxCircleRadius = (float)Math.Cos(maxAngle - Math.PI / 2) * settings.GetCameraDistance();
                float maxCircleY = (float)(settings.GetCameraDistance() * Math.Cos(maxAngle));
                Handles.color = Color.red;
                Handles.DrawWireDisc(lookAt.transform.position + new Vector3(0, maxCircleY, 0), Vector3.up, maxCircleRadius);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(lookAt.transform.position, settings.GetCameraDistance());
            }
        }
    }
}
