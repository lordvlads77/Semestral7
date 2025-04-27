using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Utils;

namespace FMOD
{
    public class SoundManager : Singleton<SoundManager>
    {
        public List<EventSoundType> eventInstances = new List<EventSoundType>();

        public void ChangeGameVolume()
        {
            
        }
    }
}
