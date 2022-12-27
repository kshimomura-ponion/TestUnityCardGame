using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers; 
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Sound;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;
using TestUnityCardGame.View.Battle;

namespace TestUnityCardGame.Presenter.Battle
{
    public class BattleViewController: SingletonMonoBehaviour<BattleViewController>
    {

        [SerializeField] AudioManager audioManager; // Audio Manager
        [SerializeField] EntitiesManager entitiesManager; // Entities Manager

        // ターンコントローラー
        public TurnController turnController;

        // View
        BattleView battleView;

        // 手札を置ける場所情報
        [SerializeField] Transform player1HandTransform, player2HandTransform;
        [SerializeField] Transform player1FieldTransform, player2FieldTransform;

        // Heroを置ける場所情報
        [SerializeField] Transform player1HeroPanel, player2HeroPanel;

        // Hero
        [System.NonSerialized] public HeroController player1Hero, player2Hero;

        // 前のページから受け渡されるデータ
        public BattleData battleData;

        // 手札の保持数の最大値
        int maxHandCardNum = 5;

        // プレイヤー1のターンかどうか識別する
        [System.NonSerialized] public bool isPlayer1Turn;

        protected override void Awake()
        {
            // Viewの準備
            battleView = GetComponent<BattleView>();

            // バトル初期データを受け取る
            battleData = MiniUniduxService.State.Scene.GetData<BattleData>();
        }

        void Start()
        {
            // 音楽を再生
            audioManager.PlayBGM(BGM.Battle);

            StartBattle();
        }

        public void StartBattle()
        {
            // Heroの準備
            SettingHeroes(battleData.hero1ID, battleData.hero2ID);

            // 手札の準備
            SettingInitHand(3);

            // ターン開始
            isPlayer1Turn = true;
            turnController.TurnStart();
        }

    void SettingHeroes(int id1, int id2)
        {
            player1Hero = Instantiate(battleView.GetHeroPrefab(), player1HeroPanel, false);
            player2Hero = Instantiate(battleView.GetHeroPrefab(), player2HeroPanel, false);

            // デッキ 1~8のカードIDから16枚をランダムに生成する
            List<int> player1Deck = new List<int>();
            List<int> player2Deck = new List<int>();
            for(int i = 1; i <= (battleData.existCardNum * 2); i++) {
                int idx1 = UnityEngine.Random.Range(1, battleData.existCardNum);
                player1Deck.Add(idx1);
                int idx2 = UnityEngine.Random.Range(1, battleData.existCardNum);
                player2Deck.Add(idx2);
            }
            // UnityEngine.Debug.Log(string.Join(",", player1Deck.Select(n => n.ToString())));
            // UnityEngine.Debug.Log(string.Join(",", player2Deck.Select(n => n.ToString())));

            // ここで各Heroの持つカード情報を整理しておく
            player1Hero.Init(entitiesManager.GetHeroEntity(id1), player1Deck, Player.Player1);
            player2Hero.Init(entitiesManager.GetHeroEntity(id2), player2Deck, Player.Player2);

            // 以後HeroのHPを監視する
            player1Hero.model.GetHP()
                .Where(x => x <= 0)
                .First()
                .Delay(TimeSpan.FromSeconds(1.0f))
                .Subscribe(_ => GameOver()).AddTo(this);

            player2Hero.model.GetHP()
                .Where(x => x <= 0)
                .First()
                .Delay(TimeSpan.FromSeconds(1.0f))
                .Subscribe(_ => GameOver())
                .AddTo(this);

            // 以後HeroのMana Costを監視する
            player1Hero.model.GetManaCost()
                .Subscribe(_ => UpdateCardSettingsWithManaCost(Player.Player1))
                .AddTo(this);

            player2Hero.model.GetManaCost()
                .Subscribe(_ => UpdateCardSettingsWithManaCost(Player.Player2))
                .AddTo(this);
        }

        public void SettingInitHand(int initHandNum)
        {
            for (int i = 0; i < initHandNum; i++) {
                // 手札置き場（Hand）、FieldともにHorizontalLayoutGroupに属していないとカードが重なってしまう
                // （カードはAlignmentでCenterに置くべき）
                GiveCardToHand(player1Hero, player1HandTransform, Player.Player1);
                GiveCardToHand(player2Hero, player2HandTransform, Player.Player2);
            }
        }

        // 手札が保有量を超えなければデッキからカードを取得
        public void GiveCardToHand(HeroController hero, Transform hand, Player player)
        {
            CardController[] currentHandCards = GetMyHandCards(player);
            if (currentHandCards.Length <= maxHandCardNum) {
                List<(int, CardType)> deck = hero.model.GetCardDeck();
                if (deck.Count == 0) {
                    return;
                }
                // デッキの上から値を削除していく
                (int, CardType) cardInfo = deck[0];
                hero.model.RemoveCard(0);

                CreateCard(cardInfo, hand, player);
            }
        }

        // カードをインスタンス化
        void CreateCard((int, CardType) cardInfo, Transform hand, Player player)
        {
            CardController card = Instantiate(battleView.GetCardPrefab(), hand, false);

            // カードエンティティを生成し、カードタイプを渡してカードを生成する
            card.Init(entitiesManager.GetCardEntity(cardInfo.Item1, cardInfo.Item2), player);
        }

        public IEnumerator CardsBattle(CardController attacker, CardController defender)
        {
            // 攻撃側の攻撃ダメージを計算し、防御側Viewのダメージ情報パネルを更新する
            int defenderDamage = attacker.Attack(defender);
            defender.CheckAlive(-1 * defenderDamage);

            yield return new WaitForSeconds(0.5f);

            // 防御側の反撃ダメージを計算し、攻撃側Viewのダメージ情報パネルを更新する
            int attackerDamage = defender.Attack(attacker);
            attacker.CheckAlive(-1 * attackerDamage);
        }

        // いらないオブジェクトの破棄
        public void CleanUp()
        {
            // handとFiledのカードを削除
            foreach (Transform card in player1HandTransform) {
                Destroy(card.gameObject);
            }
            foreach (Transform card in player1FieldTransform) {
                Destroy(card.gameObject);
            }
            foreach (Transform card in player2HandTransform) {
                Destroy(card.gameObject);
            }
            foreach (Transform card in player2FieldTransform) {
                Destroy(card.gameObject);
            }
        }

        public void GameOver()
        {
            //音楽を停止
            audioManager.StopBGM();

            StopAllCoroutines();

            CleanUp();

            bool isPlayer1Win = (player1Hero.model.GetHP().Value > 0);

            var resultData = new ResultData(isPlayer1Win, battleData.hero1ID, battleData.hero2ID, battleData.isPlayer2AI, battleData.existHeroNum, battleData.existCardNum);

            // リザルト画面へ遷移するプッシュアクションを生成
            var pushToResultAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Result, resultData);

            // プッシュアクションのディスパッチ
            MiniUniduxService.Dispatch(pushToResultAction);
        }

        // 自分の手札を取得
        public CardController[] GetMyHandCards(Player player)
        {
            if (player == Player.Player1) {
                return player1HandTransform.GetComponentsInChildren<CardController>();
            } else {
                return player2HandTransform.GetComponentsInChildren<CardController>();
            }
        }

        // 現在ターンが回っているHeroにとって味方のフィールドを取得
        public CardController[] GetFriendFieldCards(Player player)
        {
            if (player == Player.Player1) {
                return player1FieldTransform.GetComponentsInChildren<CardController>();
            } else {
                return player2FieldTransform.GetComponentsInChildren<CardController>();
            }
        }

        // 現在ターンが回っているHeroにとって敵のフィールドを取得
        public CardController[] GetOpponentFieldCards(Player player)
        {
            if (player == Player.Player1) {
                return player2FieldTransform.GetComponentsInChildren<CardController>();
            } else {
                return player1FieldTransform.GetComponentsInChildren<CardController>();
            }
        }

        public bool ExistsSheldCard(Player player)
        {
            //　シールドカードがあればシールドカード以外は攻撃できない
            CardController[] enemyFieldCards = GetOpponentFieldCards(player);

            if (Array.Exists(enemyFieldCards, card => card.model.GetAbility() == Ability.Shield)) {
                return true;
            } else {
                return false;
            }
        }
        
        public void UpdateCardSettingsWithManaCost(Player player)
        {
            SetAllCardsInHandCanAttackWithManaCost(player);
            SetAllCardsIsDraggableWithManaCost(player);
        }

        public void SetAllCardsIsDraggableWithManaCost(Player player)
        {
            CardController[] handCardList = BattleViewController.Instance.GetMyHandCards(player);
            if (player == Player.Player1) {
                SetCardsIsDraggableWithManaCost(handCardList, player1Hero);
            } else {
                SetCardsIsDraggableWithManaCost(handCardList, player2Hero);
            }
        }

        // カードのコストとPlayerのMana Costを比較してドラッグ可能かどうか判定する
        public void SetCardsIsDraggableWithManaCost(CardController[] cardList, HeroController hero)
        {
            foreach (CardController card in cardList) {
                if (card.model.GetManaCost() <= hero.model.GetManaCost().Value && hero.model.GetManaCost().Value > 0) {
                    card.SetDraggable(true);
                } else {
                    card.SetDraggable(false);
                }
            }
        }

        public void SetAllCardsInHandCanAttackWithManaCost(Player player)
        {
            CardController[] handCardList = BattleViewController.Instance.GetMyHandCards(player);

            // 攻撃表示の変更
            if (player == Player.Player1) {
                SetCardsInHandCanAttackWithManaCost(handCardList, true, BattleViewController.Instance.player1Hero);
                SetCardsInHandCanAttackWithManaCost(handCardList, false, BattleViewController.Instance.player2Hero);
            } else {
                SetCardsInHandCanAttackWithManaCost(handCardList, false, BattleViewController.Instance.player1Hero);
                SetCardsInHandCanAttackWithManaCost(handCardList, true, BattleViewController.Instance.player2Hero);
            }
        }

        public void SetCardsInHandCanAttackWithManaCost(CardController[] cardList, bool isAttackable, HeroController hero)
        {
            foreach (CardController card in cardList) {
                if (isAttackable) {
                    // フィールドに出ているカード（モンスターカード）はターン開始時に決定・手持ちで攻撃表示にできるのはスペルカードのみ
                    if (card.model.IsSpell() && !card.IsFieldCard()) {
                        if (card.model.GetManaCost() <= hero.model.GetManaCost().Value && hero.model.GetManaCost().Value > 0) {
                            CardController[] targetCards;
                            switch (card.model.GetSpell()) {
                                case Spell.AttackEnemyCard:
                                case Spell.AttackEnemyCards:
                                    targetCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                                    if (targetCards.Length > 0) {
                                        card.SetAttackable(isAttackable);
                                    }
                                    break;
                                case Spell.HealFriendCard:
                                case Spell.HealFriendCards:
                                    targetCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                                    if (targetCards.Length > 0) {
                                        card.SetAttackable(isAttackable);
                                    }
                                    break;
                                case Spell.AttackEnemyHero:
                                case Spell.HealFriendHero:
                                    card.SetAttackable(isAttackable);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void SetTurnNumText(string turnNumString)
        {
            battleView.SetTurnNumText(turnNumString);
        }

        public void TurnendButtonActivate(bool activeState)
        {
            battleView.TurnendButtonActivate(activeState);
        }

        public Transform GetPlayer1HandTransform()
        {
            return player1HandTransform;
        }

        public Transform GetPlayer2HandTransform()
        {
            return player2HandTransform;
        }

        public Transform GetPlayer1FieldTransform()
        {
            return player1FieldTransform;
        }
        public Transform GetPlayer2FieldTransform()
        {
            return player2FieldTransform;
        }
        public TurnInfoView GetTurnInfoViewPrefab()
        {
            return battleView.GetTurnInfoViewPrefab();
        }
    }
}