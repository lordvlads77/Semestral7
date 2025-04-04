using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Entity
{
    public class Dialog : Singleton<Dialog>
    {
        private Canvas _npcCanvas;
        private LivingEntity _npc;
        private DialogOptions _dialog;
        private GameObject _responsePrefab;
        private GameManager _gm;
        private GameObject _dialogPrompt;

        protected override void OnAwake()
        {
            _gm = this.GetComponent<GameManager>();
            _npcCanvas = _gm.NpcCanvas;
            if (_npcCanvas == null)
            {
                EDebug.Log("New canvas is being created...");
                _npcCanvas = _gm.GetOrCreateNpcCanvas();
            }
            _responsePrefab = _gm.canvasPrefabs.npcOption;
            _npcCanvas.enabled = false;
            EDebug.Log("Dialog ► Awake");
        }

        public void StartDialog(LivingEntity npc)
        {
            _npc = npc;
            _npc.isInDialog = true;
            _dialog = _npc.dialogOptions;
            if (_dialog != null && _dialog.dialogOptions.Count > 0) DisplayDialog(_dialog.dialogOptions[0]);
            else EDebug.LogError("DialogOptions are null or empty... >:/");
        }

        public void StopDialog()
        {
            _npc.isInDialog = false;
            _npc = null;
            _dialog = null;
            if (_npcCanvas == null) return;
            _npcCanvas.gameObject.SetActive(false);
        }
        
        private void DisplayDialog(DialogOption dialogOption)
        {
            if (_npcCanvas == null)
            {
                EDebug.LogError("NPC Canvas is not assigned.");
                return;
            }
            Text dialogText = _npcCanvas.transform.Find("Dialog Panel/Textbox Panel/Dialog Txt").GetComponent<Text>();
            Image dividerL = _npcCanvas.transform.Find("Dialog Panel/Name Holder/Left Divider").GetComponent<Image>();
            Text npcName = _npcCanvas.transform.Find("Dialog Panel/Name Holder/Name Txt").GetComponent<Text>();
            Image dividerR = _npcCanvas.transform.Find("Dialog Panel/Name Holder/Right Divider").GetComponent<Image>();
            Transform optionsPanel = _npcCanvas.transform.Find("Dialog Panel/Options Panel");
            
            Sprite divider = _npc.HasCustomSprites()? _npc.dialogSprites.nameDivider : 
                _gm.GetRandomSprites()[(int) Random.Range(2,3)];
            dividerL.sprite = divider;
            dividerR.sprite = divider;
            
            if (dialogText == null || npcName == null || optionsPanel == null)
            {
                EDebug.LogError("One or more UI components are not assigned.");
                return;
            }
            dialogText.text = dialogOption.npcDialog;
            npcName.text = _npc.HasCustomName()? _npc.entityName : MiscUtils.GetRandomName(
                _gm.randomNames, _npc.nameCustomization);
            
            foreach (Transform child in optionsPanel) { Destroy(child.gameObject); }
            for (int i = 0; i < dialogOption.userResponses.Count; i++)
            {
                GameObject resp = Instantiate(_responsePrefab, optionsPanel);
                Text responseText = resp.GetComponentInChildren<Text>();
                responseText.text = dialogOption.userResponses[i].response;
                int index = i;
                resp.GetComponent<Button>().onClick.AddListener(() => OnResponseSelected(dialogOption.userResponses[index]));
            }
            
            _npcCanvas.gameObject.SetActive(true);
        }
        
        private void OnResponseSelected(ResponseOption responseOption)
        {
            responseOption.OnResponse();
            _npc.ChangeMood(responseOption.moodChange);
            if (responseOption.nextDialog != null) DisplayDialog(responseOption.nextDialog);
            else StopDialog();
        }

        public void DisplayNpcPrompt(List<LivingEntity> npcNear)
        {
            _dialogPrompt = _gm.GetOrCreateNpcPromptCanvas();
            if (_dialogPrompt == null) return;
            Transform holder = _dialogPrompt.transform.Find("Holder");
            if (holder == null) return;
            foreach (Transform child in holder) // Clear existing name panels if any
            { Destroy(child.gameObject); }
            LivingEntity closestNpc = null; // Find closest NPC from the given list
            float closestDistance = 100000000000;
            foreach (LivingEntity npc in npcNear)
            {
                float distance = Vector3.Distance(_gm.player.transform.position, npc.transform.position);
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    closestNpc = npc;
                }
            }
            if (closestNpc == null) return;
            _dialogPrompt.transform.position = closestNpc.transform.Find("Root").transform.position;
            
            foreach (LivingEntity npc in npcNear)
            {
                GameObject namePanel = Instantiate(_gm.canvasPrefabs.promptName, holder);
                Text nameText = namePanel.transform.Find("Name Txt").GetComponent<Text>();
                nameText.text = npc.HasCustomName() ? npc.entityName : MiscUtils.GetRandomName(_gm.randomNames, npc.nameCustomization);
            }

            _dialogPrompt.SetActive(true);
        }
        
        public void RemoveNpcPrompt()
        {
            if (_dialogPrompt == null) return;
            Transform holder = _dialogPrompt.transform.Find("Holder");
            if (holder == null) return;
            foreach (Transform child in holder) { Destroy(child.gameObject); }
            _dialogPrompt.SetActive(false);
        }
        
    }
}
