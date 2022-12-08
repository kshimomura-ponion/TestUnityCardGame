using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI manaCostText;
    [SerializeField] Image iconImage;

    // ダメージ表示オブジェクト
    [SerializeField] GameObject damageInfo;
    [SerializeField] TextMeshProUGUI damageInfoText;

    // マナコスト減少表示オブジェクト
    [SerializeField] GameObject reduceManaCostInfo;
    [SerializeField] TextMeshProUGUI reduceManaCostInfoText;

    // 現在のターンのHeroにはオーラをつける
    [SerializeField] GameObject selectablePanel;

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

    public void SetActiveSelectablePanel(bool flag)
    {
        selectablePanel.SetActive(flag);
    }

    public void SetDamageInfoText(string text)
    {
        damageInfoText.text = text;
    }

    public void SetReduceManaCostInfoText(string text)
    {
        reduceManaCostInfoText.text = text;
    }

    public GameObject GetDamageInfo()
    {
        return damageInfo;
    }

    public GameObject GetReduceManaCostInfo()
    {
        return reduceManaCostInfo;
    }
}
