using System.Collections;
using UnityEngine;

namespace Utils
{
    public class SaveLoad : Singleton<SaveLoad>
    {
        private GameManager _gm;
        private Input.Actions _actions;
        
        protected override void OnAwake()
        {
            _gm = this.GetComponent<GameManager>();
            _actions = Input.Actions.Instance;
            EDebug.Log("Save/Load ► Awake");
        }
        
        [ContextMenu("Load Game")] public void LoadGame()
        {
            // Implement load game logic here
        }

        [ContextMenu("Save Game")] public void SaveGame() 
        {
            StartCoroutine(CaptureScreenshotWithResolution(1280, 720));
            // Implement save game logic here
        }
        
        private IEnumerator CaptureScreenshotWithResolution(int width, int height)
        {
            bool fullscreen = Screen.fullScreen;
            int originalWidth = Screen.width;
            int originalHeight = Screen.height;

            Screen.SetResolution(width, height, true);
            yield return new WaitForEndOfFrame();

            string path = System.IO.Path.Combine(Application.dataPath, "../saves/data_1.png");
            ScreenCapture.CaptureScreenshot(path);

            yield return new WaitForEndOfFrame();
            Screen.SetResolution(originalWidth, originalHeight, fullscreen);
        }
        
        
    }
}
