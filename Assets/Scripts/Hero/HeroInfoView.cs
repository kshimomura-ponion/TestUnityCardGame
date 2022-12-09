using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroInfoView : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [System.NonSerialized] int heroID;
    [System.NonSerialized] PLAYER player;
    // 選択中のHeroにオーラをつける
    [SerializeField] GameObject selectedPanel;

    public void Init(int id, PLAYER owner)
    {
        HeroEntity heroEntity = Resources.Load<HeroEntity>("HeroEntities/Hero"+ id.ToString());
        heroID = id;
        iconImage.sprite = heroEntity.leftIcon;
        player = owner;
    }
}