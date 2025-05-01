using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils;

namespace FMOD
{
    public class PlayPersistent : MonoBehaviour
    {
        private SoundManager _soundManager;
        private EventInstance _soundInstance;
        [Header("Sound Settings")]
        [SerializeField, Tooltip("Sound you'll play")] 
        private EventReference eventToPlay;
        [SerializeField, Tooltip("How the volume will be determined")] 
        private SoundType volumeController;
        [SerializeField, Tooltip("Parameter if any, leave blank if unnecessary")] 
        private string parameterName = "MenuSoundType";
        [SerializeField, Tooltip("Parameter value (if applies)")] 
        private int parameter;
        [SerializeField, Tooltip("Under what conditions can the event start to play?")] 
        private EventConditions startConditions;
        [SerializeField, Tooltip("Under what conditions should the event stop?")] 
        private EventConditions stopConditions;
        [SerializeField, Tooltip("Under what conditions should the trigger be activated?")]
        private TriggerConditions triggerConditions;
        [SerializeField, Tooltip("Layers that can be detected by this sound emitter")]
        private LayerMask admittedLayers;
        [SerializeField, Tooltip("Tags that can be detected by this sound emitter")]
        private string[] admittedTags;
        [Tooltip("When this sound is re-enabled, should it restart or resume?")] 
        public bool restarts = false;
        [Tooltip("When this sound is disabled, should it pause or stop?")] 
        public bool stops = false;
        [Tooltip("Set this to true if the sound event has 3D space attributes, if you don't, it will not audible")]
        public bool in3DSpace = false;
        [Tooltip("Leave empty if this sound doesn't move around")]
        public Transform followTarget;
        [SerializeField, Tooltip("Set this to true if you want to also, PHYSICALLY follow the target in 3D space")]
        private bool movePhysically = false;
        [SerializeField, Tooltip("Render area gizmo. (Only visible in play mode)")] 
        private bool renderGizmo = false;
        
        public bool Flag { get; private set; }

        private SoundManager GetOrCreateSoundManager() {
            if (_soundManager != null) return _soundManager;
            var gm = MiscUtils.GetOrCreateGameManager();
            _soundManager = gm.SoundManager;
            return _soundManager;
        }
        private void Set3DSpace(Vector3 pos) {
            FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(pos);
            attributes.velocity = new FMOD.VECTOR{ x = 0, y = 0, z = 0 };
            _soundInstance.set3DAttributes(attributes);
        }
        private void OnGameStateChanged(GameStates newState) {
            if (newState == GameStates.Paused) {
                if (startConditions.HasFlag(EventConditions.OnGamePaused)) PlayEvent();
                if (stopConditions.HasFlag(EventConditions.OnGamePaused)) StopEvent();
            } else if (newState == GameStates.Playing) {
                if (startConditions.HasFlag(EventConditions.OnGameUnpaused)) PlayEvent();
                if (stopConditions.HasFlag(EventConditions.OnGameUnpaused)) StopEvent();
            }
        }
        private void HandleTriggerEvent(Collider col, bool isEntering) {
            if (isEntering) {
                if (startConditions.HasFlag(EventConditions.OnTriggerEnter)) {
                    if (triggerConditions.HasFlag(TriggerConditions.ByTags)) {
                        foreach (string admitted in admittedTags) {
                            if (col.CompareTag(admitted)) {
                                PlayEvent();
                                break; 
                            } 
                        }
                    }
                    if (triggerConditions.HasFlag(TriggerConditions.ByLayers)) {
                        if ((admittedLayers.value & (1 << col.gameObject.layer)) != 0)  PlayEvent();
                    }
                }
                if (stopConditions.HasFlag(EventConditions.OnTriggerEnter)) {
                    if (triggerConditions.HasFlag(TriggerConditions.ByTags)) {
                        foreach (string admitted in admittedTags) {
                            if (col.CompareTag(admitted)) {
                                StopEvent();
                                break; 
                            } 
                        }
                    }
                    if (triggerConditions.HasFlag(TriggerConditions.ByLayers)) {
                        if ((admittedLayers.value & (1 << col.gameObject.layer)) != 0) StopEvent();
                    }
                }
            } else {
                if (startConditions.HasFlag(EventConditions.OnTriggerExit)) {
                    if (triggerConditions.HasFlag(TriggerConditions.ByTags)) {
                        foreach (string admitted in admittedTags) {
                            if (col.CompareTag(admitted)) {
                                PlayEvent();
                                break;
                            }
                        }
                    }
                    if (triggerConditions.HasFlag(TriggerConditions.ByLayers)) {
                        if ((admittedLayers.value & (1 << col.gameObject.layer)) != 0) PlayEvent();
                    }
                }
                if (stopConditions.HasFlag(EventConditions.OnTriggerExit)) {
                    if (triggerConditions.HasFlag(TriggerConditions.ByTags)) {
                        foreach (string admitted in admittedTags) {
                            if (col.CompareTag(admitted)) {
                                StopEvent();
                                break;
                            }
                        }
                    }
                    if (triggerConditions.HasFlag(TriggerConditions.ByLayers)) {
                        if ((admittedLayers.value & (1 << col.gameObject.layer)) != 0) StopEvent();
                    }
                }
            }
        }

        private void Awake() {
            if (startConditions.HasFlag(EventConditions.OnAwake)) PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnAwake)) StopEvent();
        }
        private void OnEnable() {
            MiscUtils.GetOrCreateGameManager().Subscribe(OnGameStateChanged);
            if (startConditions.HasFlag(EventConditions.OnEnable)) PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnEnable)) StopEvent();
        }
        private void Start() {
            if (startConditions.HasFlag(EventConditions.OnStart)) PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnStart)) StopEvent();
        }
        private void OnDisable() {
            //MiscUtils.GetOrCreateGameManager().Unsubscribe(OnGameStateChanged);
            GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChanged);
            if (startConditions.HasFlag(EventConditions.OnDisable)) PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnDisable)) StopEvent();
        }
        private void OnDestroy() {
            if (startConditions.HasFlag(EventConditions.OnDestroy)) PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnDestroy)) StopEvent();
        }
        private void OnTriggerEnter(Collider other) {
            HandleTriggerEvent(other, true);
        }
        private void OnTriggerExit(Collider other) {
            HandleTriggerEvent(other, false);
        }
        private void LateUpdate() {
            if (!in3DSpace || followTarget == null) return;
            Set3DSpace(followTarget.position);
            if (movePhysically) transform.position = followTarget.position;
        }
        
        private void SetParamsNPlay() {
            if (!string.IsNullOrWhiteSpace(parameterName))
                _soundInstance.setParameterByName(parameterName, parameter);
            _soundInstance.start();
        }
        public void SetFlag(bool value) {
            Flag = value;
            if (startConditions.HasFlag(EventConditions.OnBoolTrue) && Flag)
                PlayEvent();
            if (startConditions.HasFlag(EventConditions.OnBoolFalse) && !Flag)
                PlayEvent();
            if (stopConditions.HasFlag(EventConditions.OnBoolTrue) && Flag)
                StopEvent();
            if (stopConditions.HasFlag(EventConditions.OnBoolFalse) && !Flag)
                StopEvent();
        }

        [ContextMenu("Play Event")] public void PlayEvent() {
            if (eventToPlay.IsNull) { EDebug.LogError(this.name + "'s: eventToPlay is null!"); return; }
            if (_soundInstance.isValid()) {
                if (!restarts) _soundInstance.setPaused(false);
                else {
                    _soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    SetParamsNPlay();
                }
                if (in3DSpace) Set3DSpace((followTarget != null)? followTarget.position : transform.position);
            }
            else {
                _soundInstance = RuntimeManager.CreateInstance(eventToPlay);
                if (!_soundInstance.isValid()) {
                    EDebug.LogError("Failed to create sound instance for: " + eventToPlay);
                    return;
                }
                if (in3DSpace) Set3DSpace((followTarget != null)? followTarget.position : transform.position);
                float volume = FmodUtils.GetCompositeVolume(volumeController);
                if (volume < 0) EDebug.LogError("Invalid volume retrieved for: " + volumeController);
                _soundInstance.setVolume(Mathf.Max(0,FmodUtils.GetCompositeVolume(volumeController)));
                SetParamsNPlay();
                var soundManager = GetOrCreateSoundManager();
                if (soundManager == null) {
                    EDebug.LogError("SoundManager is null!!! Cannot add event instance.");
                    return;
                }
                soundManager.eventInstances.Add(new EventSoundType {
                    EventI = _soundInstance,
                    soundType = volumeController
                });
            }
        }
        [ContextMenu("Stop Event")] public void StopEvent() {
            if (!_soundInstance.isValid()) return;
            if (stops)
                _soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            else 
                _soundInstance.setPaused(true);
        }
        [ContextMenu("Pause Event")] public void PauseEvent() {
            if (_soundInstance.isValid()) _soundInstance.setPaused(true);
        }
        
        private void OnDrawGizmos() {
            if (!in3DSpace || !renderGizmo || !Application.isPlaying) return;
            Gizmos.color = Color.magenta;
            if (eventToPlay.IsNull) return;
            EventDescription eventDescription = RuntimeManager.GetEventDescription(eventToPlay);
            if (eventDescription.isValid()) {
                eventDescription.getMinMaxDistance(out float minDistance, out float maxDistance);
                Gizmos.DrawWireSphere(transform.position, maxDistance);
            } else  EDebug.LogError("Invalid EventDescription for: " + eventToPlay);
        }
        
    }
}
