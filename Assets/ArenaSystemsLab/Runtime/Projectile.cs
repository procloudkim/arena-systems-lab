using UnityEngine;

namespace ArenaSystemsLab
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float lifetime = 2f;

        private Rigidbody2D body;
        private int damage;
        private bool initialized;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (!initialized)
            {
                Debug.LogError("Projectile requires Initialize before the first frame.", this);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                return;
            }

            enemy.ApplyDamage(damage);
            Destroy(gameObject);
        }

        public void Initialize(Vector2 direction, float speed, int hitDamage)
        {
            if (speed <= 0f || hitDamage <= 0)
            {
                Debug.LogError("Projectile speed and damage must be positive.", this);
                Destroy(gameObject);
                return;
            }

            damage = hitDamage;
            initialized = true;
            body.linearVelocity = direction.normalized * speed;
            Destroy(gameObject, lifetime);
        }
    }
}
