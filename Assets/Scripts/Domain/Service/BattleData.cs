using System.Globalization;
using System;
using MiniUnidux.SceneTransition;
using TestUnityCardGame.Presenter.Hero;

namespace TestUnityCardGame.Domain.Service
{
    [Serializable]
    public class BattleData : ISceneData
    {
        // initial Value
        public int hero1ID { get; set; }
        public int hero2ID { get; set; }
        public bool isPlayer2AI { get; set; }
        
        // 現在用意できているヒーロー、カードの種類の数
        public int existHeroNum { get; set; }
        public int existCardNum { get; set; }
        
        BattleData() {}

        public BattleData(int hero1ID, int hero2ID, bool isPlayer2AI, int existHeroNum, int existCardNum)
        {
            this.hero1ID = hero1ID;
            this.hero2ID = hero2ID;
            this.isPlayer2AI = isPlayer2AI;
            this.existHeroNum = existHeroNum;
            this.existCardNum = existCardNum;
        }
    }
}
