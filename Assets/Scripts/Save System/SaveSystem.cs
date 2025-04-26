using System;
using System.Collections;
using System.Collections.Generic;
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

        public static void SaveLevelData()
        {
            CreateKeyIfOneDoesNotExist();
            LivingEntity[] allLivingEntities = GameObject.FindObjectsByType<LivingEntity>(FindObjectsSortMode.None);
            StringBuilder sb = new StringBuilder();

            sb.Append(SavePlayerData(allLivingEntities));
            sb.Append(SaveEnemyData(allLivingEntities));

            PlayerPrefs.SetInt(LEVEL_INDEX_KEY, SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetString(LEVEL_DATA_KEY, sb.ToString());
            PlayerPrefs.Save();
            OnSaveData?.Invoke();
        }

        public static void LoadLevelData()
        {
            CreateKeyIfOneDoesNotExist();
            string raw_data = PlayerPrefs.GetString(LEVEL_DATA_KEY);
            if (string.IsNullOrWhiteSpace(raw_data))
            {
                EDebug.LogError("No data exist to load");
                return;
            }

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

        #region SAVING_ENTITIES

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

        #endregion


        #region LOADING_ENTITIES

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


        private static void LoadGameScene()
        {
            if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt(LEVEL_INDEX_KEY))
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt(LEVEL_INDEX_KEY));
            }
        }

        #endregion

        public static void CreateKeyIfOneDoesNotExist()
        {
            if (!PlayerPrefs.HasKey(LEVEL_DATA_KEY))
            {
                PlayerPrefs.SetString(LEVEL_DATA_KEY, "");
            }

            if (!PlayerPrefs.HasKey(LEVEL_INDEX_KEY))
            {
                PlayerPrefs.SetInt(LEVEL_INDEX_KEY, SceneManager.GetActiveScene().buildIndex);
            }

            if (!PlayerPrefs.HasKey(SFX_KEY))
            {
                PlayerPrefs.SetFloat(SFX_KEY, 0.5f);
            }

            if (!PlayerPrefs.HasKey(MUSIC_KEY))
            {
                PlayerPrefs.SetFloat(MUSIC_KEY, 0.5f);
            }

            if (!PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
            {
                PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, 0.5f);
            }
        }

        public static bool DoesSaveFileExist()
        {
            return PlayerPrefs.HasKey(LEVEL_DATA_KEY);
        }
        
        

    }

}

