using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] boxes = new GameObject[3];
    [SerializeField] private Sprite[] sprites = new Sprite[4];
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    } 

    // ランダムなItemTypeを返す関数
    public ItemType GetRandomItemType()
    {
        ItemType[] values = (ItemType[])System.Enum.GetValues(typeof(ItemType));
        int randomIndex = Random.Range(0, values.Length);
        return values[randomIndex];
    } 
}
