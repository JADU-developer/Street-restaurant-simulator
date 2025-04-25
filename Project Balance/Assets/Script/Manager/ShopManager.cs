using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{   

    [Header("FoodCounter")]
    [SerializeField] private FoodContainer BurgerCounter;
    [SerializeField] private FoodContainer BlueJuiceCounter;
    [SerializeField] private FoodContainer RedJuiceCounter;
    [SerializeField] private FoodContainer YellowJuiceCounter;
    [SerializeField] private FoodContainer RedDipsCounter;
    [SerializeField] private FoodContainer WhightDipsCounter;
    [SerializeField] private FoodContainer CoffeCupCounter;
    [SerializeField] private FoodContainer FrenchFriesCounter;
    [SerializeField] private FoodContainer HotKogCounter;
    [SerializeField] private FoodContainer CrossiantCounter;
    [SerializeField] private FoodContainer TacoCounter;


    [Header("Button")]
    [SerializeField] private Button BurgerButton;
    [SerializeField] private Button BlueJuiceButton;
    [SerializeField] private Button RedJuiceButton;
    [SerializeField] private Button YellowJuiceButton;
    [SerializeField] private Button RedDipsButton;
    [SerializeField] private Button WhightDipsButton;
    [SerializeField] private Button CoffeCupButton;
    [SerializeField] private Button FrenchFriesButton;
    [SerializeField] private Button HotKogButton;
    [SerializeField] private Button CrossiantButton;
    [SerializeField] private Button TacoButton;


    private void Awake() 
    {
        AddLisner(BurgerButton , BurgerCounter);
        AddLisner(BlueJuiceButton , BlueJuiceCounter);
        AddLisner(RedJuiceButton , RedJuiceCounter);
        AddLisner(YellowJuiceButton , YellowJuiceCounter);
        AddLisner(RedDipsButton , RedDipsCounter);
        AddLisner(WhightDipsButton , WhightDipsCounter);
        AddLisner(CoffeCupButton , CoffeCupCounter);
        AddLisner(FrenchFriesButton , FrenchFriesCounter);
        AddLisner(HotKogButton , HotKogCounter);
        AddLisner(CrossiantButton , CrossiantCounter);
        AddLisner(TacoButton , TacoCounter);
    }

    private void AddLisner(Button button , FoodContainer container)
    {
        button.onClick.AddListener(() =>
        {
            container.IncreaseFoodStockBu10();
        });
    }
}