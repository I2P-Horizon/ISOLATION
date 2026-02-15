using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] Image image;

    //private LYG_Item _item;
    //public LYG_Item item
    //{
    //    get { return _item; }
    //    set
    //    {
    //        _item = value;
    //        if (_item != null)
    //        {
    //            image.sprite = item.itemImage;
    //            image.color = new Color(1, 1, 1, 1);
    //        }
    //        else
    //        {
    //            image.color = new Color(1, 1, 1, 0);
    //        }
    //    }
    //}

    public void SetSlot(Sprite sprite, bool isActive)
    {
        if (isActive && sprite != null)
        {
            image.sprite = sprite;
            image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        }
    }
}
