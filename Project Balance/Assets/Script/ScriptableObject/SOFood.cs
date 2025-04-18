using UnityEngine;

[CreateAssetMenu(fileName = "NewFood", menuName = "Food")]
public class SOFood : ScriptableObject
{
   
    public string foodName;
    public GameObject FoodPrefab;
    public Sprite foodImage;
    public int price;
    
    public SOFood(string name, int price, Sprite image, GameObject prefab)
    {
        foodName = name;
        this.price = price;
        foodImage = image;
        FoodPrefab = prefab;
    }
}
