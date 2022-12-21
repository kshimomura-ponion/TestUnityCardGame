namespace MiniUnidux
{
    public interface IReducer 
    {
        bool IsMatchedAction(object action);
        object ReduceAny(object state, object action);
    }
}
