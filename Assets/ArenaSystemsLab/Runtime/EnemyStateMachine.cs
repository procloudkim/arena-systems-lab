namespace ArenaSystemsLab
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    public sealed class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

        public bool Evaluate(bool isDead, bool canAct, bool hasTargetContact)
        {
            if (CurrentState == EnemyState.Dead)
            {
                return false;
            }

            EnemyState nextState = Resolve(isDead, canAct, hasTargetContact);
            if (nextState == CurrentState)
            {
                return false;
            }

            CurrentState = nextState;
            return true;
        }

        private static EnemyState Resolve(bool isDead, bool canAct, bool hasTargetContact)
        {
            if (isDead)
            {
                return EnemyState.Dead;
            }

            if (!canAct)
            {
                return EnemyState.Idle;
            }

            return hasTargetContact ? EnemyState.Attack : EnemyState.Chase;
        }
    }
}
