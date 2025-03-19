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
}
