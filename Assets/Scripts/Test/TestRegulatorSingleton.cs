using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;


namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TestRegulatorSingleton : RegulatorSingleton<TestRegulatorSingleton>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            EDebug.Log($"Called 'OnAwake' from {typeof(TestRegulatorSingleton)} instance ");
            SceneManager.activeSceneChanged += OnSceneChange;
        }


        private void OnSceneChange(Scene scene, Scene _s)
        {
            EDebug.Log($"Called '{nameof(OnSceneChange)}'");
        }



    }
}

