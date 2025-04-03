using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "DialogOptions", menuName = "Scriptables/DialogOptions")]
    public class DialogOptions : ScriptableObject // That's all folks!
    {
        public List<DialogOption> dialogOptions;

        public DialogOptions(List<DialogOption> dialogOptions)
        {
            this.dialogOptions = dialogOptions;
        }
    }
    
    [CreateAssetMenu(fileName = "RandDialogOptions", menuName = "Scriptables/RandDialogOptions")]
    public class RandDialogOptions : ScriptableObject
    {
        public List<DialogOption> dialogOptions;

        public DialogOption GetRandomDialogOption()
        {
            if (dialogOptions == null || dialogOptions.Count == 0)
            {
                EDebug.LogError("DialogOptions list is empty or null.");
                return new DialogOption()
                {
                    npcDialog = "I have nothing to say.",
                    userResponses = new List<ResponseOption>
                    {
                        new ResponseOption
                        {
                            response = "Ok",
                            moodChange = 0,
                            nextDialog = null,
                            onResponse = null
                        }
                    }
                    
                };
            }
            int randI = Random.Range(0, dialogOptions.Count);
            return dialogOptions[randI];
        }
    }
    
}
