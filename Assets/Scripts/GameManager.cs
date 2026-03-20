using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

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
    [Header("Item Data")]
    [SerializeField] private ItemData[] itemDatabase;

    [Header("Objects")]
    [SerializeField] private ItemGenerator itemGenerator;
    [SerializeField] private GameObject[] boxes;
    [SerializeField] private BeltConveyor[] beltConveyors;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Life UI")]
    [SerializeField] private Image[] lifeImages; // 3つのハートのImageを格納する配列
    [SerializeField] private Sprite heartFull;   // 満タンのハート画像
    [SerializeField] private Sprite heartEmpty;  // 空のハート画像

    [Header("正解判定")]
    [SerializeField] private Sprite circleSprite; // マルの画像
    [SerializeField] private Sprite crossSprite;  // バツの画像
    [SerializeField] private AudioClip correctSE;
    [SerializeField] private AudioClip incorrectSE;
    [SerializeField] private float resultDisplayTime = 1.0f; // 表示時間

    [Header("Result画面")]
    [SerializeField] private ResultManager resultManager;

    [Header("ゲームパラメタ")]
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
                box.OnItemProcessed
                    .Subscribe(result => HandleBoxProcessed(box, result.isCorrect, result.score))
                    .AddTo(this); 
            }
        }
        // UI初期化
        UpdateScoreText();
        UpdateLifeUI();

        // ハートが動くアニメーション開始
        StartCoroutine(HeartbeatRoutine());

        // ゲーム開始
        AssignUniqueRequestsToBoxes(); // リクエストは最初から出しておく
        StartCoroutine(StartGameRoutine()); //アイテム生成は遅延
    }
    
    private IEnumerator StartGameRoutine()
    {
        // 1フレーム待ってからbelt conveyorの操作をOnにする(リスタート時の連打防止)
        yield return null;
        foreach (BeltConveyor beltConveyor in beltConveyors)
        {
            beltConveyor.canClick = true; //ベルトのクリック判定をOnに
        }
        yield return new WaitForSeconds(initialWaitTime);
        SpawnRandomItem();
    }

    // Boxからイベントを受け取った時の処理
    private void HandleBoxProcessed(Box box, bool isCorrect, int score)
    {
        if (isCorrect)
        {
            AddScore(score);
            SEManager.Instance.PlaySE(correctSE);
        }
        else
        {
            DecreaseLife();
            SEManager.Instance.PlaySE(incorrectSE);
        }
        StartCoroutine(ShowResultEffect(box, isCorrect));
    }

    private IEnumerator ShowResultEffect(Box targetBox, bool isCorrect)
    {

        // 各Boxの画像を更新
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

        // 待機
        yield return new WaitForSeconds(resultDisplayTime);

        // ゲームオーバーでなければ次をセット
        if (life > 0)
        {
            AssignUniqueRequestsToBoxes();
            SpawnRandomItem();
        }
    }

    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();    
        Debug.Log("スコアアップ！ 現在のスコア: " + score);

        // スコアテキストのアニメーション
        if (scoreText != null)
        {
            // アニメーションが連続で呼ばれた時に巨大化し続けるのを防ぐため，一度リセットする
            scoreText.transform.DOKill(true);
            scoreText.transform.localScale = Vector3.one;

            // ポコン！と跳ねる演出
            scoreText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.3f, 10, 1f);
        }
    }

    private void DecreaseLife()
    {
        life--;
        UpdateLifeUI();
        Debug.Log("ミス！ 残りライフ: " + life);

        if (life <= 0)
        {
            GameOver();
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString("00000");
    }

    private void UpdateLifeUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            // 現在のライフ数よりインデックスが小さければ「満タン」，それ以上なら「空」
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

        scoreText.enabled = false; //スコア表示を消しておく

        //ベルトのクリック判定を停止
        foreach (BeltConveyor beltConveyor in beltConveyors)
        {
            beltConveyor.canClick = false; //ベルトのクリック判定をOnに
        }

        // ResultManagerの ShowResult に，現在の score を渡して演出開始
        if (resultManager != null)
        {
            resultManager.ShowResult(score);
        }
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

    private IEnumerator HeartbeatRoutine()
    {
        while (true) // ゲーム中ずっとループ
        {
            // 3秒〜5秒のランダムな間隔で待機
            yield return new WaitForSeconds(2f);

            // ゲームオーバーじゃなければ実行
            if (life > 0)
            {
                // 現在「満タン」になっているハート（インデックス 0 から life-1 まで）だけを揺らす
                for (int i = 0; i < life; i++)
                {
                    if (lifeImages[i] != null)
                    {
                        RectTransform heartRect = lifeImages[i].rectTransform;
                        
                        // 他のアニメーションと被らないようにリセット
                        heartRect.DOKill(true);
                        heartRect.localScale = Vector3.one;

                        // ドクン！という心臓の鼓動のような蠢き
                        heartRect.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.3f, 2, 0.5f);
                    }
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

