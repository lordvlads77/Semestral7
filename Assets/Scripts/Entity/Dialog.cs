using System.Collections;
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
            StartCoroutine(InitializeWithRetry());
        }
        
        private IEnumerator InitializeWithRetry()
        {
            while (!Singleton<GameManager>.HasInstance)
            {
                EDebug.LogWarning("GameManager is not ready yet. Retrying in 0.25s...");
                yield return new WaitForSeconds(0.25f);
            }
            _gm = Singleton<GameManager>.Instance;
            yield return new WaitForEndOfFrame();
            _npcCanvas = _gm.NpcCanvas;
            if (_npcCanvas == null)
            {
                EDebug.Log("Creating new NPC Canvas...");
                _npcCanvas = _gm.GetOrCreateNpcCanvas();
            }
            _responsePrefab = _gm.canvasPrefabs.npcOption;
            _npcCanvas.enabled = false;
            EDebug.Log("Dialog ► Initialized successfully.");
        }

        public void StartDialog(LivingEntity npc)
        {
            _npc = npc;
            _npc.isInDialog = true;
            _dialog = _npc.dialogOptions;
            if (_dialog != null && _dialog.dialogOptions.Count > 0) 
                DisplayDialog(_dialog.dialogOptions[0]);
            else 
                EDebug.LogError(Localization.Translate("log.dialog_null_empty"));
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
            dialogText.text = Localization.Translate(dialogOption.npcDialog);
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
            EDebug.Log("DisplayNpcPrompt");
            if (npcNear.Count == 0) {
                EDebug.LogWarning(StringUtils.AddColorToString("No NPCs near by were sent to the function", Color.yellow));
                RemoveNpcPrompt();
                _lastNearbyNpc = new List<LivingEntity>();
                return;
            }
            if (_lastNearbyNpc.Count == npcNear.Count && !_lastNearbyNpc.Except(npcNear).Any()) return;
            _lastNearbyNpc = new List<LivingEntity>(npcNear);
            _dialogPrompt = _gm.GetOrCreateNpcPromptCanvas();
            if (_dialogPrompt == null) return;
            Transform holder = _dialogPrompt.transform.Find("Holder");
            if (holder == null) return;
            EDebug.Log(StringUtils.AddColorToString("Removed old name panels", Color.red));
            foreach (Transform child in holder) // Clear existing name panels if any
            { Destroy(child.gameObject); }
            LivingEntity closestNpc = null; // Find closest NPC from the given list
            float closestDistance = float.MaxValue;
            foreach (LivingEntity npc in npcNear) {
                float distance = Vector3.Distance(_gm.player.transform.position, npc.transform.position);
                EDebug.Log($"NPC: {npc.entityName}, Distance: {distance}", this);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestNpc = npc;
                }
            }
            if (closestNpc == null) return;
            EDebug.Log($"Closest NPC: {closestNpc.entityName}, Distance: {closestDistance}", this);
            Transform rootTransform = null;
            foreach (Transform child in closestNpc.transform) {
                if (child.name == "Root") {
                    rootTransform = child;
                    break;
                }
            }
            if (rootTransform == null)
                EDebug.LogWarning($"Root transform not found for {closestNpc.entityName}. Ensure it exists as a direct child.");
            if (rootTransform != null) _dialogPrompt.transform.position = rootTransform.position;
            else EDebug.LogWarning("Root transform not found for closest NPC.");
            foreach (LivingEntity npc in npcNear) {
                GameObject namePanel = Instantiate(_gm.canvasPrefabs.promptName, holder);
                Text nameText = namePanel.transform.Find("Name Txt")?.GetComponent<Text>();
                if (nameText != null)
                    nameText.text = npc.HasCustomName() ? npc.entityName : MiscUtils.GetRandomName(_gm.randomNames, npc.nameCustomization);
                else EDebug.LogWarning("Name Txt not found in promptName prefab.");
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
