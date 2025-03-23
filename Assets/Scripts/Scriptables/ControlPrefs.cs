using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Scriptables
{
    /*[CreateAssetMenu(fileName = "ControlPrefs", menuName = "Scriptables/ControlPrefs")]
    public class ControlPrefs : ScriptableObject
    {
        private const string ControlSchemeKey = "PreferredControlScheme";
        private const string KeyMappingPrefix = "KeyMapping_";

        public string preferredControlScheme;
        public InputActionAsset inputActions;

        private void OnEnable()
        {
            LoadPreferences();
        }

        public void SavePreferences()
        {
            PlayerPrefs.SetString(ControlSchemeKey, preferredControlScheme);
            foreach (var action in inputActions)
            {
                foreach (var binding in action.bindings)
                {
                    string key = KeyMappingPrefix + action.name + "_" + binding.name;
                    PlayerPrefs.SetString(key, binding.overridePath ?? binding.path);
                }
            }
            PlayerPrefs.Save();
        }

        public void LoadPreferences()
        {
            preferredControlScheme = PlayerPrefs.GetString(ControlSchemeKey, "Keyboard&Mouse");
            foreach (var action in inputActions)
            {
                foreach (var binding in action.bindings)
                {
                    string key = KeyMappingPrefix + action.name + "_" + binding.name;
                    if (PlayerPrefs.HasKey(key))
                    {
                        action.ApplyBindingOverride(binding.index, PlayerPrefs.GetString(key));
                    }
                }
            }
        }

        public void SetPreferredControlScheme(string scheme)
        {
            preferredControlScheme = scheme;
            SavePreferences();
        }

        public void SetKeyMapping(string actionName, string bindingName, string newPath)
        {
            var action = inputActions.FindAction(actionName);
            if (action != null)
            {
                var bindingIndex = action.bindings.FindIndex(b => b.name == bindingName);
                if (bindingIndex != -1)
                {
                    action.ApplyBindingOverride(bindingIndex, newPath);
                    SavePreferences();
                }
            }
        }
    }*/
}
