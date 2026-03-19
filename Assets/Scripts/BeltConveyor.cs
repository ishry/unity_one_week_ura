using UnityEngine;
using UniRx;

public class BeltConveyor : MonoBehaviour
{
    [Header("構成パーツ")]
    [SerializeField] private Belt belt;
    [SerializeField] private GearFlipper gearFlipper_L;
    [SerializeField] private GearFlipper gearFlipper_R;
    [SerializeField] private ConveyorArrow conveyorArrow; 

    [Header("パラメタ")]
    [SerializeField] private float scrollSpeed = 1.0f; 
    [SerializeField] private float incrementSpeed = 0.1f;
    [SerializeField] private float maxSpeed = 3.0f;

    [Header("SE")]
    [SerializeField] private AudioClip clickSE;

    [HideInInspector] public bool canClick;
    public static ReactiveProperty<float> SharedSpeed;

    private bool isReversed = false;

    void Awake()
    {
        if (SharedSpeed == null) SharedSpeed = new ReactiveProperty<float>(scrollSpeed);
        else SharedSpeed.Value = scrollSpeed;
    }

    void Start()
    {
        canClick = false; 

        // 全体の速度が変わったら、配下の全パーツに「速度更新」を命令する
        SharedSpeed.Subscribe(UpdateComponentsSpeed).AddTo(this);
    }

    // 入力処理（これをBeltのクリックイベントから呼ぶ）
    public void OnClicked()
    {
        if (!canClick) return;
        if (gearFlipper_L.IsFlipping || gearFlipper_R.IsFlipping) return;

        SEManager.Instance.PlaySE(clickSE);

        // 1. 全体のスピードを上げる（これで全員の Subscribe が発火）
        SharedSpeed.Value = Mathf.Min(SharedSpeed.Value + incrementSpeed, maxSpeed); //速度上限あり

        // 2. 自分の向きを反転フラグを更新
        isReversed = !isReversed;

        // 3. 構成パーツを反転
        gearFlipper_L.FlipGear();
        gearFlipper_R.FlipGear();
        conveyorArrow.ReverseArrow(SharedSpeed.Value);
        belt.SetDirection(isReversed);
    }

    // 速度の更新（Subscribeから呼ばれる）
    private void UpdateComponentsSpeed(float newSpeed)
    {
        gearFlipper_L.speedMagnification = newSpeed;
        gearFlipper_R.speedMagnification = newSpeed;
        conveyorArrow.UpdateSpeed(newSpeed);
        belt.SetSpeed(newSpeed);
    }
}