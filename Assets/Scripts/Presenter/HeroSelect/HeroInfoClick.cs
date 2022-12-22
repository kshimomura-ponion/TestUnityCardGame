using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame.Presenter.HeroSelect
{
public class HeroInfoClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        HeroInfoController heroInfo = eventData.pointerPress.GetComponent<HeroInfoController>();
        if(heroInfo != null)
        {
            HeroSelectViewController.Instance.ClickedHeroInfoCard(heroInfo);
        }
    }
}
}