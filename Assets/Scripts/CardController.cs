using System.Diagnostics;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view;
    CardModel model;

    private void Awake()
    {
        view = GetComponent<CardView>();
    }

    public void Init(int cardId)
    {
        // カード情報の生成
        model = new CardModel(cardId);
        view.Show(model);
    }
}
