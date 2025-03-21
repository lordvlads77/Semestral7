using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Entity
{
    public sealed class ThrowingEnemy : Utils.LivingEntity
    {
        [Header("Enemy type")]
        [SerializeField] private ENEMY_TYPE type;
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

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            GameObject temp_player = GameObject.FindGameObjectWithTag("Player");
            EDebug.Assert(temp_player != null, "could not find player character", this);
            player = temp_player.GetComponent<LivingEntity>();
            agent.speed = speed;
            SetHealth(health);
        }

        void Update()
        {
            if (gameState != GameStates.Playing) { return; }

        }
    }
}
