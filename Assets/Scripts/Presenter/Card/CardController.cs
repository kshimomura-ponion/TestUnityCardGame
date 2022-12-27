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

        public int Heal(CardController friendCard)
        {
            audioManager.PlaySE(SE.Heal);
            int at = model.Heal(friendCard);
            friendCard.view.Refresh(model);
            return at;
        }

        public void SetCanAttack(bool canAttack)
        {
            model.SetCanAttack(canAttack);
            view.SetActiveSelectablePanel(canAttack);
        }

        public void CheckAlive(int damage)
        {
            view.SetDamageInfoText(damage.ToString());
            view.GetDamageInfo().SetActive(true);
            RefreshOrDestoroy();
        }

        void RefreshOrDestoroy()
        {
            var damageAnimation = DamageAnimation();
            while(damageAnimation.MoveNext()) {}
            // hpが0になったらオブジェクトを消す
            if (model.IsAlive()) {
                RefreshView();
            } else {
                StartCoroutine(DestroyCard());
            }
            RewindDamageInfo();
        }

        IEnumerator DamageAnimation() {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(view.GetDamageInfo().transform.DOLocalMove(new Vector3(0f, 20.0f, 0f), 0.5f).SetEase(Ease.InOutQuart));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x - 25, 0f, 0f), 0.05f));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x + 50,0f,0f), 0.1f));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x - 25,0f,0f), 0.05f));
            sequence.Play();
            yield return new WaitForSeconds(0.01f);
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
                        int damage = Attack(targetCard);
                        targetCard.CheckAlive(-1 * damage);
                    }
                    break;
                case Spell.HealFriendCard:
                    if (targetCard != null && targetCard.GetOwner() == owner)
                    {
                       int healed = Heal(targetCard);
                       targetCard.CheckAlive(healed);
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
                        int dmg = Attack(targetCard);
                        targetCard.CheckAlive(-1 * dmg);
                    }
                    break;
                case Spell.HealFriendCards:
                    foreach (CardController targetCard in targetCards)
                    {
                        int healed = Heal(targetCard);
                        targetCard.CheckAlive(healed);
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