using UnityEngine;

namespace ArenaSystemsLab
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health), typeof(SpriteRenderer))]
    public sealed class EnemyController : MonoBehaviour
    {
        private static readonly Color IdleColor = new Color(0.5f, 0.5f, 0.55f);
        private static readonly Color ChaseColor = new Color(1f, 0.25f, 0.3f);
        private static readonly Color AttackColor = new Color(1f, 0.65f, 0.1f);
        private static readonly Color DeadColor = new Color(0.15f, 0.15f, 0.15f);

        [SerializeField, Min(0f)] private float moveSpeed = 1.8f;
        [SerializeField, Min(1)] private int contactDamage = 10;
        [SerializeField, Min(0.1f)] private float damageInterval = 0.75f;

        private readonly EnemyStateMachine stateMachine = new EnemyStateMachine();
        private Rigidbody2D body;
        private Health health;
        private SpriteRenderer stateRenderer;
        private ArenaGame game;
        private Transform target;
        private Health targetHealth;
        private float nextDamageTime;
        private bool hasTargetContact;
        private bool initialized;

        public EnemyState CurrentState => stateMachine.CurrentState;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            stateRenderer = GetComponent<SpriteRenderer>();
            ApplyStateDebug();
        }

        private void Start()
        {
            if (!initialized)
            {
                Debug.LogError("EnemyController requires Initialize before the first frame.", this);
                enabled = false;
            }
        }

        private void FixedUpdate()
        {
            RefreshState();
            if (CurrentState != EnemyState.Chase)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction = (target.position - transform.position).normalized;
            body.MovePosition(body.position + direction * (moveSpeed * Time.fixedDeltaTime));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsTargetCollision(collision))
            {
                return;
            }

            hasTargetContact = true;
            RefreshState();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!IsTargetCollision(collision))
            {
                return;
            }

            hasTargetContact = true;
            RefreshState();
            if (CurrentState != EnemyState.Attack || Time.time < nextDamageTime)
            {
                return;
            }

            if (targetHealth.ApplyDamage(contactDamage))
            {
                nextDamageTime = Time.time + damageInterval;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!IsTargetCollision(collision))
            {
                return;
            }

            hasTargetContact = false;
            RefreshState();
        }

        private void OnDisable()
        {
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
            }
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Died -= HandleDeath;
            }
        }

        public void Initialize(ArenaGame arenaGame, Transform chaseTarget, Health chaseTargetHealth)
        {
            game = arenaGame;
            target = chaseTarget;
            targetHealth = chaseTargetHealth;
            initialized = game != null && target != null && targetHealth != null;
            health.Died += HandleDeath;

            if (!initialized)
            {
                Debug.LogError("EnemyController is missing the game or chase target.", this);
            }

            RefreshState();
        }

        public void ApplyDamage(int amount)
        {
            health.ApplyDamage(amount);
        }

        private void HandleDeath()
        {
            if (stateMachine.Evaluate(true, false, false))
            {
                ApplyStateDebug();
            }

            if (game != null)
            {
                game.RegisterEnemyDeath();
            }

            Destroy(gameObject);
        }

        private bool IsTargetCollision(Collision2D collision)
        {
            return initialized && collision.gameObject == target.gameObject;
        }

        private void RefreshState()
        {
            bool isDead = health != null && health.IsDead;
            bool canAct = initialized && !game.IsGameOver && !targetHealth.IsDead;
            if (stateMachine.Evaluate(isDead, canAct, hasTargetContact))
            {
                ApplyStateDebug();
            }
        }

        private void ApplyStateDebug()
        {
            gameObject.name = $"Enemy [{CurrentState}]";
            if (stateRenderer == null)
            {
                return;
            }

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    stateRenderer.color = IdleColor;
                    break;
                case EnemyState.Chase:
                    stateRenderer.color = ChaseColor;
                    break;
                case EnemyState.Attack:
                    stateRenderer.color = AttackColor;
                    break;
                case EnemyState.Dead:
                    stateRenderer.color = DeadColor;
                    break;
            }
        }
    }
}
