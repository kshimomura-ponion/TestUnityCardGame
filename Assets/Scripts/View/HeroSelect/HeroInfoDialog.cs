using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;
using TestUnityCardGame.Presenter.Common;
using TestUnityCardGame.Presenter.Hero;
using TestUnityCardGame.Presenter.HeroSelect;

namespace TestUnityCardGame.View.HeroSelect
{
    public class HeroInfoDialog : MonoBehaviour
    {
        [SerializeField] GameObject heroInfoDialog;

        private SoundManager soundManager;
        [SerializeField] AudioSource audioSource;
        
        public Image selectHeroIcon;
        public TextMeshProUGUI selectHeroName;
        public TextMeshProUGUI selectHeroInfo;

        [System.NonSerialized] public Player owner;

        [System.NonSerialized] public int hero1ID = 0;
        [System.NonSerialized] public int hero2ID = 0;

        public void Awake()
        {
            heroInfoDialog.SetActive(false);

            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");
        }

        public void Hide()
        {
            if (soundManager != null) soundManager.PlaySE(SE.Cancel);
            heroInfoDialog.SetActive(false);
        }

       public void PushedHeroInfoOKButton()
        {
            if (soundManager != null) soundManager.PlaySE(SE.OK);
            HeroInfoController[] hero1InfoList = HeroSelectViewController.Instance.GetHeroInfoList(Player.Player1);
            foreach (HeroInfoController heroInfo in hero1InfoList) {

                if (heroInfo.model.GetHeroID() == hero1ID) {
                    heroInfo.view.ShowSelectedPanel();
                } else {
                    heroInfo.view.HideSelectedPanel();
                }
            }

            HeroInfoController[] hero2InfoList = HeroSelectViewController.Instance.GetHeroInfoList(Player.Player2);
            foreach (HeroInfoController heroInfo in hero2InfoList) {
                if (heroInfo.model.GetHeroID() == hero2ID) {
                    heroInfo.view.ShowSelectedPanel();
                } else {
                    heroInfo.view.HideSelectedPanel();
                }
            }

            if (owner == Player.Player1) {
                HeroSelectViewController.Instance.hero1ID = hero1ID;
            } else {
                HeroSelectViewController.Instance.hero2ID = hero2ID;
            }
            Hide();
        }

        public void Show(int heroID, Player player, Sprite icon, string name, string info)
        {
            if (soundManager != null) soundManager.PlaySE(SE.DialogOpen); // ダイアログが出たよという意味で

            if (player == Player.Player1) {
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