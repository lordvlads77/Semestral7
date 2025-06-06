using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Random = UnityEngine.Random;

namespace Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    [SelectionBase]
    public class Enemy : LivingEntity
    {
         private readonly int _animDirV = Animator.StringToHash("DirV");
        private readonly int _animDirH = Animator.StringToHash("DirH");
        private readonly int _animSpeed = Animator.StringToHash("Speed");
        private readonly int _animHealth = Animator.StringToHash("Health%");
        private readonly int _animType = Animator.StringToHash("AnimType");
        private readonly int _animAttack = Animator.StringToHash("Attack");
        private readonly int _animHit = Animator.StringToHash("Hit");
        private readonly int _animDying = Animator.StringToHash("Dying");
        private readonly int _animDead = Animator.StringToHash("Dead");
        private readonly int _animFlee = Animator.StringToHash("Flee");
        
        [Header("Enemy stuff")]
        private Transform _home;
        [SerializeField, Range(1,50)] private float homeRadius = 5f;
        [SerializeField] private bool homeBound = true; // Will the enemy return to home if it's outside the home radius?
        private bool _coward;
        private float HealthPercent => GetHealth() / maxHealth;
        private Coroutine _dieRoutine;
        private Coroutine _attackRoutine;
        [SerializeField] private Weapon weapon;
        [SerializeField] private ParticleSystem[] ohImDie;
        private bool _isKnockedBack = false;
        [SerializeField] private float knockbackForce = 2f;
        [SerializeField] private float knockbackDuration = 0.25f;

        [Header("AI Components")] 
        [SerializeField, Range(2f, 50f)] private float detectionRange;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private LivingEntity player;
        [SerializeField] public string enemyID;

        [Header("Enemy properties")]
        public EnemyState curState = EnemyState.Idle;
        [SerializeField, Range(0f,4f)] private float walkSpeed = 1.0f;
        [SerializeField, Range(0f,8f)] private float runSpeed = 1.0f;
        [SerializeField] private Vector2 minMaxAttackCooldown = new (0.5f , 2.5f);
        private float _attackCooldown;
        [SerializeField] private float timeWithinAttackRange = 0f;
        [SerializeField, Range(0f,3f)] private float attackRange = 0.75f;
        
        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType = EnemyType.Melee;

        private Coroutine _imDieCoroutine;

        private void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            _coward = Random.value < 0.3f;
            agent = GetComponent<NavMeshAgent>();
            // GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
            // EDebug.Assert(tempPlayer != null, "Couldn't find player character", this);
            // player = tempPlayer.GetComponent<LivingEntity>();
            if (!string.IsNullOrEmpty(enemyID))
            {
                EnemyTracker.Register(enemyID);
            }
            if (!string.IsNullOrEmpty(enemyID) && PlayerPrefs.GetInt($"Enemy_{enemyID}_IsDead", 0) == 1)
            {
                Destroy(gameObject);
                return;
            }
            agent.speed = walkSpeed;
            runSpeed = Mathf.Max(walkSpeed, runSpeed);
            SearchForHome();
            Animator.SetFloat(_animHealth, HealthPercent);
            Animator.enabled = (gameState == GameStates.Playing);
            InvokeRepeating(nameof(LazyUpdate), 1f, 1f);
        }

        private void SearchForHome()
        {
            Transform spawnPoint = null;
            Collider[] colliders = Physics.OverlapSphere(transform.position, homeRadius);
            foreach (Collider coll in colliders)
            {
                if (!coll.CompareTag("EnemySpawnPoint")) continue;
                spawnPoint = coll.transform;
                break;
            }
            if (spawnPoint == null)
            {
                GameManager gm = MiscUtils.GetOrCreateGameManager();
                if (gm.EnemySpawnHolder == null)
                {
                    gm.GetOrCreateEnemySpawnHolder();
                }
                GameObject newSpawnPoint = new GameObject("EnemySpawnPoint") {
                    tag = "EnemySpawnPoint", transform = { position = transform.position } };
                SphereCollider sphere = newSpawnPoint.AddComponent<SphereCollider>();
                sphere.radius = 0.1f; sphere.isTrigger = true;
                newSpawnPoint.transform.SetParent(MiscUtils.GetOrCreateGameManager().EnemySpawnHolder.transform);
                spawnPoint = newSpawnPoint.transform;
            }
            _home = spawnPoint;
        }

        private void LazyUpdate() // This updates only once per second
        {
            if (gameState is GameStates.Paused or GameStates.Joining) return;
            UpdateStates();
        }

        private bool PlayerInRange() {
            return Vector3.Distance(player.transform.position, transform.position) < detectionRange;
        }

        private bool InHomeRange()
        {
            return Vector3.Distance(transform.position, _home.position) <= homeRadius;
        }

        private void UpdateStates()
        {
            if (gameState is GameStates.Paused or GameStates.Joining) return;
            switch (curState)
            {
                default:
                case EnemyState.Idle:
                    if (PlayerInRange())
                    {
                        curState = EnemyState.Chasing;
                        break;
                    }
                    if (MathUtils.WeightedRandBool(0.3f))
                    {
                        Vector3 randomDestination = (homeBound)? 
                            MathUtils.RandomPos(homeRadius, _home.position) :
                            MathUtils.RandomPos(2f, transform.position);
                        GoToDestination(randomDestination);
                    }
                    break;
                case EnemyState.Chasing:
                    if (!PlayerInRange())
                    {
                        if (homeBound)
                        {
                            curState = EnemyState.Fleeing;
                            GoToDestination(_home.position);
                            break;
                        }
                        curState = EnemyState.Idle;
                    }
                    break;
            }
        }

        [ContextMenu("On Damage Taken")] protected override void OnDamageTaken()
        {
            Animator.SetFloat(_animHealth, HealthPercent);
            float[] possibleValues = { 0f, 0.5f, 1f };
            float rand = possibleValues[Random.Range(0, possibleValues.Length)];
            Animator.SetFloat(_animType, rand);
            Animator.SetTrigger(_animHit);
            HurtFX?.Hit(hurtFXVars);
            Vector3 awayFromPlayer = (transform.position - player.transform.position).normalized;
            Knockback(awayFromPlayer);
            if (curState is EnemyState.Idle or EnemyState.Chasing && _coward && HealthPercent <= 0.25f && !InHomeRange())
                curState = EnemyState.Fleeing;
        }

        private void FixedUpdate()
        {
            if (gameState != GameStates.Playing || curState == EnemyState.Dead || curState == EnemyState.Dying)
                return;
    
            switch (curState)
            {
                case EnemyState.Idle:
                    agent.speed = walkSpeed;
                    return;
                case EnemyState.Chasing:
                    agent.speed = runSpeed;
                    GoToDestination(MathUtils.RandomPos(0.25f, player.transform.position));
                    if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                        StartCoroutine(AttackRoutine());
                    break;
                case EnemyState.Fleeing:
                    if (InHomeRange())
                        curState = EnemyState.Idle;
                    else
                    {
                        agent.speed = runSpeed * 1.25f;
                        GoToDestination(_home.position);
                    }
                    break;
            }
        }

        private void LateUpdate()
        {
            if (gameState != GameStates.Playing) return;
            Vector3 toPlayer = (player.transform.position - transform.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(agent.velocity.normalized);
            float x = 0, y = 0;
            if (PlayerInRange()) {
                Vector3 lookDirection = new Vector3(toPlayer.x, 0, toPlayer.z);
                transform.rotation = Quaternion.LookRotation(lookDirection);
                y = Mathf.Clamp(localDirection.z, -1f, 1f);
                x = Mathf.Clamp(localDirection.x, -1f, 1f);
            } else {
                y = Mathf.Clamp(localDirection.z, -1f, 1f);
                x = Mathf.Clamp(localDirection.x, -1f, 1f);
            }
            Animator.SetFloat(_animDirV, y);
            Animator.SetFloat(_animDirH, x);
        }

        private IEnumerator AttackRoutine()
        {
            if (_attackRoutine != null || curState == EnemyState.Dying || curState == EnemyState.Dead)
                yield break;
    
            EDebug.Log("AttackRoutine called by:" + this.entityName.ToString());
            _attackCooldown = Random.Range(minMaxAttackCooldown.x, minMaxAttackCooldown.y);
            _attackRoutine = StartCoroutine(PerformAttack());
        }

        private IEnumerator PerformAttack()
        {
            weapon.inUse = true;
            Animator.SetInteger(_animType, (MathUtils.RandBool())? 0 : 1);
            Animator.SetTrigger(_animAttack);
            yield return new WaitForSeconds(0.5f);
            while (Animator.GetCurrentAnimatorStateInfo(0).IsName("AttackA") || 
                   Animator.GetCurrentAnimatorStateInfo(0).IsName("AttackB"))
            { yield return null; }
            weapon.inUse = false;
            yield return new WaitForSeconds(_attackCooldown);
            _attackRoutine = null;
        }
        public void Knockback(Vector3 direction)
        {
            if (_isKnockedBack || curState == EnemyState.Dying || curState == EnemyState.Dead)
                return;

            StartCoroutine(KnockbackRoutine(direction));
        }

        private IEnumerator KnockbackRoutine(Vector3 direction)
        {
            _isKnockedBack = true;
            agent.isStopped = true;

            float timer = 0f;
            Vector3 start = transform.position;
            Vector3 end = start + direction.normalized * knockbackForce;

            while (timer < knockbackDuration)
            {
                transform.position = Vector3.Lerp(start, end, timer / knockbackDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            agent.isStopped = false;
            _isKnockedBack = false;
        }
        private IEnumerator DieRoutine()
        {
            //_animDead / _animDying
            yield return new WaitForSeconds(1f);
            curState = EnemyState.Dead;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            EDebug.Log(other, this);
            EDebug.Log(other.tag, this);
            if (other.CompareTag("Weapon"))
            {
                Weapon weapon = other.GetComponent<Weapon>();
                if (weapon != null && weapon.inUse)
                {
                    LivingEntity attacker = weapon.GetComponentInParent<LivingEntity>();
                    if (attacker != null && attacker != this) // Asegura que no se golpee a sí mismo
                    {
                        CombatUtils.Attack(attacker, this);
                    }
                }
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
            if (curState == EnemyState.Dying || curState == EnemyState.Dead) return;

            base.Die();
            Animator.SetBool("Dead", true);
            Animator.SetInteger("AnimType", Random.Range(0, 1));
            curState = EnemyState.Dying;

            // ✅ Detener completamente el agente de navegación
            if (agent != null)
            {
                agent.ResetPath();            // Borra la ruta actual
                agent.isStopped = true;      // Detiene el movimiento
                agent.enabled = false;       // Opcional: desactiva por completo
            }

            if (!string.IsNullOrEmpty(enemyID))
            {
                PlayerPrefs.SetInt($"Enemy_{enemyID}_IsDead", 1); // Guardamos que está muerto
                PlayerPrefs.Save(); // Muy importante: guarda en disco
            }
            
            if (_imDieCoroutine == null)
                _imDieCoroutine = StartCoroutine(OhImDieThankYouForever());
        }

        private IEnumerator OhImDieThankYouForever()
        {
            yield return new WaitForSeconds(3f);
            if (ohImDie.Length > 0) {
                foreach (ParticleSystem ps in ohImDie) {
                    ParticleSystem newPs = Instantiate(ps, this.transform.position, this.transform.rotation);
                    newPs.Play();
                }
            }
            if (!string.IsNullOrEmpty(enemyID))
            {
                EnemyTracker.Unregister(enemyID);
            }
            Destroy(this.gameObject);
        }
        
        protected override void OnStateChange(GameStates state)
        {
            gameState = state;
            if(gameState != GameStates.Playing) {
                agent.isStopped = true;
                Animator.enabled = false;
            }
            else {
                agent.isStopped = false;
                Animator.enabled = true;
            }
        }

    }


}
