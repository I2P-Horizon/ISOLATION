using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class energy : MonoBehaviour
{
    [SerializeField] private GameObject _textsuccess;
    //public List<LYG_Item> neededpiece;
    //public LYG_Inventory inventory;
    Button button;

    //public bool interaction;

    public bool isFilling = false;

    private EnergyBar _energyBar;

    public static Action OnEnergy;

    // Start is called before the first frame update
    void Start()
    {
        _energyBar = FindAnyObjectByType<EnergyBar>();
        _textsuccess.gameObject.SetActive(false);
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

        //if (interaction) return;

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
        isFilling = true;
        _textsuccess.GetComponent<UIAnimator>().Show();
        LYG_Inventory.Instance.ResetBoard();
        button.interactable = false;
        _energyBar.slider.value = _energyBar.slider.maxValue;
        Invoke("Textoff", 1.5f);
        OnEnergy?.Invoke();
    }

    public void Textoff()
    {
        _textsuccess.GetComponent<UIAnimator>().Close();
        isFilling = false;
    }

}
