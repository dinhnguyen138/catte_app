using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    public Material[] materials;
    private GameObject card;
    private Catte catte;
    private SpriteRenderer renderer;
    private Selectable selectable;
    private Vector3 origin;
    private Vector3 target;

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
        selectable = GetComponent<Selectable>();
        origin = transform.position;
        target = transform.position + new Vector3(0, .3f, 0);
        Debug.Log("YYYYY" + this.name);
        Debug.Log(origin);
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = selectable.sortingOrder;
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

        if (selectable != null) {
            if (selectable.faceup == true)
            {
                renderer.sprite = cardFace;
            }
            else
            {
                renderer.sprite = cardBack;
            }
            if (selectable.selected == true)
            {
                Debug.Log("source " + origin.ToString() + " target " + target.ToString());
                Debug.Log(transform.position.ToString());
                float step = 1.5f * Time.deltaTime;
                
                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
            if (selectable.deselected == true)
            {
                float step = 1.5f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, origin, step);
            }

            if (selectable.targetPos != Vector3.zero) {
                float step = 10.0f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, selectable.targetPos, step);
                origin = selectable.targetPos;
                target = origin + new Vector3(0, .3f, 0);
                if(transform.position.Equals(selectable.targetPos))
                {
                    Debug.Log("XXXX");
                    selectable.targetPos = Vector3.zero;
                }
            }

            if (selectable.targetScale != Vector3.zero)
            {
                float step = 10.0f * Time.deltaTime;
                transform.localScale = Vector3.Lerp(transform.localScale, selectable.targetScale, step);
            }

            if (selectable.lost == true)
            {
                renderer.material = materials[1];
            }
            else
            {
                renderer.material = materials[0];
            }
        }
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
