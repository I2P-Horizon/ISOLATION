using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour, ICycleListener
{
    public int EventNum => _eventNum;
    private int _eventNum;

    private List<IEvent> _eventList;



    private void Awake()
    {
        if (!TimeManager.Instance) return;
        TimeManager.Instance.Register(this);

        _eventList = new List<IEvent>();
        InitEventList();

        _eventNum = Random.Range(0, _eventList.Count);
    }



    public void OnCycleChanged()
    {
        //Day
        if (!TimeManager.Instance.IsNight)
        {
            _eventList[_eventNum].ExecuteDayEvent();
            SetNextNightEventNum();
        }

        //Night
        if (TimeManager.Instance.IsNight)
        {
            _eventList[_eventNum].ExecuteNightEvent();
        }
    }

    private void InitEventList()
    {
        IEvent[] events = GetComponents<IEvent>();
        foreach (IEvent element in events)
        {
            Debug.Log($"Event: {element}");
            _eventList.Add(element);
        }
            
    }

    private void SetNextNightEventNum()
    {
        _eventNum = Random.Range(0, _eventList.Count);
        Debug.Log($"Set RandEventNum{_eventNum}");
    }
}