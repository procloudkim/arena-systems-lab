using UnityEngine;

namespace ArenaSystemsLab
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float spawnInterval = 1.1f;
        [SerializeField, Min(1)] private int enemyHealth = 20;

        private ArenaGame game;
        private Transform player;
        private Health playerHealth;
        private Transform enemyParent;
        private Sprite enemySprite;
        private float nextSpawnTime;
        private bool initialized;

        private void Start()
        {
            if (!initialized)
            {
                Debug.LogError("EnemySpawner requires Initialize before the first frame.", this);
                enabled = false;
                return;
            }

            nextSpawnTime = Time.time + 0.5f;
        }

        private void Update()
        {
            if (!game.CanSpawnEnemy || Time.time < nextSpawnTime)
            {
                return;
            }

            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        public void Initialize(ArenaGame arenaGame, Transform target, Health targetHealth, Transform parent, Sprite sprite)
        {
            game = arenaGame;
            player = target;
            playerHealth = targetHealth;
            enemyParent = parent;
            enemySprite = sprite;
            initialized = game != null && player != null && playerHealth != null && enemyParent != null && enemySprite != null;

            if (!initialized)
            {
                Debug.LogError("EnemySpawner is missing a required reference.", this);
            }
        }

        private void SpawnEnemy()
        {
            GameObject enemy = new GameObject("Enemy");
            enemy.transform.SetParent(enemyParent);
            enemy.transform.position = RandomEdgePosition();
            enemy.transform.localScale = new Vector3(0.65f, 0.65f, 1f);

            SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
            renderer.sprite = enemySprite;
            renderer.color = new Color(1f, 0.25f, 0.3f);
            renderer.sortingOrder = 1;

            Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
            collider.radius = 0.48f;

            Health health = enemy.AddComponent<Health>();
            health.Configure(enemyHealth);

            EnemyController controller = enemy.AddComponent<EnemyController>();
            controller.Initialize(game, player, playerHealth);
            game.RegisterEnemySpawned();
        }

        private static Vector2 RandomEdgePosition()
        {
            if (Random.value < 0.5f)
            {
                float x = Random.value < 0.5f ? -ArenaGame.HalfWidth : ArenaGame.HalfWidth;
                return new Vector2(x, Random.Range(-ArenaGame.HalfHeight, ArenaGame.HalfHeight));
            }

            float y = Random.value < 0.5f ? -ArenaGame.HalfHeight : ArenaGame.HalfHeight;
            return new Vector2(Random.Range(-ArenaGame.HalfWidth, ArenaGame.HalfWidth), y);
        }
    }
}
