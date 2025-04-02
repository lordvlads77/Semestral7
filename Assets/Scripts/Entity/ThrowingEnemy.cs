using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Entity
{
    public sealed class ThrowingEnemy : Utils.LivingEntity
    {

        [SerializeField] public Animator animator;

        private int castAnimationID = Animator.StringToHash("enemy_cast");

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
            animator = GetComponentInChildren<Animator>();


            GameObject temp_player = GameObject.FindGameObjectWithTag("Player");
            EDebug.Assert(temp_player != null, "could not find player character", this);
            Debug.Assert(PreFab != null, "Enemy Needs Prefab to work", this);
            player = temp_player.GetComponent<LivingEntity>();
            agent.speed = speed;
            agent.updatePosition = false;

            SetHealth(health);
        }

        void Update()
        {
            if (gameState != GameStates.Playing) { return; }


            Vector3 player_position = player.transform.position;
            float distance = Vector3.Distance(player_position, transform.position);
            if (distance < attackRange)
            {
                agent.SetDestination(player_position);
                FacePlayer();
                timeInsdeAttackRange += Time.deltaTime;
                animator.SetBool(castAnimationID, true);
            }
            else
            {
                timeInsdeAttackRange = 0;
                //animator.SetBool(castAnimationID, false);
            }

            if (timeInsdeAttackRange >= attackCooldown)
            {
                Attack();
                timeInsdeAttackRange = 0.0f;
            }

        }

        private void OnEnable()
        {
            GameManager.Instance.Subscribe(OnGameStateChange);
            gameState = GameManager.Instance.GameState;
        }

        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
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

        public void Attack()
        {
            Projectile final_projectile = Recycle();

            Vector3 direction = (player.transform.position - transform.position).normalized;

            if (final_projectile == null)
            {
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

        public void OnGameStateChange(GameStates _newState)
        {
            gameState = _newState;
        }


        protected override void Die()
        {
            gameObject.SetActive(false);
        }

        private void FacePlayer()
        {
            var turnTowardNavSteeringTarget = agent.steeringTarget;

            Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }
    }
}
