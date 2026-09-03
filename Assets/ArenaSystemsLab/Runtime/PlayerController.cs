using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaSystemsLab
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 5f;
        [SerializeField, Min(0.01f)] private float attackInterval = 0.2f;
        [SerializeField, Min(0.1f)] private float projectileSpeed = 11f;
        [SerializeField, Min(1)] private int projectileDamage = 10;

        private Rigidbody2D body;
        private Camera arenaCamera;
        private Transform projectileParent;
        private Sprite projectileSprite;
        private Vector2 moveInput;
        private Vector2 lastAim = Vector2.right;
        private float nextAttackTime;
        private bool initialized;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (!initialized)
            {
                Debug.LogError("PlayerController requires Initialize before the first frame.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            moveInput = ReadMovement();
            if (moveInput.sqrMagnitude > 0.01f)
            {
                lastAim = moveInput.normalized;
            }

            if (Time.time >= nextAttackTime && IsAttackPressed())
            {
                FireProjectile(ReadAimDirection());
                nextAttackTime = Time.time + attackInterval;
            }
        }

        private void FixedUpdate()
        {
            Vector2 target = body.position + moveInput * (moveSpeed * Time.fixedDeltaTime);
            target.x = Mathf.Clamp(target.x, -ArenaGame.HalfWidth, ArenaGame.HalfWidth);
            target.y = Mathf.Clamp(target.y, -ArenaGame.HalfHeight, ArenaGame.HalfHeight);
            body.MovePosition(target);
        }

        private void OnDisable()
        {
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
            }
        }

        public void Initialize(Camera targetCamera, Transform parent, Sprite sprite)
        {
            arenaCamera = targetCamera;
            projectileParent = parent;
            projectileSprite = sprite;
            initialized = arenaCamera != null && projectileParent != null && projectileSprite != null;

            if (!initialized)
            {
                Debug.LogError("PlayerController is missing the camera, projectile parent, or projectile sprite.", this);
            }
        }

        private static Vector2 ReadMovement()
        {
            Vector2 movement = Vector2.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    movement.x -= 1f;
                }
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    movement.x += 1f;
                }
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    movement.y -= 1f;
                }
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    movement.y += 1f;
                }
            }

            Gamepad gamepad = Gamepad.current;
            if (movement.sqrMagnitude < 0.01f && gamepad != null)
            {
                movement = gamepad.leftStick.ReadValue();
            }

            return Vector2.ClampMagnitude(movement, 1f);
        }

        private static bool IsAttackPressed()
        {
            return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                || (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
                || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);
        }

        private Vector2 ReadAimDirection()
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                Vector2 stick = gamepad.rightStick.ReadValue();
                if (stick.sqrMagnitude > 0.04f)
                {
                    lastAim = stick.normalized;
                    return lastAim;
                }
            }

            if (Mouse.current != null)
            {
                Vector2 pointer = Mouse.current.position.ReadValue();
                Vector3 world = arenaCamera.ScreenToWorldPoint(new Vector3(pointer.x, pointer.y, -arenaCamera.transform.position.z));
                Vector2 mouseAim = (Vector2)world - body.position;
                if (mouseAim.sqrMagnitude > 0.01f)
                {
                    lastAim = mouseAim.normalized;
                }
            }

            return lastAim;
        }

        private void FireProjectile(Vector2 direction)
        {
            GameObject projectile = new GameObject("Player Projectile");
            projectile.transform.SetParent(projectileParent);
            projectile.transform.position = body.position + direction * 0.65f;
            projectile.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

            SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
            renderer.sprite = projectileSprite;
            renderer.color = new Color(1f, 0.9f, 0.2f);
            renderer.sortingOrder = 3;

            CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D projectileBody = projectile.AddComponent<Rigidbody2D>();
            projectileBody.gravityScale = 0f;
            projectileBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            Projectile projectileController = projectile.AddComponent<Projectile>();
            projectileController.Initialize(direction, projectileSpeed, projectileDamage);
        }
    }
}
