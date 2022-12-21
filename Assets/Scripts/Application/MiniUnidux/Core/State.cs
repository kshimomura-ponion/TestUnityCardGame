// original code is from Unidux
using System;
using System.Collections;
using System.Collections.Generic;
using MiniUnidux.Util;

namespace MiniUnidux
{
    [Serializable]
    public class State: IState, IStateChangeFlg, ICloneable
    {
        public bool IsStateChanged { get; private set; }
        
        public void SetStateChanged(bool changed = true)
        {
            this.IsStateChanged = changed;
        }
        
        public virtual object Clone()
        {
            // It's slow. So in case of requiring performance, override this deep clone method by your code.
            return CloneUtil.MemoryClone(this);
        }

        public override bool Equals(object obj)
        {
            // It's slow. So in case of requiring performance, override this equality method by your code.
            return EqualityUtil.EntityEquals(this, obj);
        }

        public override int GetHashCode()
        {
            // Default implmeentation for supress warnings
            return base.GetHashCode();
        }
     }
}
