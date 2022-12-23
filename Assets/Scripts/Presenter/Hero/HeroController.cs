using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TestUnityCardGame.Presenter.Card;

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
            //GameManager.instance.CheckHeroHP();
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
            DamageAnimation(view.GetDamageInfo().transform);
            RefreshView();
        }

        void DamageAnimation(Transform transform){
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMove(new Vector3(0f,20.0f,0f), 0.5f).SetEase(Ease.InOutQuart));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x - 25,0f,0f), 0.02f));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x + 50,0f,0f), 0.02f));
            sequence.Append(transform.DOLocalMove(new Vector3(transform.position.x - 25,0f,0f), 0.02f));
    
            RewindDamageInfo();
        }

        public void AddManaCost(int cost){
            model.AddManaCost(cost);
            view.SetReduceManaCostInfoText("+" + cost.ToString());
            view.GetReduceManaCostInfo().SetActive(true);
            RefreshByReduceManaCost();
        }

        public void ReduceManaCost(int cost)
        {
            model.ReduceManaCost(cost);
            view.SetReduceManaCostInfoText("-" + cost.ToString());
            view.GetReduceManaCostInfo().SetActive(true);
            RefreshByReduceManaCost();
        }

        void RefreshByReduceManaCost()
        {
            view.GetReduceManaCostInfo().transform.DOLocalMove(new Vector3(0f,180.0f,0f), 0.5f).SetEase(Ease.InOutQuart).OnComplete(RewindReduceManaCostInfo);
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
            view.GetReduceManaCostInfo().SetActive(false);
            view.GetReduceManaCostInfo().transform.DORewind();
        }

        public int GetTurnNumber(){
            return turnNumber;
        }

        public void AddTurnNumber(){
            turnNumber++;
        }
    }
}