using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class energy : MonoBehaviour
{
    public TextMeshProUGUI textsuccess;
    //public List<LYG_Item> neededpiece;
    //public LYG_Inventory inventory;
    Button button;

    public bool interaction;
   

    // Start is called before the first frame update
    void Start()
    {
        textsuccess.gameObject.SetActive(false);
        button = GetComponent<Button>();
        button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        //int count = 0;
        //int[] matching = new int[5];
        //int i = 0;
        //for(; i < inventory.items.Count && i < inventory.Slots.Length; i++)
        //{
        //    for (int j = 0; j < neededpiece.Count; j++)
        //    {
        //        if (inventory.Slots[i].item == neededpiece[j])
        //        {
        //            if (matching[j] == 1) break;

        //            matching[j] = 1;
        //        }
        //    }
        //}
        //for (int k = 0; k < matching.Length; k++)
        //{
        //    if (matching[k] == 1) count++;
        //}

        //if (count == 5)
        //{
        //    button.interactable = true;
        //}

        if (interaction) return;

        if (LYG_Inventory.Instance.IsBoardFull())
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void OnClickButton()
    {
        textsuccess.gameObject.SetActive(true);
        textsuccess.text = "Energy charge success";
        LYG_Inventory.Instance.ResetBoard();
        Invoke("Textoff", 2f);
        button.interactable = false;
        interaction = true;
    }

    public void Textoff()
    {
        textsuccess.gameObject.SetActive(false);
    }

}
