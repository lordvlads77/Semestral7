using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SaveSystem
{
    public enum LoadingLevelState
    {
        NONE,
        STARTED,
        FINISHED,
    }

    public static class SaveSystem
    {
        const string LEVEL_DATA_KEY = "level_data";
        const string LEVEL_INDEX_KEY = "level_key";
        public const string SFX_KEY = "sfx_volume";

        public const string MUSIC_KEY = "music_volume";

        public const string MASTER_VOLUME_KEY = "master_volume";

        public const string LANGUAGE_SELECTED_KEY = "language_selected";

        public const string WINDOW_RESOLUTION_KEY = "WindowResolution";
        public const string WINDOW_MODE_KEY = "WindowMode";

        public const string EMPTY_SAVE_FILE = "NULL";

        const string SEPARATOR = "|*|";

        public static Action OnSaveData;
        public static Action OnLoadData;

        public static LoadingLevelState loadingLevelState = LoadingLevelState.NONE;

        public static int CurrentSaveFileIndex { get; private set; } = 0;
        public const float DEFAULT_VOLUME = 0.5f;

        public static void SaveLevelData(int saveIndex = 0)
        {
            CreateKeyIfOneDoesNotExist(saveIndex);
            CurrentSaveFileIndex = saveIndex;

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);
            StringBuilder sb = new StringBuilder();

            sb.Append(SavePlayerData(allLivingEntities));
            sb.Append(SaveEnemyData(allLivingEntities));

            string current_level_data = LEVEL_DATA_KEY + saveIndex;

            PlayerPrefs.SetInt(LEVEL_INDEX_KEY + saveIndex, SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetString(current_level_data, sb.ToString());
            PlayerPrefs.Save();
            OnSaveData?.Invoke();
        }

        /// <summary>
        /// Load the player and level with a coroutine
        /// </summary>
        /// <remarks> USE THIS INSTEAD OF 'LoadEverything'  </remarks>
        /// <param name="loadingIndex"></param>
        public static void LoadPlayerAndLevel(int loadingIndex = 0)
        {
            CoroutineCaller.Instance.StartCoroutine(LoadEverything2(loadingIndex));
        }

        [Obsolete("LoadEverything is deprecated, please use  LoadPlayerAndLevel instead. Reason : LoadPlayerAndLevel uses coroutine the other blocks everything and looks bad")]
        public static void LoadEverything(int loadIndex = 0)
        {
            CreateKeyIfOneDoesNotExist(loadIndex);

            string raw_data = PlayerPrefs.GetString(LEVEL_DATA_KEY + loadIndex);
            if (string.IsNullOrWhiteSpace(raw_data))
            {
                EDebug.LogWarning($"No data exist to load |{raw_data}|");
                return;
            }
            else if (raw_data == EMPTY_SAVE_FILE)
            {
                EDebug.Log("This save file is being used but does not have any data on it");
                return;
            }

            CurrentSaveFileIndex = loadIndex;

            int index = 0;
            string[] data_divided = raw_data.Split(SEPARATOR);

            LoadGameScene(loadIndex);

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);
            int enemyCount = Utils.MiscUtils.CountEnemiesInScene(allLivingEntities);
            LoadPlayerData(allLivingEntities, data_divided, ref index);
            LoadEnemyData(allLivingEntities, data_divided, ref index, out int enemys_loaded);

            EDebug.Log("<color=orange>Loading data </color>");
            EDebug.Log($"<color=orange>Total elements = {data_divided.Length}</color>");
            EDebug.Log($"<color=orange>Current Index = {index}</color>");
            OnLoadData?.Invoke();
        }

        /// <summary>
        /// The Coroutine version of LoadEverthing
        /// </summary>
        /// <param name="loadIndex">which of the save file are being used</param>
        /// <returns></returns>
        private static IEnumerator LoadEverything2(int loadIndex = 0)
        {
            CreateKeyIfOneDoesNotExist(loadIndex);

            string raw_data = PlayerPrefs.GetString(LEVEL_DATA_KEY + loadIndex);
            if (string.IsNullOrWhiteSpace(raw_data))
            {
                EDebug.LogWarning($"No data exist to load |{raw_data}|");
                yield break;
            }
            else if (raw_data == EMPTY_SAVE_FILE)
            {
                EDebug.Log("This save file is being used but does not have any data on it");
                yield break;
            }

            CurrentSaveFileIndex = loadIndex;

            int index = 0;
            string[] data_divided = raw_data.Split(SEPARATOR);

            CoroutineCaller.Instance.StartCoroutine(LoadGameScene2(loadIndex));

            while (loadingLevelState != LoadingLevelState.FINISHED)
            {
                yield return new WaitForEndOfFrame();
            }

            loadingLevelState = LoadingLevelState.NONE;
            float watingTime = 0.16f;
            yield return new WaitForSeconds(watingTime);
            yield return new WaitForEndOfFrame();
            EDebug.Log($"<color=cyand>Wait for seconds=|{watingTime}|</color>");

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);


            LoadPlayerData(allLivingEntities, data_divided, ref index);

            int index_before_enemies = index;

            int enemies_in_scene = Utils.MiscUtils.CountEnemiesInScene(allLivingEntities);

            LoadEnemyData(allLivingEntities, data_divided, ref index, out int enemys_loaded);

            if (enemies_in_scene < enemys_loaded)
            {
                disableExtraEnemies(allLivingEntities, enemys_loaded, enemies_in_scene);
            }


            EDebug.Log("<color=orange>Loading data </color>");
            EDebug.Log($"<color=orange>Total elements = {data_divided.Length}</color>");
            EDebug.Log($"<color=orange>Current Index = {index}</color>");
            OnLoadData?.Invoke();
        }


        public static void CreateEmptySaveFile(int index)
        {
            CreateKeyIfOneDoesNotExist(index);
            PlayerPrefs.SetString(LEVEL_DATA_KEY + index, EMPTY_SAVE_FILE);
            CurrentSaveFileIndex = index;
        }

        #region SAVING_LOGIC_FUNCTIONS

        private static string SavePlayerData(LivingEntity[] _allLivingEntities)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _allLivingEntities.Length; ++i)
            {
                if (_allLivingEntities[i].isPlayer)
                {
                    sb.Append(_allLivingEntities[i].SaveLivingEntity());
                }
            }

            return sb.ToString();
        }

        private static string SaveEnemyData(LivingEntity[] _allLivingEntities)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _allLivingEntities.Length; ++i)
            {
                if (_allLivingEntities[i].gameObject.TryGetComponent<Enemy>(out Enemy result))
                {
                    sb.Append(SEPARATOR);
                    sb.Append(result.SaveLivingEntity());
                }
            }

            return sb.ToString();
        }

        private static void SaveFloatValue(string key, float _newValue)
        {
            CreateKeyIfOneDoesNotExist(CurrentSaveFileIndex);
            PlayerPrefs.SetFloat(key, _newValue);
        }

        public static void SaveLanguageSelection(Language selectedLanguage)
        {
            CreateKeyIfOneDoesNotExist(CurrentSaveFileIndex);
            PlayerPrefs.SetInt(LANGUAGE_SELECTED_KEY, (int)selectedLanguage);
            PlayerPrefs.Save();
        }

        public static void SaveWindowResolution(WindowResolution windowResolution)
        {
            PlayerPrefs.SetInt(WINDOW_RESOLUTION_KEY, (int)windowResolution);
        }

        public static void SaveVolume(SoundType soundType, float newVolume)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    SaveFloatValue(MASTER_VOLUME_KEY, newVolume);
                    break;

                case SoundType.Music:
                    SaveFloatValue(MUSIC_KEY, newVolume);
                    break;

                case SoundType.SFX:
                    SaveFloatValue(SFX_KEY, newVolume);
                    break;

            }
            MiscUtils.GetOrCreateGameManager().SoundManager.ChangeGameVolume();
        }

        #endregion

        #region LOADING_LOGIC_FUNCTIONS

        private static void LoadPlayerData(LivingEntity[] allLivingEntities, string[] data_divided, ref int index)
        {
            for (int i = 0; i < allLivingEntities.Length; ++i)
            {
                if (allLivingEntities[i].isPlayer)
                {
                    allLivingEntities[i].loadData(data_divided[index]);
                    index += 1;

                }
                if (data_divided.Length >= index)
                {
                    break;
                }
            }
        }

        private static void LoadEnemyData(LivingEntity[] allLivingEntities, string[] data_divided, ref int index, out int enemys_loaded)
        {
            enemys_loaded = 0;
            for (int i = 0; i < allLivingEntities.Length; ++i)
            {
                if (allLivingEntities[i].gameObject.TryGetComponent<Enemy>(out Enemy result))
                {
                    result.loadData(data_divided[index]);
                    index += 1;
                    enemys_loaded += 1;
                }
                if (data_divided.Length >= index)
                {
                    break;
                }
            }

        }


        private static void LoadGameScene(int loadIndex = 0)
        {
            if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex))
            {
                LoadingManager.Instance.LoadSceneByIndex(PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex));
            }
        }

        private static IEnumerator LoadGameScene2(int loadIndex = 0)
        {
            if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex))
            {
                LoadingManager.Instance.LoadSceneByIndex(PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex));
            }

            loadingLevelState = LoadingLevelState.STARTED;
            float current_progress = LoadingManager.Instance.sceneProgress;

            while (current_progress < 0.8999f)
            {
                yield return new WaitForEndOfFrame();
                current_progress = LoadingManager.Instance.sceneProgress;
            }

            yield return new WaitForEndOfFrame();

            loadingLevelState = LoadingLevelState.FINISHED;

            yield return null;
        }


        public static void LoadVolumePrefs()
        {
            if (!PlayerPrefs.HasKey(MASTER_VOLUME_KEY)) PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
            if (!PlayerPrefs.HasKey(MUSIC_KEY)) PlayerPrefs.SetFloat(MUSIC_KEY, DEFAULT_VOLUME);
            if (!PlayerPrefs.HasKey(SFX_KEY)) PlayerPrefs.SetFloat(SFX_KEY, DEFAULT_VOLUME);
            PlayerPrefs.Save();
        }

        public static void LoadLevelEntitiesData(int loadIndex = 0)
        {
            CreateKeyIfOneDoesNotExist(loadIndex);

            string raw_data = PlayerPrefs.GetString(LEVEL_DATA_KEY + loadIndex);
            if (string.IsNullOrWhiteSpace(raw_data))
            {
                EDebug.LogError("No data exist to load");
                return;
            }

            int index = 0;
            string[] data_divided = raw_data.Split(SEPARATOR);

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);

            LoadPlayerData(allLivingEntities, data_divided, ref index);
            LoadEnemyData(allLivingEntities, data_divided, ref index, out int enemys_loaded);
        }

        public static void LoadLevel()
        {
            CreateKeyIfOneDoesNotExist(CurrentSaveFileIndex);
            LoadGameScene();
        }


        #endregion

        #region GET_FUNCTIONS

        public static float GetVolume(SoundType soundType)
        {
            CreateKeyIfOneDoesNotExist(CurrentSaveFileIndex);
            float result = -1f;

            switch (soundType)
            {
                case SoundType.Master:
                    result = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
                    break;

                case SoundType.Music:

                    result = PlayerPrefs.GetFloat(MUSIC_KEY, DEFAULT_VOLUME);
                    break;

                case SoundType.SFX:
                    result = PlayerPrefs.GetFloat(SFX_KEY, DEFAULT_VOLUME);
                    break;
            }
            return result;
        }

        public static Language GetLanguage()
        {
            CreateKeyIfOneDoesNotExist(CurrentSaveFileIndex);
            Language result = (Language)PlayerPrefs.GetInt(LANGUAGE_SELECTED_KEY, (int)Language.En);

            return result;
        }

        public static WindowResolution GetWindowResolution()
        {
            int result = PlayerPrefs.GetInt(WINDOW_RESOLUTION_KEY, (int)WindowResolution.R640X480);

            return (WindowResolution)result;
        }

        #endregion

        public static void CreateKeyIfOneDoesNotExist(int levelIndex)
        {
            if (!PlayerPrefs.HasKey(LEVEL_DATA_KEY + levelIndex))
            {
                PlayerPrefs.SetString(LEVEL_DATA_KEY + levelIndex, "");
            }

            if (!PlayerPrefs.HasKey(LEVEL_INDEX_KEY + levelIndex))
            {
                PlayerPrefs.SetInt(LEVEL_INDEX_KEY + levelIndex, SceneManager.GetActiveScene().buildIndex);
            }

            if (!PlayerPrefs.HasKey(SFX_KEY))
            {
                PlayerPrefs.SetFloat(SFX_KEY, DEFAULT_VOLUME);
            }

            if (!PlayerPrefs.HasKey(MUSIC_KEY))
            {
                PlayerPrefs.SetFloat(MUSIC_KEY, DEFAULT_VOLUME);
            }

            if (!PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
            {
                PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
            }

            if (!PlayerPrefs.HasKey(LANGUAGE_SELECTED_KEY))
            {
                PlayerPrefs.SetInt(LANGUAGE_SELECTED_KEY, (int)Language.En);
            }

            if (!PlayerPrefs.HasKey(WINDOW_RESOLUTION_KEY))
            {
                PlayerPrefs.SetInt(WINDOW_RESOLUTION_KEY, (int)WindowResolution.R800X600);
            }

            if (!PlayerPrefs.HasKey(WINDOW_MODE_KEY))
            {
                PlayerPrefs.SetInt(WINDOW_MODE_KEY, (int)WindowMode.Fullscreen);
            }

        }

        public static bool DoesSaveFileExist(int index = 0)
        {
            return PlayerPrefs.HasKey(LEVEL_DATA_KEY + index);
        }

        public static bool isSaveFileEmpty(int index)
        {
            CreateKeyIfOneDoesNotExist(index);
            string result = PlayerPrefs.GetString(LEVEL_DATA_KEY + index);
            return result == EMPTY_SAVE_FILE || result == "";
        }

        public static void DeleteData(int index = 0)
        {
            PlayerPrefs.DeleteKey(LEVEL_DATA_KEY + index);
            PlayerPrefs.Save();
            // hacer que el nuevo menu demuestre si se borro los datos 
            var current_lang = LanguageManager.Instance.currentLanguage;
            LanguageManager.Instance.setLanguage(current_lang);
        }

        public static void setSaveFileIndex(int index)
        {
            CurrentSaveFileIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allEntities"></param>
        /// <param name="enemies_loaded"></param>
        /// <param name="enemies_in_scene"></param>
        private static void disableExtraEnemies(LivingEntity[] allEntities, int enemies_loaded, int enemies_in_scene)
        {
            /// TODO: TERMINAR ESTO
            List<Enemy> enemies = Utils.MiscUtils.extractEnemiesFromLivingEntities(allEntities));


        }


    }

}

