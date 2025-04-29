using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils;

namespace FMOD
{
    // This script is used to play a one-shot sound event in FMOD.
    // A one-shot sound event is a sound that plays once and does not loop.
    // We will lose the reference to the sound instance once it's released.
    // Therefore, we won't be able to stop it nor set any parameters after the fact.
    // This is useful for single instances of non-frequent, non-looping sounds.
    // But you have to be careful when you do choose to use it. (It cannot be paused or stopped after all)
    
    public class PlayOneShot : MonoBehaviour
    {
        [Header("One-Shot Variables")]
        [SerializeField, Tooltip("Sound you'll play")] private EventReference eventToPlay;
        [SerializeField, Tooltip("How the volume will be determined")] private SoundType volumeController;
        [SerializeField, Tooltip("Parameter if any, leave blank if unnecessary")] private string parameterName = "MenuSoundType";
        [SerializeField, Tooltip("Parameter value (if applies)")] private int parameter = 0;
        
        [ContextMenu("Play OneShot Event")] public void PlayOneShotEvent()
        {
            if(eventToPlay.IsNull) return;
            EventInstance soundInstance = RuntimeManager.CreateInstance(eventToPlay);
            if(!string.IsNullOrWhiteSpace(parameterName))
                soundInstance.setParameterByName(parameterName, parameter);
            soundInstance.setVolume(FmodUtils.GetCompositeVolume(volumeController));
            soundInstance.start();
            soundInstance.release();
        }
    }
}
