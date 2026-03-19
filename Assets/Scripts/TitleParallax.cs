using UnityEngine;
using UnityEngine.UI; // RawImageを使うために必要

// インスペクターで「画像」と「速度」をセットで登録できるようにするための構造体
[System.Serializable]
public struct ParallaxLayer
{
    public RawImage image;
    public float speed; // スクロール速度（奥は遅く，手前は速くする）
}

public class TitleParallax : MonoBehaviour
{
    // 5層のレイヤーをインスペクターから登録する配列
    [SerializeField] private ParallaxLayer[] layers;

    void Update()
    {
        // 毎フレーム，全レイヤーのUV座標を少しずつずらしてスクロールさせる
        for (int i = 0; i < layers.Length; i++)
        {
            // 現在のUV座標を取得
            Rect uvRect = layers[i].image.uvRect;
            
            // X軸方向に速度分だけ移動（Time.deltaTimeを掛けてフレームレートに依存しない滑らかな動きにする）
            uvRect.x += layers[i].speed * Time.deltaTime;
            
            // ずらした座標を戻す
            layers[i].image.uvRect = uvRect;
        }
    }
}