using Scriptables;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Entity
{
    public class Dialog : MonoBehaviour
    {
        private LivingEntity _selfEntity;
        private float _mood;
        
        [SerializeField] private DialogOptions dialogOptions;
        [SerializeField] private GameObject dialogCanvas;
        [SerializeField] private Text dialogText;
        [SerializeField] private Button[] responseButtons;
        
        private void Awake()
        {
            _selfEntity = GetComponent<LivingEntity>();
            _mood = _selfEntity.GetMood();
        }
        
        public void StartDialog()
        {
            if (dialogOptions != null && dialogOptions.dialogOptions.Count > 0)
            {
                DisplayDialog(dialogOptions.dialogOptions[0]);
            }
        }
        
        private void DisplayDialog(DialogOption dialogOption)
        {
            dialogCanvas.SetActive(true);
            dialogText.text = dialogOption.npcDialog;

            for (int i = 0; i < responseButtons.Length; i++)
            {
                if (i < dialogOption.userResponses.Count)
                {
                    responseButtons[i].gameObject.SetActive(true);
                    responseButtons[i].GetComponentInChildren<Text>().text = dialogOption.userResponses[i].response;
                    int index = i; // Capture the index for the lambda
                    responseButtons[i].onClick.RemoveAllListeners();
                    responseButtons[i].onClick.AddListener(() => OnResponseSelected(dialogOption.userResponses[index]));
                }
                else
                {
                    responseButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
        private void OnResponseSelected(ResponseOption responseOption)
        {
            responseOption.OnResponse();
            _selfEntity.ChangeMood(responseOption.moodChange);
            if (responseOption.nextDialog != null)
            {
                DisplayDialog(responseOption.nextDialog);
            }
            else
            {
                dialogCanvas.SetActive(false);
            }
        }
        
    }
}
