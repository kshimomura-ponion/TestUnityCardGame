using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // ドラッグ中にカードが裏に隠れてしまうため、現在のカメラ位置と3DのZ座標を保持しておく
    private Camera mainCamera;
    private float piecePosZ;
    public Transform defaultParent;

    void Start()
    {
        mainCamera = Camera.main;
        piecePosZ = transform.position.z;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);
        // カードを移動させるときはマウスポインタ（光線）を遮らない
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
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
}
