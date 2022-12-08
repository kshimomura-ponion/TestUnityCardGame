using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // ドラッグ中にカードが裏に隠れてしまうため、現在のカメラ位置と3DのZ座標を保持しておく
    private Camera mainCamera;
    private float piecePosZ;

    // 親コンテナの位置座標
    private Transform defaultParent;

    // ドラッグ可能フラグ
    private bool isDraggable;

    void Start()
    {
        defaultParent = transform.parent;
        mainCamera = Camera.main;
        piecePosZ = transform.position.z;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとPlayerのMana Costを比較
        CardController card = GetComponent<CardController>();
        if(!card.model.IsFieldCard() && card.model.GetManaCost() <= GameManager.instance.player1Hero.model.GetManaCost())
        {
            isDraggable = true;
        }
        else if (card.model.IsFieldCard() && card.model.CanAttack())
        {
            isDraggable = true;
        }
        else
        {
            isDraggable = false;
        }

        if(!isDraggable)
        {
            return;
        }

        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);

        // カードを移動させるときはマウスポインタ（光線）を遮らない
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isDraggable)
        {
            return;
        }   

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = piecePosZ;
        transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaultParent, false);
        // カードの移動が終わったらマウスポインタ（光線）を遮る
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public IEnumerator MoveToField(Transform field)
    {
        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }

    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と並びを取得
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをTargetに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        // 元の位置と並びに戻す
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex);

        yield return new WaitForSeconds(0.25f);
    }

    public void SetDefaultParent(Transform parentTransform)
    {
        defaultParent = parentTransform;
    }

    public Transform GetDefaultParent()
    {
        return defaultParent;
    }

    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }

    public bool IsDraggable()
    {
        return isDraggable;
    }
}
