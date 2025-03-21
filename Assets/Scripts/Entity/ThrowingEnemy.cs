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

        [Header("Throwing object")]
        [SerializeField] GameObject prefab;

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

        [SerializeField] float attackCooldown = 1.4f;

        [SerializeField] float attackRange = 5.0f;

        [SerializeField] float timeInsdeAttackRange = 0f;

        [SerializeField] Projectile PreFab;
        [SerializeField] List<Projectile> Spawned;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            GameObject temp_player = GameObject.FindGameObjectWithTag("Player");
            EDebug.Assert(temp_player != null, "could not find player character", this);
            player = temp_player.GetComponent<LivingEntity>();
            agent.speed = speed;

            gameState = GameStates.Playing;
            SetHealth(health);
        }

        void Update()
        {
            if (gameState != GameStates.Playing) { return; }


            float distance = Vector3.Distance(player.transform.position, transform.position);
            EDebug.Log("Distance from Enemy" + distance);
            if (distance < attackRange)
            {
                timeInsdeAttackRange += Time.deltaTime;
            }
            else
            {
                timeInsdeAttackRange = 0;
            }

            if (timeInsdeAttackRange >= attackCooldown)
            {
                Attack();
                timeInsdeAttackRange = 0.0f;
            }

        }

        public void Attack()
        {
            Projectile final_projectile = Recycle();

            Vector3 direction = (player.transform.position - transform.position).normalized;
            
            if (final_projectile == null) {
                Projectile go = Instantiate(PreFab, transform.position + (direction * 2.0f), Quaternion.identity);
                go.setDestination(player.transform.position);
                Spawned.Add(go);
                return;
            }

            final_projectile.gameObject.SetActive(true);
            final_projectile.transform.position = transform.position + (direction * 2.0f);
            final_projectile.setDestination(player.transform.position);
        }

        public Projectile Recycle()
        {
            Projectile result = null;
            foreach (Projectile p in Spawned)
            {
                if (!p.gameObject.activeInHierarchy)
                {
                    result = p; 
                    break;
                }
            }
            return result;
        }

        public void Prepare(Projectile pro)
        {

        }
    }
}
