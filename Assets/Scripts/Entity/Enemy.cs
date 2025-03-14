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
        [Header("AI Components")]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] LivingEntity player;

        private float realHealth = 10f;

        [Header("Enemy properties")]
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
            agent.speed = speed;
            SetHealth(health);
        }

        private void Update()
        {
            if (gameState != GameStates.Playing) { return; }


            Vector3 PlayerPosition = player.transform.position;
            GoToDestination(PlayerPosition);
            float distance = Vector3.Distance(PlayerPosition, agent.transform.position);

            //EDebug.Log("agent radius = " + agent.radius + "\nDistance = " + distance);
            if (!(attackRange >= distance))
            {
                timeInsdeAttackRange = 0.0f;
                return;
            }

            timeInsdeAttackRange += Time.deltaTime;
            if (timeInsdeAttackRange > attackCooldown)
            {
                Vector3 direction = PlayerPosition - transform.position;
                timeInsdeAttackRange = 0.0f;
                player.TakeDamage(damage, transform.position, direction);
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
            gameObject.SetActive(false);
        }

        public void OnStateChange(GameStates state)
        {
            gameState = state;
        }
    }

}
