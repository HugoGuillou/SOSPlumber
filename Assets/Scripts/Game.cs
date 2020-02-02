using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class Game : MonoBehaviour
{

    [SerializeField]
    private TextAsset csvFile;

    private Dictionary<string, List<string>> Names;
    private List<string> Genders;
    private List<string> Quotes;
    private List<string> imagePaths;
    private List<GameObject> ImagePrefabs;
    private List<Sprite> RepairIcons;

    private Queue<Card> Deck;

    [SerializeField]
    private int deckSize = 20;

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Transform deckPlaceHolder;

    [SerializeField]
    private Transform DialogBubble;

    [SerializeField]
    private float bubblePopSpeed = 1;

    //swipe detection class
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    [SerializeField]
    private GameObject VictoryCard;
    [SerializeField]
    private GameObject NoRepairCard;
    [SerializeField]
    private GameObject OverloadCard;

    [SerializeField]
    private GameObject _MenuUI;
    [SerializeField]
    private GameObject _DoorUI;
    [SerializeField]
    private GameObject _GameUI;

    private bool _IsMenu = true;
    private bool IsMenu
    {
        get { return _IsMenu; }
        set
        {
            if (_IsMenu == value)
                return;
            _MenuUI.SetActive(value);
            _DoorUI.SetActive(value);
            _GameUI.SetActive(!value);
            _IsMenu = value;
        }
    }

    private enum GameState
    {
        DoorClosed,
        DoorOpen,
        Swiping,
        Over
    }
    private GameState gameState;

    private enum Swipe
    {
        Left,
        Right,
        None
    }

    private enum EndState
    {
        Victory,
        Overload,
        NoRepair,
        None
    }
    private EndState endState = EndState.None;

    [SerializeField]
    private Transform DoorCard;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {

        Names = new Dictionary<string, List<string>>();
        Names["M"] = new List<string>();
        Names["F"] = new List<string>();
        Names["NB"] = new List<string>();

        Quotes = new List<string>();
        Genders = new List<string>();
        Genders.Add("M");
        Genders.Add("F");
        //Genders.Add("NB");
        imagePaths = new List<string>();

        GameObject[] resourcesImagePrefabs = Resources.LoadAll<GameObject>("Personnages/Canvas") as GameObject[];
        ImagePrefabs = new List<GameObject>(resourcesImagePrefabs);

        Sprite[] resourceRepairIcons = Resources.LoadAll<Sprite>("RepairIcons") as Sprite[];
        RepairIcons = new List<Sprite>(resourceRepairIcons);

        RetrieveFromCSV(csvFile);

        Deck = GenerateDeck(deckSize);

        /*
        // Prepare our first card
        Card.current = Deck.Dequeue();

        gameState = GameState.DoorOpen;
        */
        Player.OnStatChanged += StatsUpdated;

        AudioManager.PlayLoop(AudioManager.Sounds.MenuMusic);
    }


    public void StatsUpdated(Player.StatType type, float hotness, float oldHotness)
    {
        // SEXYNESS OVERLOAD
        if (type == Player.StatType.Hotness && hotness == 1)
        {
            gameOver = true;
            endState = EndState.Overload;

            GenerateEndCard(endState);

            Debug.Log("Perdu !");


        }
    }

    public void DeckEmpty()
    {
        gameOver = true;

        float totalRepair = Player.ChimneyStat.Value + Player.PlumbingStat.Value + Player.KitchenStat.Value + Player.BoilerStat.Value;

        if (totalRepair == 1)
            endState = EndState.Victory;
        else
            endState = EndState.NoRepair;

        GenerateEndCard(endState);

        Debug.Log("Fin du jeu !");
    }

    // Update is called once per frame
    void Update()
    {

        Swipe swipeDir = GetSwipe();

        if (swipeDir != Swipe.None)
        {
            if (IsMenu)
            {
                AudioManager.PlayLoop(AudioManager.Sounds.GameMusic);
                IsMenu = false;
                NextCard();
            }
            else
            {
                if (swipeDir == Swipe.Left)
                    SwipeLeft();
                else if (swipeDir == Swipe.Right)
                    SwipeRight();
            }
        }

        /*
        if (gameState == GameState.DoorOpen)
        {
            if (swipeDir == Swipe.Left)
                SwipeLeft();
            else if (swipeDir == Swipe.Right)
                SwipeRight();
        }

        else if (gameState == GameState.DoorClosed)
        {
            if (swipeDir != Swipe.None)
            {
                StartCoroutine(DrawCardAnim());
            }
        }
        */

    }

    void GenerateEndCard(EndState end)
    {
        GameObject endCard = new GameObject();
        string endText = "";

        foreach (Card card in Deck)
        {
            Destroy(card.gameObject);
        }

        TextMeshProUGUI textMesh = DialogBubble.GetComponentInChildren<TextMeshProUGUI>();

        switch (end)
        {
            case EndState.Victory:
                endCard = Instantiate(VictoryCard, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
                endText = "Vous avez eu les yeux plus gros que le ventre. Je vous emmène au 7e ciel !";
                break;

            case EndState.NoRepair:
                endCard = Instantiate(NoRepairCard, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
                endText = "On dirait qu'il reste encore du boulot. On dirait bien qu'il va falloir recommencer.";
                break;

            case EndState.Overload:
                endCard = Instantiate(OverloadCard, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
                endText = "Vous avez eu les yeux plus gros que le ventre. Je vous emmène au 7e ciel !";
                break;
            default:
                endText = "Switch Error";
                break;
        }

        endCard.transform.SetAsFirstSibling();
        textMesh.text = endText;
    }

    Queue<Card> GenerateDeck(int size)
    {
        Queue<Card> ourDeck = new Queue<Card>();

        for (int i = 0; i < size; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
            Card ourCard = cardGO.GetComponent<Card>();

            int randGender = Random.Range(0, Genders.Count);
            string gender = Genders[randGender];

            int randName = Random.Range(0, Names[gender].Count);
            string name = Names["M"][randName];

            /*
            int randQuote = Random.Range(0, Quotes.Count);
            string quote = Quotes[randQuote];
            */
            string quote = StatsPool.instance.Quote;

            //int randImage = (int)Random.Range(0, imagePaths.Count - 1);
            //string imagePath = imagePaths[randQuote];
            //Sprite image = Resources.Load(imagePath) as Sprite;
            //Sprite image = Resources.Load<Sprite>("wink");

            int randImage = Random.Range(0, ImagePrefabs.Count);
            GameObject imagePrefab = ImagePrefabs[randImage];

            // A CHANGER QUAND LES MODIFS DANS PLAYER SERONT PUSHÉS //////////////////////////////////
            //float sexyStat = Random.Range(-0.1f, 0.1f);

            float sexyStat = StatsPool.instance.BonusSexAppeal;

            /*
            int chimneyStat = Random.Range(-10, 10);
            int plumbryStat = Random.Range(-10, 10);
            int kitchenStat = Random.Range(-10, 10);
            int boilerStat = Random.Range(-10, 10);
            */

            System.Array statValues = System.Enum.GetValues(typeof(Player.StatType));
            int randHouseType = Random.Range(0, 4);
            Player.StatType houseType = (Player.StatType)statValues.GetValue(randHouseType);
            //float houseStat = Random.Range(-0.1f, 0.1f);
            float houseStat = StatsPool.instance.BonusRepair;

            string repairIconPath = "";

            switch (houseType)
            {
                case Player.StatType.Boiler:
                    repairIconPath = "RepairIcons/Spr_Heater";
                    break;
                case Player.StatType.Kitchen:
                    repairIconPath = "RepairIcons/Spr_Electricity";
                    break;
                case Player.StatType.Plumbing:
                    repairIconPath = "RepairIcons/Spr_Tap";
                    break;
                case Player.StatType.Chimney:
                    repairIconPath = "RepairIcons/Spr_House";
                    break;
            }

            Debug.Log(houseType);
            ///////////////////////////////////////////////////////////////////////////////////////////
            ourCard.SetAllData(name, quote, sexyStat, houseType, houseStat, imagePrefab, repairIconPath);

            ourCard.transform.SetAsFirstSibling();
            ourDeck.Enqueue(ourCard);

            Vector3 newScale = ourCard.transform.localScale;
            newScale.x = 0;
            //ourCard.transform.localScale = newScale;
        }

        return ourDeck;
    }


    void RetrieveFromCSV(TextAsset path)
    {
        List<string> lines = new List<string>(csvFile.text.Split("\n"[0]));
        lines.RemoveAt(lines.Count - 1);
        // Ignore first line of csv (1st is field names)
        lines.RemoveAt(0);

        foreach (string line in lines)
        {
            string[] lineData = line.Split(';');

            string imagePath = lineData[0];
            string name = lineData[1];
            string quote = lineData[2];
            string gender = lineData[4];

            imagePaths.Add(imagePath);
            Names["M"].Add(name);
            Quotes.Add(quote);

        }

    }

    //https://forum.unity.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    private Swipe GetSwipe()
    {
        if (gameState == GameState.Swiping || gameOver)
            return Swipe.None;

        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            /*
            //swipe upwards
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
        {
                Debug.Log("up swipe");
            }
            //swipe down
            if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
        {
                Debug.Log("down swipe");
            }
            */

            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                return Swipe.Left;
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                return Swipe.Right;

            }
        }

        return Swipe.None;
    }

    void SwipeRight()
    {
        gameState = GameState.Swiping;
        if (Card.current != null)
            StartCoroutine(sweepRight(Card.current.transform));
    }

    void SwipeLeft()
    {
        gameState = GameState.Swiping;
        if (Card.current != null)
            StartCoroutine(sweepLeft(Card.current.transform));
    }

    void NextCard()
    {
        if (endState != EndState.None)
            return;

        if (Deck.Count == 0)
        {
            DeckEmpty();
            return;
        }

        Card.current = Deck.Dequeue();

        gameState = GameState.DoorOpen;

        // Pop Dialog Bubble
        //DialogBubble.DOScale(1, 1);
        StartCoroutine(PopBubble());

        //Set Quote Text
        TextMeshProUGUI textMesh = DialogBubble.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = Card.current.GetQuote();

        AudioManager.PlaySingleShot(AudioManager.Sounds.DingDongSound, () =>
        {
            AudioManager.PlaySingleShot(AudioManager.Sounds.TextSound);
            AudioManager.PlaySingleShot(AudioManager.Sounds.NewCharSound);
        });
    }

    IEnumerator DrawCardAnim()
    {

        Vector3 doorScale = DoorCard.localScale;
        while (DoorCard.localScale.x > 0)
        {
            doorScale.x -= 5 * Time.deltaTime;
            DoorCard.localScale = doorScale;
            yield return new WaitForEndOfFrame();
        }

        DoorCard.gameObject.SetActive(false);

        Vector3 currentCardScale = Card.current.transform.localScale;
        while (Card.current.transform.localScale.x < 1)
        {
            currentCardScale.x += 5 * Time.deltaTime;
            Card.current.transform.localScale = currentCardScale;
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator PopBubble()
    {
        while (DialogBubble.localScale.x < 1)
        {
            DialogBubble.localScale += Vector3.one * bubblePopSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DepopBubble()
    {
        while (DialogBubble.localScale.x > 0)
        {
            DialogBubble.localScale -= Vector3.one * bubblePopSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator sweepRight(Transform tr)
    {
        Card.current.AcceptCard();

        while (tr.localPosition.x <= 600)
        {
            tr.Translate(Vector3.right * 500f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextCard();
        Debug.Log("Card gone");
    }

    IEnumerator sweepLeft(Transform tr)
    {
        Card.current.DisCard();

        while (tr.localPosition.x >= -600)
        {
            tr.Translate(Vector3.left * 500f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextCard();
        Debug.Log("Card gone");
    }
}
