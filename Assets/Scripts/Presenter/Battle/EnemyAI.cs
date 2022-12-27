using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TestUnityCardGame.Domain.Service;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Card;

namespace TestUnityCardGame.Presenter.Battle
{
    public class EnemyAI : MonoBehaviour
    { 
        public IEnumerator EnemyTurn()
        {
            // わかりやすくするため
            HeroController enemyAIHero = BattleViewController.Instance.player2Hero;
            Transform enemyAIHandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
            Transform enemyAIFieldTransform = BattleViewController.Instance.GetPlayer2FieldTransform();

            // 情報パネルが過ぎるまで待つ
            yield return new WaitForSeconds(4.0f);

            enemyAIHero.view.SetActiveActivatedPanel(true);

            // 手札のカードリストを取得
            CardController[] enemyAIHandCards = BattleViewController.Instance.GetMyHandCards(Player.Player2);

            // 敵フィールドの現時点でのカードを取得
            CardController[] player1FieldCards= BattleViewController.Instance.GetOpponentFieldCards(Player.Player2);

            // コスト以下のカードがあれば、カードをフィールドに出し続ける
            while (Array.Exists(enemyAIHandCards, card => (card.model.GetManaCost() <= enemyAIHero.model.GetManaCost().Value && (!card.model.IsSpell() || (card.model.IsSpell() && CanCastSpell(card)))))) {
                // コスト以下のカードリストを取得
                CardController[] selectableEnemyHandCards = Array.FindAll(enemyAIHandCards, card => (card.model.GetManaCost() <= enemyAIHero.model.GetManaCost().Value) && (!card.model.IsSpell() || (card.model.IsSpell() && CanCastSpell(card))));

                // 場に出すカードを選択
                CardController card = selectableEnemyHandCards[0];

                // カードを表に向ける
                card.view.SetActiveFrontPanel(true);

                // スペルカードなら使用する
                if (card.model.IsSpell()) {
                    var castSpell = CastSpellOf(card);
                    while(castSpell.MoveNext()){}
                } else {
                    // カードを移動して表示状態に変更、マナコストを減らす
                    StartCoroutine(card.movement.MoveToField(enemyAIFieldTransform));
                    enemyAIHero.ReduceManaCost(card.model.GetManaCost());

                    // 敵のカードがフィールドに出たことを明示する
                    card.OnField();
                }
                yield return new WaitForSeconds(1.0f);
                enemyAIHandCards = BattleViewController.Instance.GetMyHandCards(Player.Player2);
            }
            yield return new WaitForSeconds(1.0f);
            // 攻撃① フィールドのカードリストを取得
            CardController[] enemyAIFieldCards = BattleViewController.Instance.GetFriendFieldCards(Player.Player2);

            // 攻撃② 攻撃可能カードがあれば攻撃を繰り返す
            while (Array.Exists(enemyAIFieldCards, card => card.IsAttackable())) {
                // 攻撃③攻撃可能カードを取得検索：　Array.FindAll
                CardController[] enemyCanAttackCards = Array.FindAll(enemyAIFieldCards, card => card.IsAttackable());

                // 攻撃④ pick player's defender cards.
                player1FieldCards = BattleViewController.Instance.GetOpponentFieldCards(Player.Player2);

                if (enemyCanAttackCards.Length > 0) {
                    CardController attacker = enemyCanAttackCards[0];

                    // 攻撃⑤ 場にカードがあればカードを攻撃、なければHeroを攻撃
                    if (player1FieldCards.Length > 0) {
                        // シールドカードのみ攻撃対象にする
                        if (Array.Exists(player1FieldCards, card => card.model.GetAbility() == Ability.Shield)) {
                            player1FieldCards = Array.FindAll(player1FieldCards, card => card.model.GetAbility() == Ability.Shield);
                        }
                        CardController defender = player1FieldCards[0];

                        // 攻撃⑤ start combat
                        StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                        yield return new WaitForSeconds(0.51f);
                        StartCoroutine(BattleViewController.Instance.CardsBattle(attacker, defender));
                    } else {
                        StartCoroutine(attacker.movement.MoveToTarget(BattleViewController.Instance.player1Hero.transform));
                        yield return new WaitForSeconds(0.25f);

                        // 攻撃⑤ start combat
                        BattleViewController.Instance.player1Hero.Attacked(attacker);
                    }
                    enemyAIFieldCards = BattleViewController.Instance.GetOpponentFieldCards(Player.Player2);
                    yield return new WaitForSeconds(1);
                }
            }

            yield return new WaitForSeconds(1.5f);

            enemyAIHero.AddTurnNumber();

            BattleViewController.Instance.turnController.ChangeTurn();
        }


        bool CanCastSpell(CardController card)
        {
            // わかりやすくするため
            HeroController player1Hero = BattleViewController.Instance.player1Hero;
            HeroController enemyAIHero = BattleViewController.Instance.player2Hero;
            Transform enemyAIHandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
            Transform enemyAIFieldTransform = BattleViewController.Instance.GetPlayer2FieldTransform();

            switch (card.model.GetSpell()) {
                case Spell.AttackEnemyCard:
                case Spell.AttackEnemyCards:
                    CardController[] opponentCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                    if (opponentCards.Length > 0)
                    {
                        return (card.model.GetManaCost() <= enemyAIHero.model.GetManaCost().Value);
                    }
                    return false;
                case Spell.HealFriendCard:
                case Spell.HealFriendCards:
                    CardController[] friendCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                    if (friendCards.Length > 0)
                    {
                        return (card.model.GetManaCost() <= enemyAIHero.model.GetManaCost().Value);
                    }
                    return false;
                case Spell.AttackEnemyHero:
                case Spell.HealFriendHero:
                    return (card.model.GetManaCost() <= enemyAIHero.model.GetManaCost().Value);
            }

            return false;
        }

        IEnumerator CastSpellOf(CardController card)
        {
            // わかりやすくするため
            HeroController player1Hero = BattleViewController.Instance.player1Hero;
            HeroController enemyAIHero = BattleViewController.Instance.player2Hero;
            Transform enemyAIHandTransform = BattleViewController.Instance.GetPlayer2HandTransform();
            Transform enemyAIFieldTransform = BattleViewController.Instance.GetPlayer2FieldTransform();

            CardController target = null;
            Transform movePosition = null;

            switch (card.model.GetSpell()) {
                case Spell.AttackEnemyCard:
                    CardController[] opponentCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                    if (opponentCards.Length > 0) {
                        target = opponentCards[0];
                        movePosition = target.transform;
                    }

                    StartCoroutine(card.UseSpellToCard(target, enemyAIHero, movePosition));
                    break;
                case Spell.HealFriendCard:
                    CardController[] friendCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                    if (friendCards.Length > 0) {
                        target = friendCards[0];
                        movePosition = target.transform;
                    }
            
                    StartCoroutine(card.UseSpellToCard(target, enemyAIHero, movePosition));
                    break;
                case Spell.AttackEnemyCards:
                    opponentCards = BattleViewController.Instance.GetOpponentFieldCards(card.GetOwner());
                    movePosition = BattleViewController.Instance.GetPlayer1FieldTransform();

                    StartCoroutine(card.UseSpellToCards(opponentCards, enemyAIHero, movePosition));
                    break;
                case Spell.HealFriendCards:
                    friendCards = BattleViewController.Instance.GetFriendFieldCards(card.GetOwner());
                    movePosition = enemyAIFieldTransform;
 
                    StartCoroutine(card.UseSpellToCards(friendCards, enemyAIHero, movePosition));
                    break;
                case Spell.AttackEnemyHero:
                    movePosition = player1Hero.transform;

                    StartCoroutine(card.UseSpellToHero(player1Hero, enemyAIHero, movePosition));
                    break;
                case Spell.HealFriendHero:
                    movePosition = enemyAIHero.transform;

                    StartCoroutine(card.UseSpellToHero(enemyAIHero, enemyAIHero, movePosition));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}