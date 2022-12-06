using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI atText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image iconImage;

    // セレクト可能なカードにはオーラをつける
    [SerializeField] GameObject selectablePanel;

    public ParticleSystem explosionParticle;
    public GameObject damageInfo;
    public TextMeshProUGUI damageInfoText;

    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
    }

    public void Refresh(CardModel cardModel)
    {
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
    }

    public void SetActiveSelectablePanel(bool flag)
    {
        selectablePanel.SetActive(flag);
    }
}
