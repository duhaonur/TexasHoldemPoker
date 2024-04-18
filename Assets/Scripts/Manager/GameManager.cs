using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Reference to the ScriptableObject containing AI decision weights
    public AIDecisionWeightsSO WeightSettings;

    // Array of transform positions where community cards will be spawned
    public Transform[] CommunityCardSpawnPositions;

    // Transform position where the deck will be spawned
    public Transform DeckSpawnPos;

    // Space between cards when they are stacked
    public float SpaceBetweenCards;

    // Reference to the TextMeshProUGUI for displaying the pot amount
    public TextMeshProUGUI PotText;

    // Prefabs for the cards in the deck
    public List<Card> DeckPrefabs;

    // Private fields
    private List<Card> _deck;
    private Dictionary<int, Seat> _players;
    private SeatController _seatController;
    private int _currentCommunityCard = 0;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure SeatController is not null
        _seatController = CheckNull(_seatController);

        // Ensure WeightSettings is not null
        if (WeightSettings == null)
            WeightSettings = new AIDecisionWeightsSO();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the game by creating and shuffling the deck
        GameEvents.CallSentDeck(ShuffleDeck(CreateDeck()));

        // Update the pot text
        UpdatePotText();
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnPlayerSeated += PlayerSeatedStartTheGame;
        GameEvents.OnUpdatePotText += UpdatePotText;
        GameEvents.OnCommunityCard += DisplayCommunityCard;
        GameEvents.OnResetGame += GameReset;
    }

    // OnDisable is called when the object becomes disabled or inactive
    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnPlayerSeated -= PlayerSeatedStartTheGame;
        GameEvents.OnUpdatePotText -= UpdatePotText;
        GameEvents.OnCommunityCard -= DisplayCommunityCard;
        GameEvents.OnResetGame -= GameReset;
    }

    // Method to reset the game state
    private void GameReset()
    {
        // Hide all cards in the deck
        foreach (var card in _deck)
        {
            StartCoroutine(card.HideCard(1f));
        }

        _currentCommunityCard = 0;

        // Reset pot and betting values
        SharedData.Pot = 0;
        SharedData.HighestBet = 0;

        // Create and shuffle the deck again
        GameEvents.CallSentDeck(ShuffleDeck(_deck));

        // Send player count to subscribers
        GameEvents.CallSendPlayerCount(_players);

        // Update the pot text
        UpdatePotText();
    }

    // Method to display a community card
    private void DisplayCommunityCard(Card card)
    {
        // Set parent transform and position for the card
        card.transform.SetParent(CommunityCardSpawnPositions[_currentCommunityCard], false);
        card.transform.localRotation = Quaternion.Euler(-90, card.transform.rotation.y, card.transform.rotation.z);
        Vector3 newPos = card.transform.localPosition;
        newPos.y = 0;
        card.transform.localPosition = newPos;

        // Animate the card's appearance
        StartCoroutine(card.DisplayCard(1));

        // Move to the next community card position
        _currentCommunityCard++;
    }

    // Method to update the pot text display
    private void UpdatePotText()
    {
        PotText.text = SharedData.Pot == 0 ? "" : SharedData.Pot.ToString();
    }

    // Method to initialize players and start the game
    private void PlayerSeatedStartTheGame(Transform player)
    {
        // Initialize players dictionary
        _players = new Dictionary<int, Seat>();

        // Order seats by key (seat ID)
        var seats = _seatController.Seats;
        var orderedSeats = seats.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        seats = orderedSeats;

        // Iterate through seats and add players
        int counter = 0;
        foreach (var seat in seats.Values)
        {
            _players.Add(counter, seat);
            counter++;

            // If seat is empty, add AI player
            if (seat.seatedObj == null)
            {
                PlayerAI ai = seat.gameObject.AddComponent<PlayerAI>();
                ai.Seat = seat;
                ai.SeatUI = seat.SeatUI;
                ai.SeatId = seat.SeatId;
                ai.WeightSettings = WeightSettings;
                seat.seatedObj = ai.gameObject;
            }
        }

        // Send player count to subscribers
        GameEvents.CallSendPlayerCount(_players);

        // Start the game
        GameEvents.CallStartGame();
    }

    // Method to create and shuffle the deck
    private List<Card> CreateDeck()
    {
        // Initialize deck list
        _deck = new List<Card>();

        // Instantiate deck prefabs
        for (int i = 0; i < DeckPrefabs.Count; i++)
        {
            Card newCard = Instantiate(DeckPrefabs[i], DeckSpawnPos.position, DeckPrefabs[i].transform.rotation, DeckSpawnPos);
            _deck.Add(newCard);
        }
        return _deck;
    }
    private Stack<Card> ShuffleDeck(List<Card> deck)
    {
        // Shuffle the deck
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (deck[n], deck[k]) = (deck[k], deck[n]);
        }

        // Create a shuffled deck stack
        Stack<Card> shuffledDeck = new Stack<Card>();
        Vector3 newPos = Vector3.zero;
        newPos.y = SpaceBetweenCards * deck.Count;
        foreach (Card card in deck)
        {
            card.transform.SetParent(DeckSpawnPos, false);
            card.gameObject.transform.localPosition = newPos;
            card.gameObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
            shuffledDeck.Push(card);
            newPos.y -= SpaceBetweenCards;
        }

        SharedData.Deck = new List<Card>(shuffledDeck);

        return shuffledDeck;
    }
    // Method to check if a component is null and handle it
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
