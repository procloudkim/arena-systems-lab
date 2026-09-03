using NUnit.Framework;
using UnityEngine;

namespace ArenaSystemsLab.Tests.EditMode
{
    public sealed class HealthTests
    {
        private GameObject gameObject;
        private Health health;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("Health Test Subject");
            health = gameObject.AddComponent<Health>();
            health.Configure(100);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ApplyDamage_ReducesCurrentHealth()
        {
            Assert.That(health.ApplyDamage(25), Is.True);
            Assert.That(health.CurrentHealth, Is.EqualTo(75));
        }

        [Test]
        public void ApplyDamage_AtZero_MarksHealthDead()
        {
            health.ApplyDamage(100);

            Assert.That(health.CurrentHealth, Is.Zero);
            Assert.That(health.IsDead, Is.True);
        }

        [Test]
        public void ApplyDamage_AfterDeath_DoesNotRaiseDeathTwice()
        {
            int deathCount = 0;
            health.Died += () => deathCount++;

            health.ApplyDamage(100);
            health.ApplyDamage(1);

            Assert.That(deathCount, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ApplyDamage_WithInvalidAmount_IsIgnored(int amount)
        {
            Assert.That(health.ApplyDamage(amount), Is.False);
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
            Assert.That(health.IsDead, Is.False);
        }
    }
}
