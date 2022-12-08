using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;
using TMPro;

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

        // 右上のアイコンパネルに貼り付ける画像をアビリティごとに変える
        if(model.GetAbility() == ABILITY.SHIELD) {
            abilityIconImage.sprite = Resources.Load<Sprite>("FreeIcons/Paladin14");
        } else if (model.GetAbility() == ABILITY.INIT_ATTACKABLE) {
            abilityIconImage.sprite = Resources.Load<Sprite>("FreeIcons/Barbarian14");
        }

        if (model.GetSpell() != SPELL.NONE) {
            hpText.gameObject.SetActive(false);
            abilityIconImage.sprite = Resources.Load<Sprite>("FreeIcons/Enchanter18");
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
