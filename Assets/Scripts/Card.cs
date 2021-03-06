﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public delegate void CardEvent(Card card);

    public static CardEvent OnCurrentCardChanged;
    public static CardEvent OnCardDiscarded;
    public static CardEvent OnCardAccepted;

    private static Card _current;
    public static Card current
    {
        get { return _current; }
        set
        {
            if (_current == value)
                return;
            _current = value;
            if (value != null)
                OnCurrentCardChanged?.Invoke(_current);
        }
    }

    [SerializeField]
    private string quote;

    [SerializeField]
    private new string name;

    [SerializeField]
    private float sexyStat;
    public float SexyStat => sexyStat;

    [SerializeField]
    private Player.StatType houseType;
    public Player.StatType HouseType => houseType;

    [SerializeField]
    private float houseStat;
    public float HouseStat => houseStat;

    [SerializeField]
    private GameObject imagePrefab;

    [SerializeField]
    private Transform imageParent;

    [SerializeField]
    private TextMeshProUGUI nameComponent;

    [SerializeField]
    private TextMeshProUGUI hotnessComponent;

    [SerializeField]
    private TextMeshProUGUI repairComponent;

    [SerializeField]
    private Image repairIcon;

    public bool isDoorCard = false;

    [SerializeField]
    private RectTransform rectoCard;

    [SerializeField]
    private RectTransform versoCard;

    // Start is called before the first frame update
    void Start()
    {

        // Card start on verso

        rectoCard.gameObject.SetActive(false);
        versoCard.gameObject.SetActive(true);

        //transform.rotation = Quaternion.Euler(0, 180, 0);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcceptCard()
    {
        OnCardAccepted?.Invoke(this);
       // AudioManager.PlaySingleShot(AudioManager.Sounds.AcceptSound);
    }

    public void DisCard()
    {
        OnCardDiscarded?.Invoke(this);
        //AudioManager.PlaySingleShot(AudioManager.Sounds.DeclineSound);
    }

    public string GetQuote()
    {
        return quote;
    }

    public void FlipTexture()
    {
        rectoCard.gameObject.SetActive(true);
        versoCard.gameObject.SetActive(false);
    }

    public void SetAllData(string n, string q, int s, int cs, int ps, int ks, int bs, GameObject ip)
    {
        name = n;
        quote = q;
        sexyStat = s;
        //chimneyStat = cs;
        //plumbryStat = ps;
        //kitchenStat = ks;
        //boilerStat  = bs;
        //sprite = i;

        // Set image
        //Image cardImage = transform.Find("InCard/CharacterSprite").GetComponent<Image>();
        imagePrefab = Instantiate(ip, imageParent);

        //Set name
        //Text cardName = transform.Find("Background/CharacterName").GetComponent<Text>();
        nameComponent.text = name;

        //Set hotness points
        String sexyStr = "";
        if (sexyStat > 0)
            sexyStr += "+";
        sexyStr += sexyStat.ToString();
        //TextMesh cardHotness = transform.Find("StatsContainer/HotnessContener/HotnesIcon/HotnessPoint").GetComponent<TextMesh>();
        hotnessComponent.text = sexyStr;

        //Set repair points
        String repairStr = "";
        if (houseStat > 0f)
            repairStr += "+";
        repairStr += ((int)houseStat).ToString();
        //TextMesh cardRepair = transform.Find("StatsContainer/RepairContener/RepairIcon/RepairPoint").GetComponent<TextMesh>();
        repairComponent.text = repairStr;
    }

    public void SetAllData(string n, string q, float hotness, Player.StatType type, float stat, GameObject ip, string repairIconPath)
    {
        name = n;
        quote = q;
        sexyStat = hotness;
        houseType = type;
        houseStat = stat;
        //chimneyStat = cs;
        //plumbryStat = ps;
        //kitchenStat = ks;
        //boilerStat  = bs;
        //sprite = i;

        // Set image
        //Image cardImage = transform.Find("InCard/CharacterSprite").GetComponent<Image>();
        imagePrefab = Instantiate(ip, imageParent);

        //Set name
        //Text cardName = transform.Find("Background/CharacterName").GetComponent<Text>();
        nameComponent.text = name;

        //Set hotness points
        String sexyStr = "";
        if (sexyStat > 0)
            sexyStr += "+";
        sexyStr += ((int)(sexyStat * StatsPool.MaxSexAppeal)).ToString();
        //TextMesh cardHotness = transform.Find("StatsContainer/HotnessContener/HotnesIcon/HotnessPoint").GetComponent<TextMesh>();
        hotnessComponent.text = sexyStr;

        //Set repair points
        String repairStr = "";
        if (houseStat > 0f)
            repairStr += "+";
        repairStr += ((int)(houseStat * StatsPool.MaxRepair)).ToString();
        //TextMesh cardRepair = transform.Find("StatsContainer/RepairContener/RepairIcon/RepairPoint").GetComponent<TextMesh>();
        repairComponent.text = repairStr;

        repairIcon.sprite = Resources.Load<Sprite>(repairIconPath);
    }
}
