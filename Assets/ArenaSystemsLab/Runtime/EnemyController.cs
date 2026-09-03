using UnityEngine;

namespace ArenaSystemsLab
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 1.8f;
        [SerializeField, Min(1)] private int contactDamage = 10;
        [SerializeField, Min(0.1f)] private float damageInterval = 0.75f;

        private Rigidbody2D body;
        private Health health;
        private ArenaGame game;
        private Transform target;
        private Health targetHealth;
        private float nextDamageTime;
        private bool initialized;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
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
            if (game.IsGameOver || targetHealth.IsDead)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction = (target.position - transform.position).normalized;
            body.MovePosition(body.position + direction * (moveSpeed * Time.fixedDeltaTime));
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (game.IsGameOver || Time.time < nextDamageTime || collision.gameObject != target.gameObject)
            {
                return;
            }

            if (targetHealth.ApplyDamage(contactDamage))
            {
                nextDamageTime = Time.time + damageInterval;
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
        }

        public void ApplyDamage(int amount)
        {
            health.ApplyDamage(amount);
        }

        private void HandleDeath()
        {
            game.RegisterEnemyDeath();
            Destroy(gameObject);
        }
    }
}
