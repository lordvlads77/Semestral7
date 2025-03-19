using System;
using Entity;
using UnityEngine;
using Utils;

public sealed class TestDummy : MonoBehaviour
{
    private Dummy _dummy = default;
    [SerializeField] private bool isDummyBeingTested = false;
    [SerializeField] public Vector3 hitDirection = Vector3.forward;
    public event Action OnHit;
    
    private void Start()
    {
        _dummy = GetComponent<Dummy>();
        EDebug.Assert(_dummy != null, "Attach me to a object with the Dummy script please");
    }

    private void OnEnable()
    {
        if (isDummyBeingTested)
        {
            OnHit += () =>
            {
                //_dummy.TakeDamage(10, _dummy.transform.position, hitDirection);
                Animator animator = this.GetComponent<Animator>();
                animator.SetTrigger("Hit");
            };
            
            Input.Actions.Instance.OnAttackTriggeredEvent += OnHit;
        }
    }
    
    private void OnDisable()
    {
        if (isDummyBeingTested)
        {
            Input.Actions.Instance.OnAttackTriggeredEvent -= OnHit;
        }
    }

}
