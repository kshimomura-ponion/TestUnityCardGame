using System.Reflection;
using System.Globalization;
using System.ComponentModel.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;
using TestUnityCardGame.Presenter.Common;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.Battle;
using TestUnityCardGame.Domain.Card;
using TestUnityCardGame.View.Card;

namespace TestUnityCardGame.Presenter.Card
{
    public class CardController : MonoBehaviour
    {
        private SoundManager soundManager;

        [System.NonSerialized] public CardView view;
        [System.NonSerialized] public CardModel model;
        [System.NonSerialized] public CardMovement movement;

        private Player owner;
        private bool isDraggable;
        private bool isAttackable;
        private bool isFieldCard;

        private void Awake()
        {
            view = GetComponent<CardView>();
            movement = GetComponent<CardMovement>();
            isDraggable = false;

            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");
        }

        public void Init(CardEntity cardEntity, Player player)
        {
            model = new CardModel(cardEntity);
            owner = player;
            view.Show(model);
        }

        public void OnField()
        {
            this.isFieldCard = true;

            // 速攻カードの場合
            if (model.GetAbility() == Ability.InitAttackable) {
                SetAttackable(true);
            }
        }
        public int Attack(CardController enemyCard)
        {
            if (soundManager != null) soundManager.PlaySE(SE.Attack);
            int at = model.Attack(enemyCard);
            SetAttackable(false);
            return at;
        }

        public int Heal(CardController friendCard)
        {
            if (soundManager != null) soundManager.PlaySE(SE.Heal);
            int at = model.Heal(friendCard);
            friendCard.view.Refresh(model);
            return at;
        }

        public void SetAttackable(bool isAttackable)
        {
            this.isAttackable = isAttackable;
            view.SetActiveSelectablePanel(isAttackable);
        }

        public bool IsAttackable()
        {
            return this.isAttackable;
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
        }

        IEnumerator DamageAnimation() {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(view.GetDamageInfo().transform.DOLocalMove(new Vector3(0f, 10.0f, 0f), 0.5f).SetEase(Ease.InOutQuart));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x - 25, 0f, 0f), 0.05f));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x + 50,0f,0f), 0.1f));
            sequence.Append(this.transform.DOLocalMove(new Vector3(this.transform.position.x - 25,0f,0f), 0.05f));
            sequence.Play().OnComplete(RewindDamageInfo);
            yield return new WaitForSeconds(0.01f);
            view.GetDamageInfo().transform.DORewind();
        }

        void RewindDamageInfo()
        {
            view.GetDamageInfo().transform.DOLocalMove(new Vector3(0f,-10f, 0f), 0.1f);
            view.GetDamageInfo().SetActive(false);
            this.transform.DORewind();
        }

        void RefreshView()
        {
            view.Refresh(model);
        }

        IEnumerator DestroyCard()
        {
            if (soundManager != null) soundManager.PlaySE(SE.Died);
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

        public IEnumerator UseSpellToCard(CardController targetCard, HeroController ownerHero, Transform movePosition)
        {
            int targetNum = 0;

            // カードを移動
            if (movement != null && movePosition != null) {
                var moveToField = movement.MoveToField(movePosition);
                while(moveToField.MoveNext()){}
            }

            switch (model.GetSpell()) {
                case Spell.AttackEnemyCard:
                    // 特定の敵を攻撃する
                    if (targetCard != null && targetCard.GetOwner() != owner)
                    {
                        int damage = Attack(targetCard);
                        targetCard.CheckAlive(-1 * damage);
                        targetNum++;
                    }
                    break;
                case Spell.HealFriendCard:
                    if (targetCard != null && targetCard.GetOwner() == owner)
                    {
                       int healed = Heal(targetCard);
                       targetCard.CheckAlive(healed);
                        targetNum++;
                    }
                    break;
                case Spell.None:
                    break;
            }
            yield return new WaitForSeconds(0.5f);

            if (targetNum > 0) {
                ownerHero.ReduceManaCost(model.GetManaCost());  // 使用者のManaCostを減らす
                StartCoroutine(DestroyCard());  // 使用後は破壊する
            }
        }

        public IEnumerator UseSpellToCards(CardController[] targetCards, HeroController ownerHero, Transform movePosition)
        {
            int targetNum = 0;

            // カードを移動
            if (movement != null && movePosition != null) {
                var moveToField = movement.MoveToField(movePosition);
                while(moveToField.MoveNext()){}
            }

            switch (model.GetSpell()) {
                case Spell.AttackEnemyCards:
                    // 相手フィールドの全てのカードに攻撃する
                    foreach (CardController targetCard in targetCards)
                    {
                        // 特定の敵を攻撃する
                        if (targetCard != null && targetCard.GetOwner() != owner)
                        {
                            int dmg = Attack(targetCard);
                            targetCard.CheckAlive(-1 * dmg);
                            targetNum++;
                        }
                    }
                    break;
                case Spell.HealFriendCards:
                    foreach (CardController targetCard in targetCards)
                    {
                        if (targetCard != null && targetCard.GetOwner() == owner)
                        {
                            int healed = Heal(targetCard);
                            targetCard.CheckAlive(healed);
                            targetNum++;
                        }
                    }
                    break;
                case Spell.None:
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);

            if (targetNum > 0) {
                ownerHero.ReduceManaCost(model.GetManaCost());  // 使用者のManaCostを減らす
                StartCoroutine(DestroyCard());  // 使用後は破壊する
            }
        }

        public IEnumerator UseSpellToHero(HeroController target, HeroController ownerHero, Transform movePosition)
        {
            // カードを移動
            if (movement != null && movePosition != null) {
                var moveToField = movement.MoveToField(movePosition);
                while(moveToField.MoveNext()){}
            }

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

            ownerHero.ReduceManaCost(model.GetManaCost());  // 使用者のManaCostを減らす
            StartCoroutine(DestroyCard());  // 使用後は破壊する
        }

        public bool IsFieldCard()
        {
            return isFieldCard;
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