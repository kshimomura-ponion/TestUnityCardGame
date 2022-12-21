using System;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame
{
    [Serializable]
    public class BattleInitialData : ISceneData
    {
        public int hero1ID { get; set; }
        public int hero2ID { get; set; }
        public bool isPlayer2AI { get; set; }
        
        BattleInitialData() {}

        public BattleInitialData(int hero1ID, int hero2ID, bool isPlayer2AI)
        {
            this.hero1ID = hero1ID;
            this.hero2ID = hero2ID;
            this.isPlayer2AI = isPlayer2AI;
        }
    }
}
