using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot1 : MonoBehaviour
{
    public LYG_Inventory inventory;
    public List<Item> _item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButton()
    {
        Item item = _item[0];
        Debug.Log($"{item.itemName}");
        inventory.AddItem(item);
    }
}
