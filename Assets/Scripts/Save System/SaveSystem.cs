using System;
using System.Text;
using Entity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SaveSystem
{
    public static class SaveSystem
    {
        const string LEVEL_DATA_KEY = "level_data";
        const string LEVEL_INDEX_KEY = "level_key";
        public const string SFX_KEY = "sfx_volume";

        public const string MUSIC_KEY = "music_volume";

        public const string MASTER_VOLUME_KEY = "master_volume";

        const string SEPARATOR = "|*|";

        public static Action OnSaveData;
        public static Action OnLoadData;

        public static int CurrentSaveFileIndex { get; private set; } = 0;
        public const float DEFAULT_VOLUME = 0.5f;

        public static void SaveLevelData(int saveIndex = 0)
        {
            CreateKeyIfOneDoesNotExist();
            CurrentSaveFileIndex = saveIndex;

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);
            StringBuilder sb = new StringBuilder();

            sb.Append(SavePlayerData(allLivingEntities));
            sb.Append(SaveEnemyData(allLivingEntities));

            string current_level_data = LEVEL_DATA_KEY + saveIndex;

            PlayerPrefs.SetInt(current_level_data, SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetString(current_level_data, sb.ToString());
            PlayerPrefs.Save();
            OnSaveData?.Invoke();
        }

        public static void LoadEverything(int loadIndex = 0)
        {
            CreateKeyIfOneDoesNotExist();

            string raw_data = PlayerPrefs.GetString(LEVEL_DATA_KEY + loadIndex);
            if (string.IsNullOrWhiteSpace(raw_data))
            {
                EDebug.LogWarning($"No data exist to load |{raw_data}|");
                return;
            }

            CurrentSaveFileIndex = loadIndex;

            int index = 0;
            string[] data_divided = raw_data.Split(SEPARATOR);

            LoadGameScene();

            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);

            LoadPlayerData(allLivingEntities, data_divided, ref index);
            LoadEnemyData(allLivingEntities, data_divided, ref index);

            EDebug.Log("<color=orange>Loading data </color>");
            EDebug.Log($"<color=orange>Total elements = {data_divided.Length}</color>");
            EDebug.Log($"<color=orange>Current Index = {index}</color>");
            OnLoadData?.Invoke();
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
            CreateKeyIfOneDoesNotExist();

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
            LoadEnemyData(allLivingEntities, data_divided, ref index);
        }

        public static void LoadLevel()
        {
            CreateKeyIfOneDoesNotExist();
            LoadGameScene();
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

        public static float GetVolume(SoundType soundType)
        {
            CreateKeyIfOneDoesNotExist();
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

        #region SAVING_FUNCTIONS

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
            CreateKeyIfOneDoesNotExist();
            PlayerPrefs.SetFloat(key, _newValue);
        }

        #endregion


        #region LOADING_FUNCTIONS

        private static void LoadPlayerData(LivingEntity[] allLivingEntities, string[] data_divided, ref int index)
        {
            for (int i = 0; i < allLivingEntities.Length; ++i)
            {
                if (allLivingEntities[i].isPlayer)
                {
                    allLivingEntities[i].loadData(data_divided[index]);
                    index += 1;

                }
            }
        }

        private static void LoadEnemyData(LivingEntity[] allLivingEntities, string[] data_divided, ref int index)
        {
            for (int i = 0; i < allLivingEntities.Length; ++i)
            {
                if (allLivingEntities[i].gameObject.TryGetComponent<Enemy>(out Enemy result))
                {
                    result.loadData(data_divided[index]);
                    index += 1;
                }
            }
        }


        private static void LoadGameScene(int loadIndex = 0)
        {
            if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex))
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt(LEVEL_INDEX_KEY + loadIndex));
            }
        }

        #endregion

        public static void CreateKeyIfOneDoesNotExist(int levelIndex = 0)
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
        }

        public static bool DoesSaveFileExist(int index = 0)
        {
            return PlayerPrefs.HasKey(LEVEL_DATA_KEY + index);
        }

        public static void DeleteData(int index = 0)
        {
            PlayerPrefs.DeleteKey(LEVEL_DATA_KEY + index);
        }

        public static void setSaveFileIndex(int index)
        {
            CurrentSaveFileIndex = index;
        }

    }

}

