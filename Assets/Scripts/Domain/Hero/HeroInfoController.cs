using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroInfoController : MonoBehaviour
{
    [System.NonSerialized] Sprite iconImage;
    [System.NonSerialized] int heroInfoID;  // ランダムは0、その他はHeroIDと同じ
    [System.NonSerialized] Player owner;
    [System.NonSerialized] public HeroInfoView view;
    [System.NonSerialized] public HeroModel model;

    public void Init(HeroEntity heroEntity, Player player, bool isRandom)
    {
        List<int> deck = new List<int>();
        model = new HeroModel(heroEntity, deck, player);
        view = GetComponent<HeroInfoView>();
        view.Show(model, isRandom);
        owner = player;
        if(isRandom){
            heroInfoID = 0;
        } else {
            heroInfoID = model.GetHeroID();
        }
    }

    public void UpdateModel(HeroEntity heroEntity)
    {
        List<int> deck = new List<int>();
        model = new HeroModel(heroEntity, deck, owner);
    }

    public int GetHeroInfoID()
    {
        return heroInfoID;
    }

    public Player GetOwner()
    {
        return owner;
    }
}