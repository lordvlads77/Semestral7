using FMOD;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

public class TriggerOnConditions : MonoBehaviour
{
    [Header("Conditions")]
    [SerializeField] private CustomCondition[] conditions;
    [SerializeField] private bool triggersMoreThanOnce;
    [Header("For Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerName;
    [SerializeField] private bool playAnimation;
    [Header("For GameObjects")]
    [SerializeField] private GameObject objectToDoStuffOn;
    [SerializeField] private bool disableGameObject;
    [SerializeField] private bool destroyGameObject;
    [SerializeField] private bool enableGameObject;
    [Header("For FX")]
    [SerializeField] private bool playParticle;
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private bool playSound;
    [SerializeField] private PlayPersistent soundToPlay;

    private bool _triggered;
    
    private void Awake()
    {
        foreach (var cond in conditions) {
            cond.SetManager(this);
        }
    }
    
    public void OnConditionMet(CustomCondition condition)
    {
        Debug.Log($"Condition met for: {condition.name}");
        ValidateConditions();
    }
    
    private void ValidateConditions()
    {
        int metCount = 0;
        foreach (var cond in conditions) {
            if (cond.ConditionMet)
                metCount++;
        }
        if (metCount == conditions.Length) {
            if (_triggered && !triggersMoreThanOnce) return;
            _triggered = true;
            Debug.Log("All conditions met!! :O");
            if (playAnimation && animator != null && !string.IsNullOrWhiteSpace(animationTriggerName))
                MiscUtils.ActionToDo(animator, animationTriggerName, null, 0, null, null);
            if (disableGameObject && objectToDoStuffOn != null)
                MiscUtils.ActionToDo(null, null, objectToDoStuffOn, 0, null, null);
            if (destroyGameObject && objectToDoStuffOn != null)
                MiscUtils.ActionToDo(null, null, objectToDoStuffOn, 1, null, null);
            if (enableGameObject && objectToDoStuffOn != null)
                MiscUtils.ActionToDo(null, null, objectToDoStuffOn, 2, null, null);
            if (playParticle && particleSys != null)
                MiscUtils.ActionToDo(null, null, null, 0, particleSys, null);
            if (playSound && soundToPlay != null)
                MiscUtils.ActionToDo(null, null, null, 0, null, soundToPlay);
        }
    }
}
