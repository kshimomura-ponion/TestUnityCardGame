using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;
using TMPro;

namespace TestUnityCardGame
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
    [SerializeField] public ParticleSystem explosionParticle;

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
        if (model.GetSpell() != SPELL.NONE) {
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
