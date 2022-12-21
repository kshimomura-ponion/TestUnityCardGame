namespace MiniUnidux
{
    public abstract class Reducer<TState, TAction> : IReducer
    {
        public abstract TState Reduce(TState state, TAction action);
        public virtual object ReduceAny(object state, object action)
        {
            return this.Reduce((TState) state, (TAction) action);
        }

        public virtual bool IsMatchedAction(object action)
        {
            return action is TAction;
        }
    }
}
