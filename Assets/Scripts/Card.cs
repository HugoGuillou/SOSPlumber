using System;
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

    [SerializeField]
    private Player.StatType houseType;

    [SerializeField]
    private float houseStat;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcceptCard()
    {
        OnCardAccepted(this);
    }

    public void DisCard()
    {
        OnCardDiscarded(this);
    }

    public string GetQuote()
    {
        return quote;
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

    public void SetAllData(string n, string q, float hotness, Player.StatType type, float stat, GameObject ip)
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
}
