using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TestUnityCardGame
{
public class HeroController : MonoBehaviour
{
    [System.NonSerialized] public HeroView view;
    [System.NonSerialized] public HeroModel model;
    private float xDestination = 0.0f;

    // ターン数
    private int turnNumber;
    
    public void Init(HeroEntity heroEntity, List<int> deck, PLAYER player)
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
        SoundManager.instance.PlaySE(SE.ATTACK);
        model.Damage(attacker.model.GetAT());
        view.SetDamageInfoText("-" + attacker.model.GetAT().ToString());
        view.GetDamageInfo().SetActive(true);
        RefreshByDamage();
        attacker.SetCanAttack(false);
        //GameManager.instance.CheckHeroHP();
    }

    public void Healed(CardController healer)
    {
        SoundManager.instance.PlaySE(SE.HEAL);
        model.Heal(healer.model.GetAT());
        view.SetDamageInfoText("+" + healer.model.GetAT().ToString());
        view.GetDamageInfo().SetActive(true);
        RefreshByDamage();
    }

    void RefreshByDamage()
    {
        view.GetDamageInfo().transform.DOLocalMove(new Vector3(0f,180.0f,0f), 0.5f).SetEase(Ease.InOutQuart).OnComplete(MoveXAxis);
        view.Refresh(model);
    }

    void MoveXAxis(){
        xDestination = transform.position.x - 25;
        XAxisTransForm();
        xDestination = transform.position.x + 25;
        Invoke("XAxisTransForm", 0.1f);
        xDestination = transform.position.x + 25;
        Invoke("XAxisTransForm", 0.1f);
        xDestination = transform.position.x - 25;
        Invoke("XAxisTransForm", 0.1f);
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

    void XAxisTransForm()
    {
        transform.DOLocalMove(new Vector3(xDestination,0f,0f), 0.02f).SetEase(Ease.InOutQuart);
    }

    void RewindDamageInfo()
    {
        view.GetDamageInfo().SetActive(false);
        view.GetDamageInfo().transform.DORewind();
        xDestination = 0.0f;
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