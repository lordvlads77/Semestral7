using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class OptionsMenuScript : MonoBehaviour
    {

        public void Regresar()
        {
            EDebug.Log($"<color=orange> {nameof(this.Regresar)} was executed </color> ");
        }

        public void SetMusicVolume(float volume)
        {
            EDebug.Log($"<color=orange> {nameof(this.SetMusicVolume)} was executed</color>");
        }

        public void SetSFXVolume(float volume)
        {
            EDebug.Log($"<color=orange> {nameof(this.SetSFXVolume)} was executed</color>");
        }


    }

}
