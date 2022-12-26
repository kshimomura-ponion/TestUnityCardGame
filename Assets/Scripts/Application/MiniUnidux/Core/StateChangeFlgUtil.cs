// Original Code is Unidux
namespace MiniUnidux
{
    public static class StateUtil
    {
    public static bool ApplyStateChanged(IStateChangeFlg oldState, IStateChangeFlg newState)
    {
            if (oldState == null || newState == null) {
                return oldState != null || newState != null;
            }

            bool stateChanged = false;

            var properties = newState.GetType().GetProperties();
            foreach (var property in properties) {
                var newValue = property.GetValue(newState, null);
                var oldValue = property.GetValue(oldState, null);

                if (newValue is IStateChangeFlg) {
                    stateChanged |= ApplyStateChanged((IStateChangeFlg) oldValue, (IStateChangeFlg) newValue);
                } else if (newValue == null && oldValue != null || newValue != null && !newValue.Equals(oldValue)) {
                    stateChanged = true;
                }
            }

            var fields = newState.GetType().GetFields();
            foreach (var field in fields) {
                var newValue = field.GetValue(newState);
                var oldValue = field.GetValue(oldState);

                if (newValue is IStateChangeFlg) {
                    stateChanged |= ApplyStateChanged((IStateChangeFlg) oldValue, (IStateChangeFlg) newValue);
                } else if (newValue == null && oldValue != null || newValue != null && !newValue.Equals(oldValue)) {
                    stateChanged = true;
                }
            }

            if (stateChanged) {
                newState.SetStateChanged();
            }

            return stateChanged;
        }

        public static void ResetStateChanged(IStateChangeFlg state)
        {
            if (state != null) {
                state.SetStateChanged(false);
            }

            var properties = state.GetType().GetProperties();
            foreach (var property in properties) {
                var value = property.GetValue(state, null);

                if (value != null && value is IStateChangeFlg) {
                    var changedValue = (IStateChangeFlg) value;
                    changedValue.SetStateChanged(false);
                }
            }

            var fields = state.GetType().GetFields();
            foreach (var field in fields) {
                var value = field.GetValue(state);
                if (value != null && value is IStateChangeFlg) {
                    var changedValue = (IStateChangeFlg) value;
                    changedValue.SetStateChanged(false);
                }
            }
        }
    }
}
