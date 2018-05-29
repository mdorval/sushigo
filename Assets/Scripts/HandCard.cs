﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HandCard : MonoBehaviour {
    private CardType mycard = CardType.Null;
    public HumanPlayer player;
    public GameObject toolTip;
    private EventTrigger myEventTrigger;

    public void Start()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        myEventTrigger = GetComponentInChildren<EventTrigger>();
        myEventTrigger.triggers.Add(entry);
        toolTip.SetActive(false);
    }

    public void ApplyCard(CardInfo info)
    {
        Image image = GetComponent<Image>();
        image.sprite = info.cardSprite;
        GetComponentInChildren<Text>().text = info.tooltipText;
        mycard = info.type;
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        player.PlayCard(this.mycard);
    }
}
