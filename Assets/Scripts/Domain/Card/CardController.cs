using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TestUnityCardGame.Domain.Hero;
using TestUnityCardGame.Presenter.Battle;
using TestUnityCardGame.View.Card;

namespace TestUnityCardGame.Domain.Card
{
    public class CardController : MonoBehaviour
    {
        [SerializeField] AudioManager audioManager;
        [System.NonSerialized] public CardView view;
        [System.NonSerialized] public CardModel model;
        [System.NonSerialized] public CardMovement movement;

        private Player owner;
        private bool isDraggable;

        private void Awake()
        {
            view = GetComponent<CardView>();
            movement = GetComponent<CardMovement>();
            isDraggable = false;
        }

        public void Init(CardEntity cardEntity, Player player)
        {
            model = new CardModel(cardEntity);
            owner = player;
            view.Show(model);
        }

        public int Attack(CardController enemyCard)
        {
            audioManager.PlaySE(SE.Attack);
            int at = model.Attack(enemyCard);
            SetCanAttack(false);
            return at;
        }

        public void Heal(CardController friendCard)
        {
            audioManager.PlaySE(SE.Heal);
            model.Heal(friendCard);
            friendCard.view.Refresh(model);
        }

        public void SetCanAttack(bool canAttack)
        {
            model.SetCanAttack(canAttack);
            view.SetActiveSelectablePanel(canAttack);
        }

        public void CheckAlive()
        {
            view.GetDamageInfo().SetActive(true);
            RefreshOrDestoroy();
        }

        void RefreshOrDestoroy()
        {
            DamageAnimation(view.GetDamageInfo().transform);
            // hpが0になったらオブジェクトを消す
            if(model.IsAlive()) {
                RefreshView();
            } else {
                DestroyCard();
            }
        }

        void DamageAnimation(Transform transform){
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMove(new Vector3(0f,20.0f,0f), 0.5f).SetEase(Ease.InOutQuart));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x - 25,0f,0f), 0.02f));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x + 50,0f,0f), 0.02f));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x - 25,0f,0f), 0.02f));
      
            RewindDamageInfo();
        }

        void RewindDamageInfo()
        {
            view.GetDamageInfo().SetActive(false);
            view.GetDamageInfo().transform.DORewind();
        }

        void RefreshView()
        {
            view.Refresh(model);
        }

        void DestroyCard()
        {
            audioManager.PlaySE(SE.Died);
            if(model.IsSpell() == true) {
                Instantiate(view.explosionParticleSpell, transform.position, view.explosionParticleSpell.transform.rotation);
            } else {
                Instantiate(view.explosionParticleMonster, transform.position, view.explosionParticleSpell.transform.rotation);
            }
            if(this.gameObject != null)
            {
                Destroy(this.gameObject);
            }
        }

        public bool CanUseSpell(CardController[] targetCards)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARD:
                case SPELL.DAMAGE_ENEMY_CARDS:
                    // 相手フィールドの全てのカードに攻撃する
                    if (targetCards.Length > 0)
                    {
                        return true;
                    }
                    return false;
                case SPELL.DAMAGE_ENEMY_HERO:
                case SPELL.HEAL_FRIEND_HERO:
                    return true;
                case SPELL.HEAL_FRIEND_CARD:
                case SPELL.HEAL_FRIEND_CARDS:
                    if (targetCards.Length > 0)
                    {
                        return true;
                    }
                    return false;
                case SPELL.NONE:
                    return false;
            }
            return false;
        }

        public IEnumerator UseSpellTo(CardController targetCard, HeroController ownerHero)
        {
 UnityEngine.Debug.Log(model.GetSpell().ToString());
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARD:
                UnityEngine.Debug.Log("usespell_enemycard");
                    // 特定の敵を攻撃する
                    if (targetCard != null && targetCard.GetOwner() != owner)
                    {
                        Attack(targetCard);
                        targetCard.CheckAlive();
                    }
                    break;
                case SPELL.HEAL_FRIEND_CARD:
                    if (targetCard != null && targetCard.GetOwner() == owner)
                    {
                        Heal(targetCard);
                    }
                    break;
                case SPELL.NONE:
                    break;
            }
            yield return new WaitForSeconds(0.5f);
            // カードを破棄する
            DestroyCard();

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public IEnumerator UseSpellTo(CardController[] targetCards, HeroController ownerHero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARDS:
                    // 相手フィールドの全てのカードに攻撃する
                    foreach (CardController targetCard in targetCards)
                    {
                        Attack(targetCard);
                    }
                    foreach (CardController targetCard in targetCards)
                    {
                        targetCard.CheckAlive();
                    }
                    break;
                case SPELL.HEAL_FRIEND_CARDS:
                    foreach (CardController targetCard in targetCards)
                    {
                        Heal(targetCard);
                    }
                    break;
                case SPELL.NONE:
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);
            // カードを破棄する
            DestroyCard();

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public IEnumerator UseSpellTo(HeroController target, HeroController ownerHero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_HERO:
                    target.Attacked(this);
                    break;
                case SPELL.HEAL_FRIEND_HERO:
                    target.Healed(this);
                    break;
                case SPELL.NONE:
                    break;
            }

            yield return new WaitForSeconds(0.5f);
            // カードを破棄する
            DestroyCard();

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public Player GetOwner()
        {
            return owner;
        }
        public bool IsDraggable()
        {
            return isDraggable;
        }
        public void SetDraggable(bool isDraggable)
        {
            this.isDraggable = isDraggable;
        }
    }
}