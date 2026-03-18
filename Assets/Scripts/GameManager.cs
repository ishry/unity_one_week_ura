using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public enum ItemType
{
    cell,
    lever,
    screw,
    spring,
}

[System.Serializable]
public struct ItemData
{
    public ItemType itemType;
    public GameObject prefab;
    public Sprite iconSprite;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private ItemGenerator itemGenerator;
    [SerializeField] private GameObject[] boxes;
    [SerializeField] private ItemData[] itemDatabase;
    [SerializeField] private TMP_Text scoreText;

    [Header("Life UI")]
    [SerializeField] private Image[] lifeImages; // 3つのハートのImageを格納する配列
    [SerializeField] private Sprite heartFull;   // 満タンのハート画像
    [SerializeField] private Sprite heartEmpty;  // 空のハート画像

    [Header("Result UI")]
    [SerializeField] private Sprite circleSprite; // マルの画像
    [SerializeField] private Sprite crossSprite;  // バツの画像
    [SerializeField] private float resultDisplayTime = 1.0f; // 表示時間

    [Header("Game Settings")]
    [SerializeField] private float initialWaitTime = 2.0f; // 開始までの待機時間（秒）

    private int score = 0;
    private int life = 3;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 各Boxのイベントストリームを購読する
        foreach (GameObject boxObj in boxes)
        {
            Box box = boxObj.GetComponent<Box>();
            if (box != null)
            {
                // Boxが「OnNext」で流してきた結果(isCorrect)を受け取って処理する
                box.OnItemProcessed
                    .Subscribe(isCorrect => HandleBoxProcessed(box, isCorrect))
                    .AddTo(this); 
            }
        }
        UpdateTexts();
        UpdateLifeUI();
        StartCoroutine(StartGameRoutine());
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartGameRoutine()
    {
        // 指定した時間だけ待機する
        yield return new WaitForSeconds(initialWaitTime);

        // 待機が終わったら、最初のお題とアイテムをセットする
        AssignUniqueRequestsToBoxes();
        SpawnRandomItem();
    }

    // イベントを受け取った時の処理
    private void HandleBoxProcessed(Box box, bool isCorrect)
    {
        if (isCorrect)
        {
            AddScore(100);
            
            // TODO: 正解したこのboxに対して、新しいお題を再設定する
            // （他の3つのBoxと被らないItemTypeを選んで SetRequest を呼ぶ）
        }
        else
        {
            DecreaseLife();
        }
        UpdateTexts();
        UpdateLifeUI();
        StartCoroutine(ShowResultEffect(box, isCorrect));
    }

    private IEnumerator ShowResultEffect(Box targetBox, bool isCorrect)
    {

        // 1. 各Boxの画像を更新（入った箱にはマルかバツ、それ以外は透明）
        foreach (GameObject boxObj in boxes)
        {
            Box b = boxObj.GetComponent<Box>();
            if (b != null)
            {
                if (b == targetBox)
                {
                    b.ShowResultIcon(isCorrect ? circleSprite : crossSprite);
                }
                else
                {
                    b.ShowResultIcon(null);
                }
            }
        }

        // 2. 指定時間（1秒）待機
        yield return new WaitForSeconds(resultDisplayTime);

        // 3. ゲームオーバーでなければ次をセット
        if (life > 0)
        {
            AssignUniqueRequestsToBoxes(); // Boxの画像を再設定（透明度も白に戻る）
            SpawnRandomItem();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("スコアアップ！ 現在のスコア: " + score);
    }

    public void DecreaseLife()
    {
        life--;
        Debug.Log("ミス！ 残りライフ: " + life);

        UpdateLifeUI();

        if (life <= 0)
        {
            GameOver();
        }
    }

    public void UpdateTexts()
    {
        scoreText.text = "Score: " + score.ToString("00000");
    }

    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            // 現在のライフ数よりインデックスが小さければ「満タン」、それ以上なら「空」
            if (i < life)
            {
                lifeImages[i].sprite = heartFull;
            }
            else
            {
                lifeImages[i].sprite = heartEmpty;
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("ゲームオーバー！");
    }

    // 土管からランダムにアイテム生成
    public void SpawnRandomItem()
    {
        ItemType randomType = GetRandomItemType();
        GameObject prefabToSpawn = GetItemPrefab(randomType);
        if (itemGenerator != null && prefabToSpawn != null)
        {
            itemGenerator.SpawnItem(prefabToSpawn);
        }
        else
        {
            Debug.LogWarning("ItemGenerator または 生成するPrefab が設定されていません！");
        }
    }

    // 各boxの要求の割り当て
    public void AssignUniqueRequestsToBoxes()
    {
        ItemType[] allTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));
        for (int i = allTypes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            ItemType temp = allTypes[i];
            allTypes[i] = allTypes[j];
            allTypes[j] = temp;
        }
        for (int i = 0; i < boxes.Length; i++)
        {
            if (i < allTypes.Length) 
            {
                ItemType assignedType = allTypes[i];
                Sprite assignedSprite = GetItemSprite(assignedType); // データベースから画像を取得
                
                Box box = boxes[i].GetComponent<Box>();
                if (box != null)
                {
                    box.SetRequest(assignedType, assignedSprite);
                }
            }
        }
    }

    // 以下汎用関数

    // ランダムなItemTypeを返す関数
    public ItemType GetRandomItemType()
    {
        ItemType[] values = (ItemType[])System.Enum.GetValues(typeof(ItemType));
        return values[Random.Range(0, values.Length)];
    }

    // Typeを渡すと、対応するPrefabを返す関数
    public GameObject GetItemPrefab(ItemType type)
    {
        foreach (var data in itemDatabase)
        {
            if (data.itemType == type) return data.prefab;
        }
        return null;
    }

    // Typeを渡すと、対応するSpriteを返す関数
    public Sprite GetItemSprite(ItemType type)
    {
        foreach (var data in itemDatabase)
        {
            if (data.itemType == type) return data.iconSprite;
        }
        return null;
    }
}

