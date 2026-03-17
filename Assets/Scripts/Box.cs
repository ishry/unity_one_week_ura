using UnityEngine;

public class Box : MonoBehaviour
{
    [HideInInspector] public ItemType requestedItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Item item = collision.transform.GetComponent<Item>();
        if (item != null)
        {
            ItemType receivedType = item.myItemType;
            
            // 判定テスト
            if (receivedType == requestedItem)
            {
                Debug.Log(gameObject.name + "：正解！ " + receivedType + " をもらった！");
                // TODO: Managerにスコア加算を報告し、次のお題をもらう
            }
            else
            {
                Debug.Log(gameObject.name + "：違う！ 欲しいのは " + requestedItem + " だ！");
                // TODO: ペナルティ処理
            }
        }
        Destroy(collision.gameObject);
    }

    // GameManagerから呼ばれる
    public void SetRequest(ItemType type)
    {
        requestedItem = type;
        Debug.Log(gameObject.name + " の新しい要求: " + requestedItem);

        // ※いずれここに、要求されたアイテムのドット絵アイコンを
        // 人の頭上のUI（吹き出しなど）に表示する処理を書くと綺麗にまとまります。
    }
}
