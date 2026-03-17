using System;
using UnityEngine;
using UniRx;


public class Box : MonoBehaviour
{
    [HideInInspector] public ItemType requestedItem;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Subject<bool> onItemProcessed = new Subject<bool>();
    public IObservable<bool> OnItemProcessed => onItemProcessed;

    void Start() {}
    void Update() {}

    void OnCollisionEnter2D(Collision2D collision)
    {
        Item item = collision.transform.GetComponent<Item>();
        if (item != null)
        {
            bool isCorrect = (item.myItemType == requestedItem);

            if (isCorrect)
            {
                Debug.Log(gameObject.name + "：正解！");
            }
            else
            {
                Debug.Log(gameObject.name + "：違う！");
            }

            // ★Managerを直接呼ぶのではなく、結果(true/false)をストリームに流すだけ！
            onItemProcessed.OnNext(isCorrect);
        }
        Destroy(collision.gameObject);
    }

    public void SetRequest(ItemType type, Sprite sprite)
    {
        requestedItem = type;
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    // ★追加：Boxが破棄された時にストリームを閉じる（メモリリーク防止）
    private void OnDestroy()
    {
        onItemProcessed.OnCompleted();
        onItemProcessed.Dispose();
    }
}
