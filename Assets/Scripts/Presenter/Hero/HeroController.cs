using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using TestUnityCardGame.Domain.Sound;
using TestUnityCardGame.Domain.Hero;
using TestUnityCardGame.Presenter.Card;
using TestUnityCardGame.View.Hero;

namespace TestUnityCardGame.Presenter.Hero
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] AudioManager audioManager;
        [System.NonSerialized] public HeroView view;
        [System.NonSerialized] public HeroModel model;

        // ターン数
        private int turnNumber;
        
        public void Init(HeroEntity heroEntity, List<int> deck, Player player)
        {
            // カード情報の生成
            model = new HeroModel(heroEntity, deck, player);
            view = GetComponent<HeroView>();
            view.Show(model);

            // ターン数を1にセットする
            turnNumber = 1;
        }

        public void Attacked(CardController attacker)
        {
            audioManager.PlaySE(SE.Attack);
            model.Damage(attacker.model.GetAT());
            view.SetDamageInfoText("-" + attacker.model.GetAT().ToString());
            view.GetDamageInfo().SetActive(true);
            RefreshByDamage();
            attacker.SetCanAttack(false);
        }

        public void Healed(CardController healer)
        {
            audioManager.PlaySE(SE.Heal);
            model.Heal(healer.model.GetAT());
            view.SetDamageInfoText("+" + healer.model.GetAT().ToString());
            view.GetDamageInfo().SetActive(true);
            RefreshByDamage();
        }

        void RefreshByDamage()
        {
            var damageAnimation = DamageAnimation();
            while(damageAnimation.MoveNext()) {}
            RefreshView();
            if (model.GetHP().Value <= 0) {
                audioManager.PlaySE(SE.Died);
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
            yield return new WaitForSeconds(0.1f);
        }

        public void AddManaCost(int cost) {
            model.AddManaCost(cost);
            view.SetManaCostInfoText("+" + cost.ToString());
            view.GetManaCostInfo().SetActive(true);
            RefreshByChangedManaCost();
        }

        public void ReduceManaCost(int cost)
        {
            model.ReduceManaCost(cost);
            view.SetManaCostInfoText("-" + cost.ToString());
            view.GetManaCostInfo().SetActive(true);

            RefreshByChangedManaCost();
        }

        void RefreshByChangedManaCost()
        {
            view.GetManaCostInfo().transform.DOLocalMove(new Vector3(0f,180.0f,0f), 0.5f).SetEase(Ease.InOutQuart).OnComplete(RewindReduceManaCostInfo);
            view.Refresh(model);
        }

        void RefreshView()
        {
            view.Refresh(model);
        }

        void RewindDamageInfo()
        {
            view.GetDamageInfo().SetActive(false);
            view.GetDamageInfo().transform.DORewind();
        }
        void RewindReduceManaCostInfo()
        {
            view.GetManaCostInfo().SetActive(false);
            view.GetManaCostInfo().transform.DORewind();
        }

        public int GetTurnNumber() {
            return turnNumber;
        }

        public void AddTurnNumber() {
            turnNumber++;
        }
    }
}