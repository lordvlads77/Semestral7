using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils;

namespace FMOD
{
    public class PlayOneShot : MonoBehaviour
    {
        [SerializeField] private EventReference eventToPlay;
        [SerializeField] private SoundType volumeController;
        [SerializeField] private string parameterName = "MenuSoundType";
        [SerializeField] private int parameter = 0;

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
