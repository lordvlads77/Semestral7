using Scriptables;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Entity
{
    public class Dialog : MonoBehaviour
    {
        private Canvas _npcCanvas;

        private void Awake()
        {
            GameManager gm = (GameManager.Instance != null)? GameManager.Instance : MiscUtils.CreateGameManager();
            _npcCanvas = gm.NpcCanvas;
            if (_npcCanvas == null)
            {
                EDebug.Log("New canvas is being created...");
                _npcCanvas = gm.CreateNpcCanvas();
            }
        }

        public void StartDialog(LivingEntity npc)
        {
            if (npc == null)
            {
                EDebug.LogError("NPC is null.");
                return;
            }
            DialogOptions dialogOptions = npc.dialogOptions;
            if (dialogOptions != null && dialogOptions.dialogOptions.Count > 0)
            {
                DisplayDialog(npc, dialogOptions.dialogOptions[0]);
            }
            else EDebug.LogError("DialogOptions are null or empty.");
        }

        public void StopDialog()
        {
            if (_npcCanvas == null) return;
            _npcCanvas.gameObject.SetActive(false);
        }
        
        private void DisplayDialog(LivingEntity npc, DialogOption dialogOption)
        {
            if (_npcCanvas == null)
            {
                EDebug.LogError("NPC Canvas is not assigned.");
                return;
            }
            Text dialogText = _npcCanvas.GetComponentInChildren<Text>();
            Text npcName = _npcCanvas.GetComponentInChildren<Text>();
            Button[] responseButtons = _npcCanvas.GetComponentsInChildren<Button>();
            Text[] responseText = _npcCanvas.GetComponentsInChildren<Text>();
            
            if (dialogText == null || npcName == null || responseButtons == null || responseText == null)
            {
                EDebug.LogError("One or more UI components are not assigned.");
                return;
            }
            dialogText.text = dialogOption.npcDialog;
            npcName.text = npc.HasCustomName()? npc.entityName : MiscUtils.GetRandomName(
                GameManager.Instance.randomNames, npc.nameCustomization);
            

            for (int i = 0; i < responseText.Length; i++)
            {
                responseText[i].text = dialogOption.userResponses[i].response;
            }
           
            _npcCanvas.gameObject.SetActive(true);

            for (int i = 0; i < responseButtons.Length; i++)
            {
                if (i < dialogOption.userResponses.Count)
                {
                    responseButtons[i].gameObject.SetActive(true);
                    responseButtons[i].GetComponentInChildren<Text>().text = dialogOption.userResponses[i].response;
                    int index = i; // Capture the index for the lambda
                    responseButtons[i].onClick.RemoveAllListeners();
                    responseButtons[i].onClick.AddListener(() => OnResponseSelected(
                        npc, dialogOption.userResponses[index]));
                }
                else
                {
                    responseButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
        private void OnResponseSelected(LivingEntity npc, ResponseOption responseOption)
        {
            responseOption.OnResponse();
            npc.ChangeMood(responseOption.moodChange);
            if (responseOption.nextDialog != null)
            {
                DisplayDialog(npc, responseOption.nextDialog);
            }
            else StopDialog();
        }
        
    }
}
