using System;
using UnityEngine;
using UniRx;


public class Box : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int boxScore = 100;

    [HideInInspector] public ItemType requestedItem;

    private Subject<(bool isCorrect, int score)> onItemProcessed = new Subject<(bool, int)>();
    public IObservable<(bool isCorrect, int score)> OnItemProcessed => onItemProcessed;

    // 成否判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        Item item = collision.transform.GetComponent<Item>();
        if (item != null)
        {
            bool isCorrect = (item.myItemType == requestedItem);

            if (isCorrect)
            {
                Debug.Log(gameObject.name + "：正解！ スコア: " + boxScore);
            }
            else
            {
                Debug.Log(gameObject.name + "：違う！");
            }
            onItemProcessed.OnNext((isCorrect, boxScore));
        }
        Destroy(collision.gameObject);
    }

    // Game Managerが次のお題をセット
    public void SetRequest(ItemType type, Sprite sprite)
    {
        requestedItem = type;
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    // Game Managerが成否の画像をセット
    public void ShowResultIcon(Sprite resultSprite)
    {
        spriteRenderer.sprite = resultSprite;
    }
    
    private void OnDestroy()
    {
        onItemProcessed.OnCompleted();
        onItemProcessed.Dispose();
    }
}
