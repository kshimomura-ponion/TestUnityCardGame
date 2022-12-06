using UnityEngine;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    [System.NonSerialized] public CardView view;
    [System.NonSerialized] public CardModel model;
    [System.NonSerialized] public CardMovement movement;
    private float xDestination = 0.0f;

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardId)
    {
        // カード情報の生成
        model = new CardModel(cardId);
        view.Show(model);
    }

    public void CheckAlive()
    {
        view.damageInfo.SetActive(true);
        Invoke("RefreshOrDestoroy", 2.0f);
    }

    void RefreshOrDestoroy()
    {
        view.damageInfo.transform.DOLocalMove(new Vector3(0f,10f,0f), 0.1f);
        xDestination = transform.position.x - 25;
        XAxisTransForm();
        xDestination = transform.position.x + 25;
        Invoke("XAxisTransForm", 0.1f);
        xDestination = transform.position.x + 25;
        Invoke("XAxisTransForm", 0.1f);
        xDestination = transform.position.x - 25;
        Invoke("XAxisTransForm", 0.1f);
        view.damageInfo.SetActive(false);
        xDestination = 0.0f;

        // hpが0になったらオブジェクトを消す
        if(model.isAlive)
        {
            view.Refresh(model);
        } else {
            Invoke("DestroyCard", 0.5f);
        }
    }

    void XAxisTransForm()
    {
        transform.DOLocalMove(new Vector3(xDestination,0f,0f), 0.02f);
    }

    void DestroyCard()
    {
        Instantiate(view.explosionParticle, transform.position, view.explosionParticle.transform.rotation);
        Destroy(this.gameObject);
    }
}
