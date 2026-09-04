using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaSystemsLab
{
    public sealed class ArenaGame : MonoBehaviour
    {
        public const float HalfWidth = 8.5f;
        public const float HalfHeight = 5f;

        private const int MaxEnemies = 40;
        private const int LeaderboardLimit = 5;
        private const string LeaderboardPlayerId = "UnityPlayer";
        private const string ControlsText = "Move: WASD / Arrows / Left Stick    Attack: Left Click / Enter / West Button";
        private const string EnemyStatesText = "Enemy: Gray Idle / Red Chase / Orange Attack";

        private Camera arenaCamera;
        private Sprite squareSprite;
        private Transform roundRoot;
        private Health playerHealth;
        private PlayerController playerController;
        private EnemySpawner spawner;
        private GUIStyle hudStyle;
        private GUIStyle gameOverStyle;
        private LeaderboardClient leaderboardClient;
        private CancellationTokenSource leaderboardCancellation;
        private string hudText;
        private string gameOverText;
        private string leaderboardStatus;
        private string leaderboardText;
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
            leaderboardClient = new LeaderboardClient();
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
            CancelLeaderboardRequest();
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

            GUI.Box(new Rect(12f, 12f, 530f, 122f), GUIContent.none);
            GUI.Label(new Rect(24f, 18f, 500f, 28f), hudText, hudStyle);
            GUI.Label(new Rect(24f, 48f, 500f, 24f), ControlsText, hudStyle);
            GUI.Label(new Rect(24f, 72f, 500f, 24f), EnemyStatesText, hudStyle);
            GUI.Label(new Rect(24f, 96f, 500f, 24f), leaderboardStatus, hudStyle);

            if (isGameOver)
            {
                GUI.Box(new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f - 145f, 440f, 290f), GUIContent.none);
                GUI.Label(new Rect(Screen.width * 0.5f - 205f, Screen.height * 0.5f - 132f, 410f, 264f), gameOverText, gameOverStyle);
            }
        }

        public void RegisterEnemySpawned()
        {
            enemyCount++;
        }

        public void RegisterEnemyDeath()
        {
            enemyCount = Mathf.Max(0, enemyCount - 1);
            if (!isGameOver)
            {
                score++;
            }
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
            CancelLeaderboardRequest();
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
            leaderboardStatus = "Leaderboard: waiting for Game Over";
            leaderboardText = string.Empty;
            gameOverText = string.Empty;

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

            leaderboardStatus = "Leaderboard: submitting score...";
            RefreshGameOverText();
            leaderboardCancellation = new CancellationTokenSource();
            _ = SyncLeaderboardAsync(score, leaderboardCancellation.Token);
        }

        private async Task SyncLeaderboardAsync(int finalScore, CancellationToken cancellationToken)
        {
            try
            {
                LeaderboardEntry[] entries = await leaderboardClient.SubmitAndGetLeaderboardAsync(
                    LeaderboardPlayerId,
                    finalScore,
                    LeaderboardLimit,
                    cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                leaderboardStatus = "Leaderboard: connected";
                leaderboardText = FormatLeaderboard(entries);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (LeaderboardClientException)
            {
                leaderboardStatus = "Leaderboard: unavailable (start local server, then play again)";
                leaderboardText = string.Empty;
            }
            catch (Exception)
            {
                leaderboardStatus = "Leaderboard: unexpected client error";
                leaderboardText = string.Empty;
                Debug.LogError("Leaderboard synchronization failed with an unexpected client error.", this);
            }

            RefreshGameOverText();
        }

        private void CancelLeaderboardRequest()
        {
            if (leaderboardCancellation == null)
            {
                return;
            }

            leaderboardCancellation.Cancel();
            leaderboardCancellation.Dispose();
            leaderboardCancellation = null;
        }

        private void RefreshGameOverText()
        {
            gameOverText = $"GAME OVER\nFinal Score: {score}\nPress R to restart\n\n{leaderboardStatus}";
            if (!string.IsNullOrEmpty(leaderboardText))
            {
                gameOverText += $"\n\nTop {LeaderboardLimit}\n{leaderboardText}";
            }
        }

        private static string FormatLeaderboard(LeaderboardEntry[] entries)
        {
            if (entries.Length == 0)
            {
                return "No scores yet";
            }

            var builder = new StringBuilder(entries.Length * 24);
            for (int index = 0; index < entries.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append('\n');
                }

                builder.Append(index + 1)
                    .Append(". ")
                    .Append(entries[index].PlayerId)
                    .Append("  ")
                    .Append(entries[index].Score);
            }

            return builder.ToString();
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
