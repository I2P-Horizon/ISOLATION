using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot1 : MonoBehaviour
{
    //public LYG_Inventory inventory;
    //public List<LYG_Item> _item;

    [SerializeField] private int targetItemID; // **SMJ**

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void OnClickButton()
    {
        //LYG_Item item = _item[0];
        //Debug.Log($"{item.itemName}");
        //inventory.AddItem(item);

        if (StoneBoard3D.Instance != null)
        {
            StoneBoard3D.Instance.TryInsertStone(targetItemID);
        }
    }
}
