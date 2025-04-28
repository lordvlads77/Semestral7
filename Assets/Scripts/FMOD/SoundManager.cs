using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Utils;

namespace FMOD
{
    public class SoundManager : MonoBehaviour
    {
        public List<EventSoundType> eventInstances = new List<EventSoundType>();
        private HashSet<EventInstance> _pausedByManager = new HashSet<EventInstance>();

        public void ChangeGameVolume() {
            foreach (var eventSoundType in eventInstances) {
                if (eventSoundType.EventI.isValid())
                    eventSoundType.EventI.setVolume(FmodUtils.GetCompositeVolume(eventSoundType.soundType));
            }
        }
        
        [ContextMenu("Pause all sounds")] public void PauseAllSounds() {
            foreach (var eventSoundType in eventInstances) {
                if (eventSoundType.EventI.isValid() && !_pausedByManager.Contains(eventSoundType.EventI)) {
                    eventSoundType.EventI.setPaused(true);
                    _pausedByManager.Add(eventSoundType.EventI);
                }
            }
        }
        
        [ContextMenu("Resume all sounds")] public void ResumeAllSounds() {
            foreach (var eventInstance in _pausedByManager) {
                if (eventInstance.isValid())
                    eventInstance.setPaused(false);
            }
            _pausedByManager.Clear();
        }
        
        [ContextMenu("Set all sounds to 0 volume")] public void SetAllSoundsToZeroVolume() {
            SaveSystem.SaveSystem.SaveVolume(SoundType.Master, 0);
            SaveSystem.SaveSystem.SaveVolume(SoundType.Music, 0);
            SaveSystem.SaveSystem.SaveVolume(SoundType.SFX, 0);
        }
        
        [ContextMenu("Set all sounds to max")] public void SetAllSoundsToMaxVolume() {
            SaveSystem.SaveSystem.SaveVolume(SoundType.Master, 1);
            SaveSystem.SaveSystem.SaveVolume(SoundType.Music, 1);
            SaveSystem.SaveSystem.SaveVolume(SoundType.SFX, 1);
        }
        
    }
}
