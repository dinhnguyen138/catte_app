using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catte : MonoBehaviour
{
    private enum State {
        Idle,
        Ready,
        Joint,
        Cards,
        Play,
    }
    private static string uuid = "1";// System.Guid.NewGuid().ToString();

    public Sprite[] cards;
    public GameObject playerCard;
    public GameObject playerView;
    public GameObject[] otherPlayerView;
    public Button startButton;
    public Button foldButton;
    public Button playButton;

    private Player player;
    private List<Player> otherPlayers;
    private List<Play> plays;

    public static string[] suits = { "C", "D", "H", "S" };
    public static string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    private State currentState;
    
    // Start is called before the first frame update
    void Awake()
    {
        currentState = State.Ready;
        player = null;
        otherPlayers = new List<Player>();
        plays = new List<Play>();
        GameClient.Init();
        GameClient.OnConnectEvent += OnConnect;
        GameClient.OnJoinEvent += OnJoin;
        GameClient.OnCardsEvent += OnCards;
        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(StartGame);
        playButton.gameObject.SetActive(false);
        playButton.onClick.AddListener(PlayCard);
        foldButton.gameObject.SetActive(false);
        foldButton.onClick.AddListener(FoldCard);
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Play) {
            RenderPlays();
        }
        if (currentState == State.Cards)
        {
            if (player != null && player.cards != null)
            {
                RenderCards();
            }
        }

        currentState = State.Idle;
    }

    public void OnApplicationQuit()
    {
        GameClient.OnConnectEvent -= OnConnect;
        GameClient.OnJoinEvent -= OnJoin;
        GameClient.OnCardsEvent -= OnCards;
        GameClient.Disconnect();

        startButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        foldButton.onClick.RemoveAllListeners();
    }

    public void RenderCards() {
        float xOffset;
        float zOffset;

        xOffset = -2.5f;
        zOffset = 0.03f;
        for (int i = 0; i < player.cards.Count; i++)
        {
            Debug.Log(player.cards[i]);
            GameObject newCard = Instantiate(playerCard, new Vector3(playerView.transform.position.x + xOffset, playerView.transform.position.y, playerView.transform.position.z - zOffset), Quaternion.identity);
            newCard.name = player.cards[i];
            xOffset += 1.3f;
            zOffset += 0.03f;
            Debug.Log(transform.position.x);
        }
        //for (int j = 0; j < otherPlayers.Count; j++)
        //{
        //    xOffset = -0.7f;
        //    xOffset = 0.03f;
        //    GameObject newCard = Instantiate(playerCard, new Vector3(otherPlayerView[j].transform.position.x + xOffset, otherPlayerView[j].transform.position.y, otherPlayerView[j].transform.position.z - zOffset), Quaternion.identity);
        //    xOffset += 0.3f;
        //    zOffset += 0.03f;
        //}
    }

    public void RenderPlays() {
        foreach(var p in plays)
        {
            if(p.userid == uuid)
            {
                player.cards.Remove(p.card);
                GameObject obj = GameObject.Find(p.card);
                Selectable selectable = obj.GetComponent<Selectable>();
                if (p.action == GameClient.FOLD)
                {
                    selectable.faceup = false;
                }
                float xOffset = -0.7f + (0.3f * (5 - player.cards.Count));
                float zOffset = 0.03f + (0.03f * (5 - player.cards.Count));
                selectable.targetPos = new Vector3(otherPlayerView[0].transform.position.x + xOffset, otherPlayerView[0].transform.position.y, otherPlayerView[0].transform.position.z - zOffset);
                Debug.Log("XXXXXX");
                Debug.Log(selectable.targetPos);
                Debug.Log(obj.transform.position);
                selectable.targetScale = new Vector3(obj.transform.localScale.x * 0.8f, obj.transform.localScale.y * 0.8f, obj.transform.localScale.z);
            }
        }
        plays.Clear();
    }

    public void StartGame() {
        startButton.gameObject.SetActive(false);
        Debug.Log("StartGame");
        string msg = "{\"action\": \"DEAL\", \"room\":\"1\", \"id\":\"" + uuid + "\", \"data\":\"xxx\"}\n";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
        GameClient.Send(data);
    }

    public void OnConnect()
    {
        Debug.Log("Server Connected");
        string msg = "{\"action\": \"JOIN\", \"room\":\"1\", \"id\":\"" + uuid + "\", \"data\":\"\"}\n";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
        GameClient.Send(data);
    }

    public void OnJoin(List<Player> players) {
        otherPlayers.Clear();
        foreach(var p in players) {
            if (p == null) {
                continue;
            }
            if (p.id == uuid)
            {
                Debug.Log("JOINT");
                player = p;
            }
            else {
                otherPlayers.Add(p);
            }
        }
    }

    public void OnCards(List<string> cards) {
        player.cards = cards;
        currentState = State.Cards;
    }

    public static List<string> generateDeck() {
        List<string> deck = new List<string>();
        
        foreach (string v in values) {
            foreach (string s in suits)
            {
                deck.Add(v + s);
            }
        }

        return deck;
    }

    public void OnClickCard(string cardname) {
        bool cardSelected = false;
        foreach (var p in player.cards) {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (p != cardname)
            {
                if (s.selected == true) {
                    s.selected = false;
                    s.deselected = true;
                }
            }
            else
            {
                if (s.selected == true)
                {
                    s.selected = false;
                    s.deselected = true;
                }
                else
                {
                    s.deselected = false;
                    s.selected = true;
                    cardSelected = true;
                }
            }
        }

        if (cardSelected == true) {
            playButton.gameObject.SetActive(true);
            foldButton.gameObject.SetActive(true);
        }
    }

    public void PlayCard() {
        foreach (var p in player.cards)
        {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (s.selected == true)
            {
                s.selected = false;
                Debug.Log("Select Card Play");
                Play play = new Play();
                play.action = GameClient.PLAY;
                play.userid = uuid;
                play.card = p;
                plays.Add(play);
            }
        }
        currentState = State.Play;
    }

    public void FoldCard()
    {
        foreach (var p in player.cards)
        {
            GameObject obj = GameObject.Find(p);
            Selectable s = obj.GetComponent<Selectable>();
            if (s.selected == true)
            {
                s.selected = false;
                Play play = new Play();
                play.action = GameClient.FOLD;
                play.userid = uuid;
                play.card = p;
                plays.Add(play);
            }
        }
        currentState = State.Play;
    }

    public void onBeginTurn(int index, bool newRound) {
        // Start timer on index

    }

    public void onShowCard(int index, string card) {

    }
}
