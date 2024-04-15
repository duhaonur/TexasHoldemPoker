using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AIDecisionWeightsSO WeightSettings;

    public Transform[] CommunityCardSpawnPositions;
    public Transform DeckSpawnPos;

    public float SpaceBetweenCards;

    public TextMeshProUGUI PotText;

    public List<Card> DeckPrefabs;

    private List<Card> _deck;

    private SeatController _seatController;

    private int _currentCommunityCard = 0;

    private void Awake()
    {
        _seatController = CheckNull(_seatController);

        if (WeightSettings == null)
            WeightSettings = new AIDecisionWeightsSO();
    }
    private void Start()
    {
        GameEvents.CallSentDeck(CreateAndShuffleDeck());
        UpdatePotText();
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerSeated += PlayerSeatedStartTheGame;
        GameEvents.OnUpdatePotText += UpdatePotText;
        GameEvents.OnCommunityCard += DisplayCommunityCard;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerSeated -= PlayerSeatedStartTheGame;
        GameEvents.OnUpdatePotText -= UpdatePotText;
        GameEvents.OnCommunityCard -= DisplayCommunityCard;
    }
    private void DisplayCommunityCard(Card card)
    {
        card.transform.SetParent(CommunityCardSpawnPositions[_currentCommunityCard], false);
        card.transform.localRotation = Quaternion.Euler(-90, card.transform.rotation.y, card.transform.rotation.z);
        card.transform.localScale = Vector3.one;
        StartCoroutine(card.DisplayCard(1));
        _currentCommunityCard++;
    }
    private void UpdatePotText()
    {
        PotText.text = SharedData.Pot == 0 ? "" : SharedData.Pot.ToString();
    }
    private void PlayerSeatedStartTheGame(Transform player)
    {
        Dictionary<int, Seat> players = new Dictionary<int, Seat>();
        var seats = _seatController._seats;
        var orderedSeats = seats.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        seats = orderedSeats;
        int counter = 0;
        foreach (var seat in seats.Values)
        {
            players.Add(counter, seat);
            counter++;
            if (seat.seatedObj == null)
            {
                PlayerAI ai = seat.gameObject.AddComponent<PlayerAI>();
                ai.Seat = seat;
                ai.SeatUI = seat.SeatUI;
                ai.SeatId = seat.SeatId;
                ai.WeightSettings = WeightSettings;
                ai.SetLookAtConstraintSource(player);
                seat.seatedObj = ai.gameObject;
            }
        }
        GameEvents.CallSendPlayerCount(players);
        GameEvents.CallStartGame();
    }
    private Stack<Card> CreateAndShuffleDeck()
    {
        _deck = new List<Card>();
        for (int i = 0; i < DeckPrefabs.Count; i++)
        {
            Card newCard = Instantiate(DeckPrefabs[i], DeckSpawnPos.position, DeckPrefabs[i].transform.rotation, DeckSpawnPos);
            _deck.Add(newCard);
        }
        SharedData.Deck = _deck;
        #region CreateDeck

        #endregion
        #region Shuffle Deck
        System.Random rng = new System.Random();
        int n = _deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_deck[n], _deck[k]) = (_deck[k], _deck[n]);
        }

        Stack<Card> shuffledDeck = new Stack<Card>();
        Vector3 newPos = Vector3.zero;
        newPos.y = SpaceBetweenCards * _deck.Count;
        foreach (Card card in _deck)
        {
            card.gameObject.transform.localPosition = newPos;
            shuffledDeck.Push(card);
            newPos.y -= SpaceBetweenCards;
        }
        #endregion
        return shuffledDeck;
    }
    private T CheckNull<T>(T component) where T : Component
    {
        if (component == null)
        {
            if (TryGetComponent(out T comp))
            {
                return comp;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
        else
        {
            return component;
        }
    }
}
