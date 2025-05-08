using UnityEngine;
using Utils;

namespace Entity
{
    public sealed class Dummy : LivingEntity
    {
        private readonly int _animHit = Animator.StringToHash("Hit");   // Trigger
        private readonly int _animDie = Animator.StringToHash("Died");  // Boolean
        
        [SerializeField] private bool canDie = true;
        [SerializeField] private ParticleSystem particle;
        
        [ContextMenu("On Damage Taken")] protected override void OnDamageTaken()
        {
            Animator.SetTrigger(_animHit);
            HurtFX?.Hit(hurtFXVars);
            if (!canDie) SetHealth(maxHealth);
        }

        [ContextMenu("Die")] protected override void Die()
        {
            if (!canDie) return;
            Animator.SetBool(_animDie, true);
        }
        
        protected override void PlayParticleEffect(ParticleSystem particlePrefab, Vector3 position, Vector3 direction)
        {
            if (particle == null) {
                Debug.LogWarning("No particle system assigned.");
                return;
            }
            particle.gameObject.SetActive(false);
            particle.Stop();
            particle.gameObject.SetActive(true);
            particle.Play();
        }

    }
}
