using System;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame.Domain.Service
{
    [Serializable]
    public class ResultData : ISceneData
    {
        public bool isPlayer1Win { get; set; }

        // バトルをやり直す時のために受け渡すデータ
        public int hero1ID { get; set; }
        public int hero2ID { get; set; }
        public bool isPlayer2AI { get; set; }
        
        // 現在用意できているヒーロー、カードの種類の数
        public int existHeroNum { get; set; }
        public int existCardNum { get; set; }
        
        ResultData() {}

        public ResultData(bool isPlayer1Win, int hero1ID, int hero2ID, bool isPlayer2AI, int existHeroNum, int existCardNum)
        {
            this.isPlayer1Win = isPlayer1Win;

            // バトルをやり直す時のために受け渡されるデータ
            this.hero1ID = hero1ID;
            this.hero2ID = hero2ID;
            this.isPlayer2AI = isPlayer2AI;
            this.existHeroNum = existHeroNum;
            this.existCardNum = existCardNum;
        }
    }
}
