using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private GameObject card;
    private Catte catte;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Vector3 origin;
    private Vector3 target;
    
    // Start is called before the first frame update
    void Start()
    {
        List<string> deck = Catte.generateDeck();
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
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
        origin = transform.position;
        target = transform.position + new Vector3(0, .3f, 0);
        Debug.Log("YYYYY" + this.name);
        Debug.Log(origin);
    }

    // Update is called once per frame
    void Update()
    {
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
        
        if(selectable != null) {
            if (selectable.faceup == true)
            {
                spriteRenderer.sprite = cardFace;
            }
            else
            {
                spriteRenderer.sprite = cardBack;
            }
            if (selectable.selected == true)
            {
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
            }

            if (selectable.targetScale != Vector3.zero)
            {
                float step = 12.0f * Time.deltaTime;
                transform.localScale = Vector3.Lerp(transform.localScale, selectable.targetScale, step);
            }
        }
        
    }
}
