using System;
using UI;
using UnityEditor;
using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;

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
        [SerializeField] private float lerpValue = 10.0f;
        
        public Transform lockTarget;
        
        public GameObject lockIndicatorPrefab;
        private GameObject _lockIndicator;
        public static float ShakeStrength = 0f;

        private double _theta = Math.PI / 2;
        private float _tTheta = 0.5f;
        private double _alpha = -Math.PI / 2;
        private float currentAlpha;
        private float currentTheta;
        private float smoothSpeed = 25f;

        public static NewLoadingScreen NewLoadingScreen;
        
        [SerializeField]
        private float maxLockDistance = 10f;
        
        private bool _wasZTargetPressedLastFrame = false;
        private Collider[] _currentEnemies = new Collider[0];
    
        [Serializable] public struct CameraSettings
        {
            [SerializeField] [Range(-1.0f, 1.0f)]
            private float offsetX;
            [SerializeField] [Range(-1.0f, 1.0f)]
            private float offsetY;
            [SerializeField] [Range(0.0f, 80.0f)]
            private float maxVerticalAngle;
            [SerializeField] [Range(0.0f, 80.0f)]
            private float minVerticalAngle;
            [SerializeField] 
            private float cameraDistance;

            public float GetCameraDistance()
            {
                return cameraDistance;
            }

            public Vector2 GetOffset() {return new Vector2(offsetX, offsetY);}
        
            public Vector2 GetLimitVerticalAnglesRadians(){ return new Vector2(maxVerticalAngle * (float)Math.PI / 180.0f, (minVerticalAngle * (float)Math.PI / 90.0f));}
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
                lookAt = transform;
            }
            SetCameraToOrigin();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reasigna al jugador
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                lookAt = player.transform;
                SetCameraToOrigin();
            }
        }

        void Update()
        {
            if (GameManager.Instance.GameState != GameStates.Playing) return;
            bool isZTargetPressed = _input.ZTarget;

            // Si se acaba de presionar
            if (isZTargetPressed && !_wasZTargetPressedLastFrame)
            {
                _currentEnemies = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Enemy"));
                FindClosestTarget(_currentEnemies);
            }

            // Si se acaba de soltar
            if (!isZTargetPressed && _wasZTargetPressedLastFrame)
            {
                _currentEnemies = new Collider[0]; // Limpiamos
            }

            _wasZTargetPressedLastFrame = isZTargetPressed;

            if (lockTarget == null && _lockIndicator != null)
            {
                _lockIndicator.SetActive(false);
            }
        }

        void FindClosestTarget(Collider[] enemies)
        {
            float closestDistance = Mathf.Infinity;
            Transform closest = null;

            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy.transform;
                }
            }

            if (closest != null)
            {
                lockTarget = closest;

                if (_lockIndicator == null && lockIndicatorPrefab != null)
                {
                    _lockIndicator = Instantiate(lockIndicatorPrefab);
                }
            }
            else
            {
                lockTarget = null;
            }
        }
        private void LateUpdate() // FixedUpdate → LateUpdate (This prevents jittering / choppy movement)
        {
            if (GameManager.Instance.GameState != GameStates.Playing) return;
            OrbitSphericalCoords();
            
            if (lockTarget && _lockIndicator) {
                _lockIndicator.transform.position = lockTarget.position + Vector3.up;
                _lockIndicator.SetActive(true);
            }
            else if (_lockIndicator) _lockIndicator.SetActive(false);
        }

        private void OrbitSphericalCoords()
        {
            // Read input
            float h = _input.Camera.x;
            float v = _input.Camera.y;

            // Check Settings
            h = (invertXAxis)? h : (-h);
            v = (invertYAxis)? (-v) : v;

            // Orbit the camera around the character
            if (h != 0) _alpha += h * sensitivity * Time.deltaTime;
            if (v != 0)
            {
                Vector2 limitAnglesRads = settings.GetLimitVerticalAnglesRadians();
                float maxAngle = ((float)Math.PI / 2) - limitAnglesRads.x;
                float minAngle = ((float)Math.PI / 2) + limitAnglesRads.y;

                _tTheta += v * sensitivity * Time.deltaTime;
                _tTheta = Mathf.Clamp(_tTheta, 0f, 1f);
                _theta = Mathf.Lerp(maxAngle, minAngle, _tTheta);
            }
            // Smooth the angles
            currentAlpha = Mathf.Lerp(currentAlpha, (float)_alpha, Time.deltaTime * smoothSpeed);
            currentTheta = Mathf.Lerp(currentTheta, (float)_theta, Time.deltaTime * smoothSpeed);

            // Calc camera position
            float x = lookAt.position.x + settings.GetCameraDistance() * Mathf.Sin(currentTheta) * Mathf.Cos(currentAlpha);
            float y = lookAt.position.y + settings.GetCameraDistance() * Mathf.Cos(currentTheta);
            float z = lookAt.position.z + settings.GetCameraDistance() * Mathf.Sin(currentTheta) * Mathf.Sin(currentAlpha);
        
            Vector3 newCameraPosition = new Vector3(x, y, z);
            Vector3 offsetCameraPosition = newCameraPosition + settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
            cam.transform.position = Vector3.Lerp(cam.transform.position, offsetCameraPosition, Time.deltaTime * lerpValue);

            // Look at
            Vector3 targetLookAt = lookAt.transform.position + settings.GetOffset().x * cam.transform.right + settings.GetOffset().y * cam.transform.up;
            _trueLookAt.transform.position = Vector3.Lerp(_trueLookAt.transform.position, targetLookAt, Time.deltaTime * lerpValue);
            if (_input.ZTarget && lockTarget) {
                Quaternion targetRotation = Quaternion.LookRotation(lockTarget.position - cam.transform.position);
                cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, Time.deltaTime * lerpValue);
            } 
            else {
                Quaternion targetRotation = Quaternion.LookRotation(_trueLookAt.position - cam.transform.position);
                cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, Time.deltaTime * lerpValue);
            }
        }

        public void SetCameraToOrigin()
        {
            double originTheta = Math.PI / 2;
            double originAlpha = -Math.PI / 2;
            _alpha = -Math.PI / 2;
            _theta = Math.PI / 2;
            currentAlpha = (float)_alpha;
            currentTheta = (float)_theta;
            _tTheta = 0.5f;
            if (!_trueLookAt) _trueLookAt = transform.Find("LookAtTransform");
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
                //Handles.color = Color.red;
                //Handles.DrawWireDisc(lookAt.transform.position + new Vector3(0, minCircleY, 0), Vector3.up, minCircleRadius);

                float maxCircleRadius = (float)Math.Cos(maxAngle - Math.PI / 2) * settings.GetCameraDistance();
                float maxCircleY = (float)(settings.GetCameraDistance() * Math.Cos(maxAngle));
                //Handles.color = Color.red;
                //Handles.DrawWireDisc(lookAt.transform.position + new Vector3(0, maxCircleY, 0), Vector3.up, maxCircleRadius);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(lookAt.transform.position, settings.GetCameraDistance());
            }
        }
        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxLockDistance);

            Collider[] enemies = Physics.OverlapSphere(transform.position, maxLockDistance, LayerMask.GetMask("Enemy"));

            float closestDistance = Mathf.Infinity;
            Transform closestTarget = null;

            foreach (Collider enemy in enemies)
            {
                if (!enemy) continue;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, enemy.transform.position);

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy.transform;
                }
            }

            if (closestTarget)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, closestTarget.position);
                Gizmos.DrawSphere(closestTarget.position + Vector3.up * 2f, 0.3f); // Marcador visual
            }
            #endif
        }
    }
}
