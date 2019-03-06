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
        New,
        Leave,
        Cards,
        Play,
    }

    private const int MAXPLAYER = 6;
    private const int MAXCARD = 6;

    // Replace with real data later
    private string userId = System.Guid.NewGuid().ToString();
    private string roomId = "1";
    private PlayerInfo playerInfo = new PlayerInfo();
    private bool inGame = false;
    private int leavePlayerIndex;

    public Sprite[] cards;
    public GameObject playerCard;
    public GameObject userInfo;
    public GameObject playerView;
    public GameObject[] playView;
    public GameObject[] otherCard;
    public GameObject[] playerInfos;
    public Button startButton;
    public Button foldButton;
    public Button playButton;

    private Player player;
    private List<Player> otherPlayers;
    private List<Play> plays;

    public static string[] suits = { "C", "D", "H", "S" };
    public static string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    private State currentState;
    private int currentRow;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInfo.userId = userId;
        playerInfo.userName = System.Guid.NewGuid().ToString();
        playerInfo.image = "";
        playerInfo.amount = 1000000;
        currentState = State.Ready;
        player = null;
        otherPlayers = new List<Player>();
        plays = new List<Play>();
        GameClient.Init();
        GameClient.OnConnectEvent += OnConnect;
        MessageHandler.Init(roomId, userId);
        MessageHandler.OnJoinEvent += OnJoin;
        MessageHandler.OnNewPlayerEvent += OnNewPlayer;
        MessageHandler.OnLeaveEvent += OnLeave;
        MessageHandler.OnCardsEvent += OnCards;
        MessageHandler.OnPlayEvent += OnPlay;
        MessageHandler.OnStartRowEvent += OnStartRowEvent;
        MessageHandler.OnEliminatedEvent += OnEliminatedEvent;
        MessageHandler.OnResultEvent += OnResultEvent;
        startButton.gameObject.SetActive(false);
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
        if (currentState == State.Play)
        {
            RenderPlays();
        }

        if (currentState == State.Cards)
        {
            RenderCards();
        }

        if (currentState == State.Joint)
        {
            if (!inGame && otherPlayers.Count != 0)
            {
                startButton.gameObject.SetActive(true);
            }
            RenderPlayers();
        }

        

        currentState = State.Idle;
    }

    public void OnApplicationQuit()
    {
        GameClient.OnConnectEvent -= OnConnect;
        MessageHandler.OnNewPlayerEvent -= OnNewPlayer;
        MessageHandler.OnLeaveEvent -= OnLeave;
        MessageHandler.OnJoinEvent -= OnJoin;
        MessageHandler.OnCardsEvent -= OnCards;
        MessageHandler.OnPlayEvent -= OnPlay;
        MessageHandler.OnStartRowEvent -= OnStartRowEvent;
        MessageHandler.OnEliminatedEvent -= OnEliminatedEvent;
        MessageHandler.OnResultEvent -= OnResultEvent;
        GameClient.Disconnect();

        startButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        foldButton.onClick.RemoveAllListeners();
    }

    public void RenderCards() {
        float xOffset;
        float zOffset;

        xOffset = -3.3f;
        zOffset = 0.03f;
        for (int i = 0; i < player.cards.Count; i++)
        {
            Debug.Log(player.cards[i]);
            GameObject newCard = Instantiate(playerCard, new Vector3(playerView.transform.position.x + xOffset, playerView.transform.position.y, playerView.transform.position.z - zOffset), Quaternion.identity);
            Selectable selectable = newCard.GetComponent<Selectable>();
            selectable.faceup = true;
            newCard.name = player.cards[i];
            xOffset += 1.3f;
            zOffset += 0.03f;
            Debug.Log(transform.position.x);
        }
        for (int j = 0; j < otherPlayers.Count; j++)
        {
            Debug.Log("otherplayer " + otherPlayers[j].mappedIndex);
            GameObject newCard = Instantiate(playerCard, new Vector3(otherCard[otherPlayers[j].mappedIndex].transform.position.x, otherCard[otherPlayers[j].mappedIndex].transform.position.y, otherCard[otherPlayers[j].mappedIndex].transform.position.z), Quaternion.identity);
            newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, 0);
        }
    }

    public void RenderPlays() {
        foreach(var p in plays)
        {
            if (p.userid == userId)
            {
                player.cards.Remove(p.card);
                player.numCard--;
                Debug.Log(p.card);
                GameObject obj = GameObject.Find(p.card);
                Selectable selectable = obj.GetComponent<Selectable>();
                if (p.action == MessageHandler.PLAY)
                {
                    selectable.faceup = true;
                }
                float xOffset = -0.7f + (0.3f * (5 - player.numCard));
                float zOffset = 0.03f + (0.03f * (5 - player.numCard));
                selectable.targetPos = new Vector3(playView[0].transform.position.x + xOffset, playView[0].transform.position.y, playView[0].transform.position.z - zOffset);
                Debug.Log(selectable.targetPos);
                Debug.Log(obj.transform.position);
                selectable.targetScale = new Vector3(obj.transform.localScale.x * 0.8f, obj.transform.localScale.y * 0.8f, obj.transform.localScale.z);
            }
            else
            {
                foreach (var player in otherPlayers) { 
                    if (p.userid == player.playerInfo.userId) {
                        player.numCard--;
                        int index = player.mappedIndex;
                        GameObject newCard = Instantiate(playerCard, new Vector3(otherCard[index].transform.position.x, otherCard[index].transform.position.y, otherCard[index].transform.position.z), Quaternion.identity);
                        newCard.transform.localScale = new Vector3(newCard.transform.localScale.x * 0.6f, newCard.transform.localScale.y * 0.6f, 0);
                        Selectable selectable = newCard.GetComponent<Selectable>();
                        if (p.action == MessageHandler.PLAY)
                        {
                            selectable.faceup = true;
                        }
                        float xOffset = -0.7f + (0.3f * (5 - player.numCard));
                        float zOffset = 0.03f + (0.03f * (5 - player.numCard));
                        selectable.targetPos = new Vector3(playView[index + 1].transform.position.x + xOffset, playView[index + 1].transform.position.y, playView[index + 1].transform.position.z - zOffset);
                        Debug.Log(selectable.targetPos);
                        selectable.targetScale = new Vector3(newCard.transform.localScale.x * 1.34f, newCard.transform.localScale.y * 1.34f, newCard.transform.localScale.z);
                    }
                }
            }
        }
        plays.Clear();
    }

    void RenderNewPlayer() {
        Player player = otherPlayers[otherPlayers.Count - 1];
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject obj = Instantiate(userInfo);
        obj.name = player.playerInfo.userId;
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[player.mappedIndex + 1].transform.position.x, playerInfos[player.mappedIndex + 1].transform.position.y, playerInfos[player.mappedIndex + 1].transform.position.z);
        Text[] child = obj.GetComponentsInChildren<Text>();
        for (int i = 0; i < child.Length; i++)
        {
            child[i].transform.SetParent(obj.transform);
            if (child[i].name == "Username")
            {
                child[i].text = player.playerInfo.userId;
            }
            if (child[i].name == "Amount")
            {
                child[i].text = player.playerInfo.amount.ToString();
            }
        }
        
        obj.transform.localScale = new Vector3(1, 1, 1);
    }

    void RenderPlayers() {
        
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject obj = Instantiate(userInfo);
        obj.name = player.playerInfo.userId;
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.position = new Vector3(playerInfos[0].transform.position.x, playerInfos[0].transform.position.y, playerInfos[0].transform.position.z);
        Text[] child = obj.GetComponentsInChildren<Text>();
        for (int i = 0; i < child.Length; i++)
        {
            if (child[i].name == "Username")
            {
                child[i].text = player.playerInfo.userId;
            }
            if (child[i].name == "Amount")
            {
                child[i].text = player.playerInfo.amount.ToString();
            }
        }
        
        obj.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            Debug.Log("xxxxx");
            GameObject otherObj = Instantiate(userInfo);
            otherObj.name = otherPlayers[i].playerInfo.userId;
            otherObj.transform.SetParent(canvas.transform, false);
            otherObj.transform.position = new Vector3(playerInfos[otherPlayers[i].mappedIndex + 1].transform.position.x, playerInfos[otherPlayers[i].mappedIndex + 1].transform.position.y, playerInfos[otherPlayers[i].mappedIndex + 1].transform.position.z);
            Text[] otherChilds = otherObj.GetComponentsInChildren<Text>();
            for (int j = 0; j < otherChilds.Length; j++)
            {
                if (otherChilds[i].name == "Username")
                {
                    otherChilds[i].text = otherPlayers[i].playerInfo.userId;
                }
                if (otherChilds[i].name == "Amount")
                {
                    otherChilds[i].text = otherPlayers[i].playerInfo.amount.ToString();
                }
            }
            
            otherObj.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void StartGame() {
        startButton.gameObject.SetActive(false);
        Debug.Log("StartGame");
        MessageHandler.Deal();
    }

    public void OnConnect()
    {
        Debug.Log("Server Connected");
        MessageHandler.JoinRoom(playerInfo);
    }

    public void OnJoin(List<Player> players) {
        otherPlayers.Clear();
        foreach(var p in players) {
            if (p.inGame == true)
            {
                //inGame = true;
            }
            if (p.playerInfo.userId == userId)
            {
                
                Debug.Log("JOINT");
                player = p;
                player.mappedIndex = 0;
            }
            else {
                Debug.Log("Have other player");
                otherPlayers.Add(p);
            }
        }

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            otherPlayers[i].mappedIndex = MapIndex(player.index, otherPlayers[i].index);
        }

        currentState = State.Joint;
    }

    public void OnNewPlayer(Player newPlayer) {
        newPlayer.mappedIndex = MapIndex(player.index, newPlayer.index);
        otherPlayers.Add(newPlayer);
        currentState = State.New;
    }

    public void OnLeave(string id)
    {
        for (int i = 0; i < otherPlayers.Count; i++) {
            if (otherPlayers[i].playerInfo.userId == id) {
                leavePlayerIndex = otherPlayers[i].mappedIndex;
            }
        }
        currentState = State.Leave;
    }
    
    private int MapIndex(int playerIndex, int otherPlayerIndex)
    {
        Debug.Log(playerIndex.ToString() + " " + otherPlayerIndex.ToString());
        if (otherPlayerIndex < playerIndex)
        {
            return playerIndex - otherPlayerIndex - 1;
        }
        else
        {
            return MAXPLAYER - otherPlayerIndex - 1;
        }
    } 

    public void OnCards(List<string> cards) {
        player.cards = cards;
        player.numCard = MAXCARD;
        foreach(var p in otherPlayers) {
            p.numCard = 6;
        }
        currentState = State.Cards;
    }

    public void OnStartRowEvent(string id) {
    
    }

    public void OnEliminatedEvent(List<string> disqualifiers) {
        
    }

    public void OnResultEvent() {
    
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
                MessageHandler.Play(obj.name);
            }
        }
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
                MessageHandler.Fold(obj.name);
            }
        }
    }

    public void OnPlay(Play play)
    {
        plays.Add(play);
        currentState = State.Play;
    }

    public void onBeginTurn(int index, bool newRound) {
        // Start timer on index

    }

    public void onShowCard(int index, string card) {

    }
}
