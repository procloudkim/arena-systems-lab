using NUnit.Framework;

namespace ArenaSystemsLab.Tests.EditMode
{
    public sealed class EnemyStateMachineTests
    {
        [Test]
        public void NewMachine_StartsIdle()
        {
            EnemyStateMachine machine = new EnemyStateMachine();

            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Idle));
        }

        [Test]
        public void Evaluate_WhenEnemyCanAct_TransitionsBetweenChaseAndAttack()
        {
            EnemyStateMachine machine = new EnemyStateMachine();

            Assert.That(machine.Evaluate(false, true, false), Is.True);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Chase));
            Assert.That(machine.Evaluate(false, true, true), Is.True);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Attack));
            Assert.That(machine.Evaluate(false, true, false), Is.True);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Chase));
        }

        [Test]
        public void Evaluate_WhenEnemyCannotAct_TransitionsToIdle()
        {
            EnemyStateMachine machine = new EnemyStateMachine();
            machine.Evaluate(false, true, false);

            Assert.That(machine.Evaluate(false, false, true), Is.True);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Idle));
        }

        [Test]
        public void Evaluate_AfterDeath_RemainsDead()
        {
            EnemyStateMachine machine = new EnemyStateMachine();
            machine.Evaluate(false, true, false);

            Assert.That(machine.Evaluate(true, true, true), Is.True);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Dead));
            Assert.That(machine.Evaluate(false, true, false), Is.False);
            Assert.That(machine.CurrentState, Is.EqualTo(EnemyState.Dead));
        }
    }
}
