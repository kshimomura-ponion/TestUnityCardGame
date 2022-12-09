using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{ 
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public IEnumerator EnemyTurn()
    {
        // わかりやすくするため
        HeroController enemyAIHero = gameManager.player2Hero;
        Transform enemyAIHandTransform = gameManager.player2HandTransform;
        Transform enemyAIFieldTransform = gameManager.player2FieldTransform;

        // 情報パネルが過ぎるまで待つ
        yield return new WaitForSeconds(4.0f);

        enemyAIHero.view.SetActiveActivatedPanel(true);

        Debug.Log("Enemyのターン");

        // 手札のカードリストを取得
        CardController[] enemyAIHandCardList = enemyAIHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードがあれば、カードをフィールドに出し続ける
        while (Array.Exists(enemyAIHandCardList, card => card.model.GetManaCost() <= enemyAIHero.model.GetManaCost())) {
            // コスト以下のカードリストを取得
            CardController[] selectableEnemyHandCardList = Array.FindAll(enemyAIHandCardList, card => card.model.GetManaCost() <= enemyAIHero.model.GetManaCost());

            // 場に出すカードを選択
            CardController card = selectableEnemyHandCardList[0];
            
            // スペルカードなら使用する
            if (card.model.IsSpell()){
                StartCoroutine(CastSpellOf(card));
            } else {
                // カードを移動して表示状態に変更、マナコストを減らす
                StartCoroutine(card.movement.MoveToField(enemyAIFieldTransform));

                // 敵のカードがフィールドに出たことを明示する
                card.model.OnField();

                // 敵のカードを表に向ける
                card.view.SetActiveFrontPanel(true);

                enemyAIHero.ReduceManaCost(card.model.GetManaCost());
                card.model.SetIsFieldCard(true);

                enemyAIHandCardList = gameManager.GetMyHandCards(PLAYER.PLAYER2);
            }
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);

        // 攻撃① フィールドのカードリストを取得
        CardController[] enemyAIFieldCardList = gameManager.GetFriendFieldCards(PLAYER.PLAYER2);

        // 攻撃② 攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(enemyAIFieldCardList, card => card.model.CanAttack()))
        {
            // 攻撃③攻撃可能カードを取得検索：　Array.FindAll
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyAIFieldCardList, card => card.model.CanAttack());

            // 攻撃④ pick player's defender cards.
            CardController[] player1FieldCardList = gameManager.GetOpponentFieldCards(PLAYER.PLAYER2);

            if (enemyCanAttackCardList.Length > 0){
                CardController attacker = enemyCanAttackCardList[0];

                // 攻撃⑤ 場にカードがあればカードを攻撃、なければHeroを攻撃
                if (player1FieldCardList.Length > 0){
                    // シールドカードのみ攻撃対象にする
                    if(Array.Exists(player1FieldCardList, card => card.model.GetAbility() == ABILITY.SHIELD)){
                        player1FieldCardList = Array.FindAll(player1FieldCardList, card => card.model.GetAbility() == ABILITY.SHIELD);
                    }
                    CardController defender = player1FieldCardList[0];

                    // 攻撃⑤ start combat
                    StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                    yield return new WaitForSeconds(0.51f);
                    gameManager.turnController.CardsBattle(attacker, defender);

                } else {
                    StartCoroutine(attacker.movement.MoveToTarget(gameManager.player1Hero.transform));
                    yield return new WaitForSeconds(0.25f);

                    // 攻撃⑤ start combat
                    gameManager.player1Hero.Attacked(attacker);
                }
                enemyAIFieldCardList = gameManager.GetOpponentFieldCards(PLAYER.PLAYER2);
                yield return new WaitForSeconds(1);
            }
        }
        yield return new WaitForSeconds(1.5f);
        enemyAIHero.AddTurnNumber();
        gameManager.turnController.ChangeTurn();
    }

    IEnumerator CastSpellOf(CardController card)
    {
        // わかりやすくするため
        HeroController enemyAIHero = gameManager.player2Hero;
        Transform enemyAIHandTransform = gameManager.player2HandTransform;
        Transform enemyAIFieldTransform = gameManager.player2FieldTransform;

        CardController target = null;
        Transform movePosition = null;

        switch (card.model.GetSpell()){
            case SPELL.DAMAGE_ENEMY_CARD:
                CardController[] opponentCardList = gameManager.GetOpponentFieldCards(card.GetOwner());
                if(opponentCardList.Length > 0) {
                    target = opponentCardList[0];
                    movePosition = target.transform;
                }
                break;
            case SPELL.HEAL_FRIEND_CARD:
                CardController[] friendCardList =gameManager.GetFriendFieldCards(card.GetOwner());
                if(friendCardList.Length > 0) {
                    target = friendCardList[0];
                    movePosition = target.transform;
                }
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.player1FieldTransform;
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = enemyAIFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.player1Hero.transform;
                break;
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = enemyAIHero.transform;
                break;

        }

        yield return new WaitForSeconds(0.25f);
        
        // 移動先としてターゲット/それぞれのフィールド/それぞれのHeroのTransformが必要
        StartCoroutine(card.movement.MoveToField(movePosition));

       

        // カードを使用したらMana Costを減らして破壊する
        card.UseSpellTo(target, enemyAIHero);
    }
}
