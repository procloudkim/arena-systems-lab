using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaSystemsLab
{
    public sealed class ArenaGame : MonoBehaviour
    {
        public const float HalfWidth = 8.5f;
        public const float HalfHeight = 5f;

        private const int MaxEnemies = 40;
        private const string ControlsText = "Move: WASD / Arrows / Left Stick    Attack: Left Click / Enter / West Button";
        private const string GameOverText = "GAME OVER\nPress R to restart";

        private Camera arenaCamera;
        private Sprite squareSprite;
        private Transform roundRoot;
        private Health playerHealth;
        private PlayerController playerController;
        private EnemySpawner spawner;
        private GUIStyle hudStyle;
        private GUIStyle gameOverStyle;
        private string hudText;
        private int score;
        private int enemyCount;
        private int cachedHealth = -1;
        private int cachedScore = -1;
        private int cachedEnemyCount = -1;
        private bool isGameOver;

        public bool IsGameOver => isGameOver;
        public bool CanSpawnEnemy => !isGameOver && enemyCount < MaxEnemies;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<ArenaGame>() == null)
            {
                new GameObject("Arena Systems Lab").AddComponent<ArenaGame>();
            }
        }

        private void Awake()
        {
            squareSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);

            ConfigureCamera();
            StartRound();
        }

        private void Update()
        {
            if (isGameOver && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                StartRound();
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= HandlePlayerDied;
            }

            if (squareSprite != null)
            {
                Destroy(squareSprite);
            }
        }

        private void OnGUI()
        {
            EnsureGuiStyles();
            RefreshHudText();

            GUI.Box(new Rect(12f, 12f, 430f, 72f), GUIContent.none);
            GUI.Label(new Rect(24f, 18f, 400f, 28f), hudText, hudStyle);
            GUI.Label(new Rect(24f, 48f, 400f, 24f), ControlsText, hudStyle);

            if (isGameOver)
            {
                GUI.Box(new Rect(Screen.width * 0.5f - 180f, Screen.height * 0.5f - 70f, 360f, 140f), GUIContent.none);
                GUI.Label(new Rect(Screen.width * 0.5f - 170f, Screen.height * 0.5f - 55f, 340f, 110f), GameOverText, gameOverStyle);
            }
        }

        public void RegisterEnemySpawned()
        {
            enemyCount++;
        }

        public void RegisterEnemyDeath()
        {
            enemyCount = Mathf.Max(0, enemyCount - 1);
            score++;
        }

        private void ConfigureCamera()
        {
            arenaCamera = Camera.main;
            if (arenaCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                arenaCamera = cameraObject.AddComponent<Camera>();
            }

            arenaCamera.orthographic = true;
            arenaCamera.orthographicSize = 6.25f;
            arenaCamera.transform.position = new Vector3(0f, 0f, -10f);
            arenaCamera.backgroundColor = new Color(0.035f, 0.045f, 0.07f);
        }

        private void StartRound()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= HandlePlayerDied;
            }

            if (roundRoot != null)
            {
                roundRoot.gameObject.SetActive(false);
                Destroy(roundRoot.gameObject);
            }

            score = 0;
            enemyCount = 0;
            cachedHealth = -1;
            cachedScore = -1;
            cachedEnemyCount = -1;
            isGameOver = false;

            roundRoot = new GameObject("Round").transform;
            roundRoot.SetParent(transform);

            Transform player = CreatePlayer();
            GameObject spawnerObject = new GameObject("Enemy Spawner");
            spawnerObject.transform.SetParent(roundRoot);
            spawner = spawnerObject.AddComponent<EnemySpawner>();
            spawner.Initialize(this, player, playerHealth, roundRoot, squareSprite);
        }

        private Transform CreatePlayer()
        {
            GameObject player = new GameObject("Player");
            player.transform.SetParent(roundRoot);
            player.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = new Color(0.15f, 0.75f, 1f);
            renderer.sortingOrder = 2;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.48f;

            playerHealth = player.AddComponent<Health>();
            playerHealth.Configure(100);
            playerHealth.Died += HandlePlayerDied;

            playerController = player.AddComponent<PlayerController>();
            playerController.Initialize(arenaCamera, roundRoot, squareSprite);
            return player.transform;
        }

        private void HandlePlayerDied()
        {
            isGameOver = true;
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            if (spawner != null)
            {
                spawner.enabled = false;
            }
        }

        private void RefreshHudText()
        {
            int health = playerHealth == null ? 0 : playerHealth.CurrentHealth;
            if (health == cachedHealth && score == cachedScore && enemyCount == cachedEnemyCount)
            {
                return;
            }

            cachedHealth = health;
            cachedScore = score;
            cachedEnemyCount = enemyCount;
            hudText = $"HP: {health}    Score: {score}    Enemies: {enemyCount}";
        }

        private void EnsureGuiStyles()
        {
            if (hudStyle != null)
            {
                return;
            }

            hudStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15
            };
            hudStyle.normal.textColor = Color.white;
            gameOverStyle = new GUIStyle(hudStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
