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
        private EventConditions conditions;
        public bool Flag { get; private set; }

        private void GetSoundManager()
        { _soundManager = MiscUtils.GetOrCreateGameManager().SoundManager; }

        private void Awake() {
            if (eventToPlay.IsNull) { EDebug.LogError(this.name + "'s: eventToPlay is null!"); return; }
            if (_soundManager == null) GetSoundManager();
            if (conditions.HasFlag(EventConditions.OnAwake) || 
                (conditions.HasFlag(EventConditions.OnBoolCommand) && Flag)) PlayEvent();
        }
        private void OnEnable() {
            if (eventToPlay.IsNull) { EDebug.LogError(this.name + "'s: eventToPlay is null!"); return; }
            if (_soundManager == null) GetSoundManager();
            if (conditions.HasFlag(EventConditions.OnEnable) ||
                (conditions.HasFlag(EventConditions.OnBoolCommand) && Flag)) PlayEvent();
        }
        private void Start() {
            if (eventToPlay.IsNull) { EDebug.LogError(this.name + "'s: eventToPlay is null!"); return; }
            if (_soundManager == null) GetSoundManager();
            if (conditions.HasFlag(EventConditions.OnStart) ||
                (conditions.HasFlag(EventConditions.OnBoolCommand) && Flag)) PlayEvent();
        }

        private void SetParamsNPlay() {
            if (!string.IsNullOrWhiteSpace(parameterName))
                _soundInstance.setParameterByName(parameterName, parameter);
            _soundInstance.start();
        }
        public void SetFlag(bool value) {
            Flag = value;
            if (conditions.HasFlag(EventConditions.OnBoolCommand) && Flag)
                PlayEvent();
        }

        [ContextMenu("Play Event")] private void PlayEvent() {
            if (_soundInstance.isValid())
            {
                _soundInstance.stop(Studio.STOP_MODE.IMMEDIATE);
                SetParamsNPlay();
            }
            else
            {
                _soundInstance = RuntimeManager.CreateInstance(eventToPlay);
                _soundInstance.setVolume(FmodUtils.GetCompositeVolume(volumeController));
                SetParamsNPlay();
                _soundManager.eventInstances.Add(new EventSoundType {
                    EventI = _soundInstance,
                    soundType = volumeController
                });
            }
        }
        
    }
}
