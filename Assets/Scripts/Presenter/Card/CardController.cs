using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Battle;
using TestUnityCardGame.View.Card;

namespace TestUnityCardGame.Presenter.Card
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
            if (model.IsAlive()) {
                RefreshView();
            } else {
                StartCoroutine(DestroyCard());
            }
        }

        void DamageAnimation(Transform transform) {
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

        IEnumerator DestroyCard()
        {
            audioManager.PlaySE(SE.Died);
            if (model.IsSpell()) {
                Instantiate(view.explosionParticleSpell, transform.position, view.explosionParticleSpell.transform.rotation);
            } else {
                Instantiate(view.explosionParticleMonster, transform.position, view.explosionParticleSpell.transform.rotation);
            }
            yield return new WaitForSeconds(0.5f);
            if (this.gameObject != null) {
                Destroy(this.gameObject);
            }
        }

        public IEnumerator UseSpellToCard(CardController targetCard)
        {
            switch (model.GetSpell()) {
                case Spell.AttackEnemyCard:
                    // 特定の敵を攻撃する
                    if (targetCard != null && targetCard.GetOwner() != owner)
                    {
                        Attack(targetCard);
                        targetCard.CheckAlive();
                    }
                    break;
                case Spell.HealFriendCard:
                    if (targetCard != null && targetCard.GetOwner() == owner)
                    {
                        Heal(targetCard);
                    }
                    break;
                case Spell.None:
                    break;
            }
            yield return new WaitForSeconds(0.5f);

            // カードを破棄する
            StartCoroutine(DestroyCard());
        }

        public IEnumerator UseSpellToCards(CardController[] targetCards)
        {
            switch (model.GetSpell()) {
                case Spell.AttackEnemyCards:
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
                case Spell.HealFriendCards:
                    foreach (CardController targetCard in targetCards)
                    {
                        Heal(targetCard);
                    }
                    break;
                case Spell.None:
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);

            // カードを破棄する
            StartCoroutine(DestroyCard());
        }

        public IEnumerator UseSpellToHero(HeroController target)
        {
            switch (model.GetSpell()) {
                case Spell.AttackEnemyHero:
                    target.Attacked(this);
                    break;
                case Spell.HealFriendHero:
                    target.Healed(this);
                    break;
                case Spell.None:
                    break;
            }

            yield return new WaitForSeconds(0.5f);

            // カードを破棄する
            StartCoroutine(DestroyCard());
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