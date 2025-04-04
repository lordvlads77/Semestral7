using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : LivingEntity
    {

        [SerializeField] BoxCollider boxCollider;
        [SerializeField] Rigidbody body;

        [SerializeField] public Animator animator;

        private readonly int chaseAnimationID = Animator.StringToHash("enemy_chase");
        private readonly int attackAnimationID = Animator.StringToHash("enemy_attack");

        [Header("AI Components")]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] LivingEntity player;

        private float realHealth = 10f;

        [Header("Enemy properties")]
        [SerializeField] public Utils.ENEMY_STATE enemy_state = Utils.ENEMY_STATE.ALIVE;

        [Range(0f, 20f)]
        [SerializeField] float damage = 1.0f;

        [Range(0f, 5f)]
        [SerializeField] float speed = 1.0f;

        [field: Range(0f, 100f)]
        [field: SerializeField]
        float health
        {
            get { return realHealth; }
            set
            {
                realHealth = value;
                this.SetHealth(realHealth);
            }
        }

        [SerializeField] float attackCooldown = .4f;
        [SerializeField] float timeInsdeAttackRange = 0f;


        [Range(0f, 5f)]
        [SerializeField] float attackRange = 1.0f;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            GameObject temp_player = GameObject.FindGameObjectWithTag("Player");
            EDebug.Assert(temp_player != null, "could not find player character", this);
            player = temp_player.GetComponent<LivingEntity>();


            boxCollider = GetComponentInChildren<BoxCollider>();
            body = GetComponentInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();


            animator.SetBool(chaseAnimationID, true);

            agent.speed = speed;

            SetHealth(health);
        }

        private void Update()
        {
            if (gameState != GameStates.Playing) { return; }


            Vector3 PlayerPosition = player.transform.position;
            GoToDestination(PlayerPosition);
            float distance = Vector3.Distance(PlayerPosition, agent.transform.position);

            if (enemy_state == ENEMY_STATE.DYING)
            {
                if (animator.GetBool("enemy_is_dead"))
                {
                    enemy_state = ENEMY_STATE.DEAD;
                    gameObject.SetActive(false);
                }
                return;
            }

            //EDebug.Log("agent radius = " + agent.radius + "\nDistance = " + distance);
            if (!(attackRange >= distance) && enemy_state == ENEMY_STATE.ALIVE)
            {
                timeInsdeAttackRange = 0.0f;
                animator.SetBool(attackAnimationID, false);
                return;
            }

            timeInsdeAttackRange += Time.deltaTime;
            if (timeInsdeAttackRange > attackCooldown)
            {

                Vector3 direction = PlayerPosition - transform.position;
                timeInsdeAttackRange = 0.0f;
                animator.SetBool(attackAnimationID, true);

                //player.TakeDamage(damage, transform.position, direction);
                CombatUtils.Attack(this, player);
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.Subscribe(OnStateChange);
            OnStateChange(GameManager.Instance.GameState);
        }

        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnStateChange);
        }

        private void OnTriggerEnter(Collider other)
        {
            EDebug.Log(other, this);
            EDebug.Log(other.tag, this);
            if (other.CompareTag("Weapon"))
            {
                CombatUtils.Attack(player, this);
            }
        }

        /// <summary>
        /// Tell's the enemy were to go
        /// </summary>
        /// <param name="destination">position to go to</param>
        public void GoToDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }

        /// <summary>
        /// Tell's the enemy were to go
        /// </summary>
        /// <param name="destination">position to go to</param>
        public void GoToDestination(Transform destination)
        {
            GoToDestination(destination.position);
        }

        protected override void Die()
        {
            base.Die();
            //gameObject.SetActive(false);
            animator.SetBool("enemy_dead", true);
            enemy_state = ENEMY_STATE.DYING;
        }

        public void OnStateChange(GameStates state)
        {
            gameState = state;
        }

    }

}
