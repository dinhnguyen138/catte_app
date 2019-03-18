using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    public Material[] materials;
    private GameObject card;
    private Catte catte;
    private SpriteRenderer renderer;
    private Vector3 origin;
    private Vector3 target;
    public bool selected = false;
    public bool deselected = false;
    public bool faceup = false;
    public bool lost = false;
    public int sortingOrder = 1;
    public Vector3 targetPos = Vector3.zero;
    public Vector3 targetScale = Vector3.zero;

    public static string[] suits = { "C", "D", "H", "S" };
    public static string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    // Start is called before the first frame update
    void Start()
    {
        List<string> deck = generateDeck();
        catte = FindObjectOfType<Catte>();
        int i = 0;
        foreach (string card in deck) {
            if (this.name == card) {
                cardFace = catte.cards[i];
                break;
            }
            i++;
        }
        
        if(this.name != null)
        {
            card = GameObject.Find(this.name);
        }
        
        renderer = GetComponent<SpriteRenderer>();
        origin = transform.position;
        target = transform.position + new Vector3(0, .3f, 0);
        Debug.Log("YYYYY" + this.name);
        Debug.Log(origin);
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = sortingOrder;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BoxCollider2D col = card.GetComponent<BoxCollider2D>();
            if (col != null && col.OverlapPoint(wp))
            {
                Debug.Log("OnClickCard" + this.name);
                catte.OnClickCard(this.name);
            }
        }

        if (faceup == true)
        {
            renderer.sprite = cardFace;
        }
        else
        {
            renderer.sprite = cardBack;
        }
        if (selected == true)
        {
            Debug.Log("source " + origin.ToString() + " target " + target.ToString());
            Debug.Log(transform.position.ToString());
            float step = 1.5f * Time.deltaTime;
                
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
        if (deselected == true)
        {
            float step = 1.5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, origin, step);
        }

        if (targetPos != Vector3.zero) {
            float step = 10.0f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            origin = targetPos;
            target = origin + new Vector3(0, .3f, 0);
            if(transform.position.Equals(targetPos))
            {
                Debug.Log("XXXX");
                targetPos = Vector3.zero;
            }
        }

        if (targetScale != Vector3.zero)
        {
            float step = 10.0f * Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, step);
            if (transform.localScale.Equals(targetScale))
            {
                targetScale = Vector3.zero;
            }
        }

        if (lost == true)
        {
            renderer.material = materials[1];
        }
        else
        {
            renderer.material = materials[0];
        }
    }

    public void Action(string action, int row, int order, Vector3 position, Vector3 scale)
    {
        if (row < 4)
        {
            if (action == MessageHandler.PLAY)
            {
                faceup = true;
            }
            else
            {
                faceup = false;
                lost = true;
            }
        }
        else
        {
            faceup = true;
        }
        
        sortingOrder = order;
        targetPos = position;
        targetScale = scale;
    }

    public void SetInit(Vector3 position, Vector3 scale)
    {
        this.transform.position = position;
        this.transform.localScale = scale;
    }

    public static List<string> generateDeck()
    {
        List<string> deck = new List<string>();

        foreach (string v in values)
        {
            foreach (string s in suits)
            {
                deck.Add(v + s);
            }
        }

        return deck;
    }
}
