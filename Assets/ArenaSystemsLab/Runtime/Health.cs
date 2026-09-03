using System;
using UnityEngine;

namespace ArenaSystemsLab
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;

        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Configure(int maximumHealth)
        {
            if (maximumHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumHealth));
            }

            maxHealth = maximumHealth;
            CurrentHealth = maxHealth;
            IsDead = false;
        }

        public bool ApplyDamage(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return false;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (CurrentHealth == 0)
            {
                IsDead = true;
                Died?.Invoke();
            }

            return true;
        }
    }
}
