using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EventManager : MonoBehaviour, ICycleListener
{
    public static EventManager Instance { get; private set; }
    public int EventNum => _eventNum;
    private int _eventNum;

    private List<IEvent> _eventList;

    private bool _dirty = false;

    public static event Action OnNextEventNumberSet;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (!TimeManager.Instance) return;
        TimeManager.Instance.Register(this);

        _eventList = new List<IEvent>();
        InitEventList();

        _eventNum = UnityEngine.Random.Range(0, _eventList.Count);
    }



    public bool IsDirty() { return _dirty; }

    public void SetDirty(bool dirty = false)
    {
        _dirty = dirty;
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
        _eventNum = UnityEngine.Random.Range(0, _eventList.Count);
        Debug.Log($"Set RandEventNum{_eventNum}");

        OnNextEventNumberSet?.Invoke();
        SetDirty(true);
    }
}