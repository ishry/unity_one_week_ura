using System;
using UnityEngine;
using UniRx;


public class Box : MonoBehaviour
{
    [HideInInspector] public ItemType requestedItem;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int boxScore = 100;

    private Subject<(bool isCorrect, int score)> onItemProcessed = new Subject<(bool, int)>();
    public IObservable<(bool isCorrect, int score)> OnItemProcessed => onItemProcessed;

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

    public void SetRequest(ItemType type, Sprite sprite)
    {
        requestedItem = type;
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

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
