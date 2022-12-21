using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TestUnityCardGame
{
public class HeroInfoDialog : MonoBehaviour
{
    [SerializeField] GameObject heroInfoDialog;

    [System.NonSerialized] public PLAYER owner;
    [SerializeField] public  Image selectHeroIcon;
    [SerializeField] public  TextMeshProUGUI selectHeroName;
    [SerializeField] public  TextMeshProUGUI selectHeroInfo;

    [System.NonSerialized] public int hero1ID = 0;
    [System.NonSerialized] public int hero2ID = 0;

    public void Awake()
    {
        heroInfoDialog.SetActive(false);
    }
    // Hero Info DialogのOKボタンが押された時
    public void PushedHeroInfoOKButton()
    {
        HeroInfoController[] hero1InfoList = HeroSelectViewModel.Instance.GetHeroInfoList(PLAYER.PLAYER1);
        foreach (HeroInfoController heroInfo in hero1InfoList)
        {

            if(heroInfo.model.GetHeroID() == hero1ID){
                heroInfo.view.ShowSelectedPanel();
            } else {
                heroInfo.view.HideSelectedPanel();
            }
        }

        HeroInfoController[] hero2InfoList = HeroSelectViewModel.Instance.GetHeroInfoList(PLAYER.PLAYER2);
        foreach (HeroInfoController heroInfo in hero2InfoList)
        {
            if(heroInfo.model.GetHeroID() == hero2ID){
                heroInfo.view.ShowSelectedPanel();
            } else {
                heroInfo.view.HideSelectedPanel();
            }
        }

        if(owner == PLAYER.PLAYER1){
            HeroSelectViewModel.Instance.hero1ID = hero1ID;
        } else {
            HeroSelectViewModel.Instance.hero2ID = hero2ID;
        }
        HideHeroInfoDialog();
    }

     public void HideHeroInfoDialog()
    {
        heroInfoDialog.SetActive(false);
    }

    public void ShowHeroInfoDialog(int heroID, PLAYER player, Sprite icon, string name, string info)
    {
        if(player == PLAYER.PLAYER1){
            this.hero1ID = heroID;
        } else {
            this.hero2ID = heroID;
        }

        owner = player;
        selectHeroIcon.sprite = icon;
        selectHeroName.text = name;
        selectHeroInfo.text = info;

        heroInfoDialog.SetActive(true);
    }
}
}