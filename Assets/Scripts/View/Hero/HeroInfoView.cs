using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TestUnityCardGame.Domain.Hero;

namespace TestUnityCardGame.View.Hero
{
    public class HeroInfoView : MonoBehaviour
    {
        [SerializeField] Image iconImage;

        // 選択中のHeroにオーラをつける
        [SerializeField] GameObject selectedPanel;

        // ランダムの場合
        [SerializeField] GameObject randomSelectPanel;

        public void Show(HeroModel model, bool isRandom)
        {
            if (isRandom) {
                randomSelectPanel.SetActive(true);
            } else {
                iconImage.sprite = model.GetIcon();
                randomSelectPanel.SetActive(false);
            }
        }

        public void ShowSelectedPanel()
        {
            selectedPanel.SetActive(true);
        }

        public void HideSelectedPanel()
        {
            selectedPanel.SetActive(false);
        }
    }
}