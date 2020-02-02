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

    private Queue<Card> Deck;
    private Card currentCard;

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

    private enum GameState{
        DoorClosed,
        DoorOpen
    }

    private enum Swipe
    {
        Left,
        Right,
        None
    }
    private GameState gameState;

    [SerializeField]
    private Transform DoorCard;

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
        Debug.Log(ImagePrefabs[1]);

        RetrieveFromCSV(csvFile);
          
        Deck = GenerateDeck(deckSize);

        // Prepare our first card
        currentCard = Deck.Dequeue();

        gameState = GameState.DoorClosed;
    }

    // Update is called once per frame
    void Update()
    {

        Swipe swipeDir = GetSwipe();

        if(gameState == GameState.DoorOpen)
        {
            if (swipeDir == Swipe.Left)
                SwipeLeft();
            else if (swipeDir == Swipe.Right)
                SwipeRight();
        }

        else if(gameState == GameState.DoorClosed)
        {
            if(swipeDir != Swipe.None)
            {
                StartCoroutine(DrawCardAnim());
            }
        }

    }

    Queue<Card> GenerateDeck(int size)
    {
        Queue<Card> ourDeck = new Queue<Card>();

        for(int i=0; i<size; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
            Card ourCard = cardGO.GetComponent<Card>();

            int randGender = Random.Range(0, Genders.Count);
            string gender = Genders[randGender];

            int randName = Random.Range(0, Names[gender].Count);
            Debug.Log(randName+ " " + Names[gender].Count);
            string name = Names[gender][randName];

            int randQuote = Random.Range(0, Quotes.Count);
            Debug.Log(randQuote);
            string quote = Quotes[randQuote];

            //int randImage = (int)Random.Range(0, imagePaths.Count - 1);
            //string imagePath = imagePaths[randQuote];
            //Sprite image = Resources.Load(imagePath) as Sprite;
            //Sprite image = Resources.Load<Sprite>("wink");

            int randImage = Random.Range(0, ImagePrefabs.Count);
            GameObject imagePrefab = ImagePrefabs[randImage];

            // A CHANGER QUAND LES MODIFS DANS PLAYER SERONT PUSHÉS //////////////////////////////////
            int sexyStat    = Random.Range(-10, 10);
            int chimneyStat = Random.Range(-10, 10);
            int plumbryStat = Random.Range(-10, 10);
            int kitchenStat = Random.Range(-10, 10);
            int boilerStat  = Random.Range(-10, 10);
            ///////////////////////////////////////////////////////////////////////////////////////////
            ourCard.SetAllData(name, quote, sexyStat, chimneyStat, plumbryStat, kitchenStat, boilerStat, imagePrefab);

            ourCard.transform.SetAsFirstSibling();
            ourDeck.Enqueue(ourCard);

            Vector3 newScale = ourCard.transform.localScale;
            newScale.x = 0;
            ourCard.transform.localScale = newScale;
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
            string name      = lineData[1];
            string quote     = lineData[2];
            string gender    = lineData[3];

            string debugLine = imagePath + ", " + name + ", " + quote + ", " + gender;

            Debug.Log(debugLine);

            imagePaths.Add(imagePath);
            Names[gender].Add(name);
            Quotes.Add(quote);

        }

    }

    //https://forum.unity.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    private Swipe GetSwipe()
    {

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
        StartCoroutine(sweepRight(currentCard.transform));
        //currentCard = null;
    }

    void SwipeLeft()
    {
        StartCoroutine(sweepLeft(currentCard.transform));
        //currentCard = null;

    }

    void NextCard()
    {
        if(Deck.Count > 0)
            currentCard = Deck.Dequeue();

        gameState = GameState.DoorClosed;

        // Pop Dialog Bubble
        //DialogBubble.DOScale(1, 1);
        StartCoroutine(PopBubble());

        //Set Quote Text
        TextMeshProUGUI textMesh = DialogBubble.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = currentCard.GetQuote();

    }

    IEnumerator DrawCardAnim()
    {

        Vector3 doorScale = DoorCard.localScale;
        while (DoorCard.localScale.x > 0)
        {
            doorScale.x -= 1 * Time.deltaTime;
            DoorCard.localScale = doorScale;
            yield return new WaitForEndOfFrame();
        }

        DoorCard.gameObject.SetActive(false);

        Vector3 currentCardScale = currentCard.transform.localScale;
        while (currentCard.transform.localScale.x < 1)
        {
            currentCardScale.x += 1 * Time.deltaTime;
            currentCard.transform.localScale = currentCardScale;
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator PopBubble()
    {
        while(DialogBubble.localScale.x < 1)
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
        while (tr.localPosition.x >= -600)
        {
            tr.Translate(Vector3.left * 500f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextCard();
        Debug.Log("Card gone");
    }
}
