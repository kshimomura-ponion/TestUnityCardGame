using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TestUnityCardGame.Domain.Card;
using TestUnityCardGame.Presenter.Card;

namespace TestUnityCardGame.View.Card
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI hpText;
        [SerializeField] TextMeshProUGUI atText;
        [SerializeField] TextMeshProUGUI manaCostText;
        [SerializeField] Image iconImage;

        // セレクト可能なカードにはオーラをつける
        [SerializeField] GameObject selectablePanel;

        // アビリティ表示アイコン
        [SerializeField] Image abilityIconImage;

        // 消滅エフェクト
        [SerializeField] public ParticleSystem explosionParticleMonster;
        [SerializeField] public ParticleSystem explosionParticleSpell;

        // 表面パネル
        [SerializeField] GameObject frontPanel;

        // 背面パネル
        [SerializeField] GameObject backPanel;

        // ダメージ表示オブジェクト
        [SerializeField] GameObject damageInfo;
        [SerializeField] TextMeshProUGUI damageInfoText;

        public void Show(CardModel model)
        {
            nameText.text = model.GetName();
            hpText.text = model.GetHP().ToString();
            atText.text = model.GetAT().ToString();
            manaCostText.text = model.GetManaCost().ToString();
            iconImage.sprite = model.GetIcon();

            abilityIconImage.sprite = model.GetSkillIcon();

            // スペルカードにHPはないため
            if (model.GetSpell() != Spell.None) {
                hpText.gameObject.SetActive(false);
            }
        }

        public void Refresh(CardModel model)
        {
            hpText.text = model.GetHP().ToString();
            atText.text = model.GetAT().ToString();
        }

        public void SetActiveSelectablePanel(bool flag)
        {
            selectablePanel.SetActive(flag);
        }

        public void SetActiveFrontPanel(bool flag)
        {
            frontPanel.SetActive(flag);

            // 攻撃表示オーラは裏表紙になっている時は開かない
            if(!flag) {
                SetActiveSelectablePanel(false);
            }
        }

        public void SetActiveBackPanel(bool flag)
        {
            backPanel.SetActive(flag);
        }

        public void SetDamageInfoText(string text)
        {
            damageInfoText.text = text;
        }

        public GameObject GetDamageInfo()
        {
            return damageInfo;
        }
    }
}
