using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]
    private TextAsset csvFile;

    private Dictionary<string, List<string>> Names;
    private List<string> Genders;
    private List<string> Quotes;
    private List<string> imagePaths;

    private Queue<Card> Deck;
    private Card currentCard;

    [SerializeField]
    private int deckSize = 20;

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Transform deckPlaceHolder;

    //swipe detection class
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    // Start is called before the first frame update
    void Start()
    {

        Names = new Dictionary<string, List<string>>();
        Names["M"] = new List<string>();
        Names["F"] = new List<string>();
        Names["NB"] = new List<string>();

        Quotes = new List<string>();
        Genders = new List<string>();
        imagePaths = new List<string>();

        RetrieveFromCSV(csvFile);
        Deck = GenerateDeck(deckSize);

        // Prepare our first card
        currentCard = Deck.Dequeue();
        currentCard.GetComponent<SpriteRenderer>().sortingOrder = 1;

    }

    // Update is called once per frame
    void Update()
    {

        Swipe();

    }


    Queue<Card> GenerateDeck(int size)
    {
        Queue<Card> ourDeck = new Queue<Card>();

        for(int i=0; i<size; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, deckPlaceHolder.position, Quaternion.identity, deckPlaceHolder);
            Card ourCard = cardGO.GetComponent<Card>();
            ourCard.GetComponent<BoxCollider2D>().enabled = false;
            ourDeck.Enqueue(ourCard);
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

            imagePaths.Add(imagePath);
            Names[gender].Add(gender);
            Quotes.Add(quote);

        }

    }

    //https://forum.unity.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    public void Swipe()
    {
        if (!currentCard)
            return;

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
                SwipeLeft();
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                SwipeRight();
            }
        }
    }

    void SwipeRight()
    {
        StartCoroutine(sweepRight(currentCard.transform));
        currentCard = null;
    }

    void SwipeLeft()
    {
        StartCoroutine(sweepLeft(currentCard.transform));
        currentCard = null;
    }

    void NextCard()
    {
        currentCard = Deck.Dequeue();
        currentCard.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    IEnumerator sweepRight(Transform tr)
    {
        while (tr.localPosition.x <= 600)
        {
            tr.Translate(Vector3.right * 80f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextCard();
        Debug.Log("Card gone"); 
    }

    IEnumerator sweepLeft(Transform tr)
    {
        while (tr.localPosition.x >= -600)
        {
            tr.Translate(Vector3.left * 80f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextCard();
        Debug.Log("Card gone");
    }
}
