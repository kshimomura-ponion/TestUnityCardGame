using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TestUnityCardGame.Domain.Hero;

namespace TestUnityCardGame.View.Hero
{
    public class HeroView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI hpText;
        [SerializeField] TextMeshProUGUI manaCostText;
        [SerializeField] Image iconImage;

        // ダメージ表示オブジェクト
        [SerializeField] GameObject damageInfo;
        [SerializeField] TextMeshProUGUI damageInfoText;

        // マナコスト表示オブジェクト
        [SerializeField] GameObject manaCostInfo;
        [SerializeField] TextMeshProUGUI manaCostInfoText;

        // 現在のターンのHeroにはオーラをつける
        [SerializeField] GameObject activatedPanel;

        public void Show(HeroModel model)
        {
            nameText.text = model.GetName();
            hpText.text = model.GetHP().ToString();
            manaCostText.text = model.GetManaCost().ToString();
            iconImage.sprite = model.GetIcon();
        }

        public void Refresh(HeroModel model)
        {
            hpText.text = model.GetHP().ToString();
            manaCostText.text = model.GetManaCost().ToString();
        }

        public void SetActiveActivatedPanel(bool flag)
        {
            activatedPanel.SetActive(flag);
        }

        public void SetDamageInfoText(string text)
        {
            damageInfoText.text = text;
        }

        public void SetManaCostInfoText(string text)
        {
            manaCostInfoText.text = text;
        }

        public GameObject GetDamageInfo()
        {
            return damageInfo;
        }

        public GameObject GetManaCostInfo()
        {
            return manaCostInfo;
        }
    }
}