using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
// Handles displaying game state
public class UIController : MonoBehaviour
{
    [TitleGroup("Component References"), SerializeField] GameObject _cardPrefab;
    [TitleGroup("Component References"), SerializeField] GameController _gameController;
    [TitleGroup("Component References"), SerializeField] Sprite _defendIcon;
    [TitleGroup("Component References"), SerializeField] Sprite _attackIcon;


    [BoxGroup("Sidebar")]
    [SerializeField, BoxGroup("Sidebar")] TextMeshProUGUI _roundCount;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1Name;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2Name;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1DeckCount;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2DeckCount;
    [SerializeField, BoxGroup("Sidebar/ID1")] TextMeshProUGUI _index1HP;
    [SerializeField, BoxGroup("Sidebar/ID2")] TextMeshProUGUI _index2HP;
    [SerializeField, BoxGroup("Sidebar/ID1")] Image _index1Icon;
    [SerializeField, BoxGroup("Sidebar/ID2")] Image _index2Icon;

    [SerializeField, BoxGroup("Board")] Transform _index1CardContainer;
    [SerializeField, BoxGroup("Board")] Transform _index2CardContainer;
    [SerializeField, BoxGroup("Board")] Transform _deck1Transform;
    [SerializeField, BoxGroup("Board")] Transform _deck2Transform;
    [SerializeField, BoxGroup("Board")] Transform _board;
    [SerializeField, BoxGroup("Board")] Image _player1TotalDisplay;
    [SerializeField, BoxGroup("Board")] Image _player2TotalDisplay;
    [SerializeField, BoxGroup("Board")] TextMeshProUGUI _player1TotalTextDisplay;
    [SerializeField, BoxGroup("Board")] TextMeshProUGUI _player2TotalTextDisplay;

    [SerializeField, BoxGroup("Board")] List<CardController> _player1Cards = new List<CardController>();
    [SerializeField, BoxGroup("Board")] List<CardController> _player2Cards = new List<CardController>();
    [SerializeField, BoxGroup("Board")] GameObject _newRoundDisplay;
    [SerializeField, BoxGroup("Board")] TextMeshProUGUI _newRoundDisplayText;

    [SerializeField, BoxGroup("Effects")] GameObject _attackTallyPS;         //the effect spawned by the card when tally the defense/attack amount



    Player _player1;
    Player _player2;


    public void InitialiseGame(Player player1, Player player2)
    {
        _player1 = player1;
        _player2 = player2;

        _index1Name.text = player1.PlayerPepemon.DisplayName;
        _index2Name.text = player2.PlayerPepemon.DisplayName;

        _index1DeckCount.text = player1.PlayerDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = player2.PlayerDeck.GetDeck().Count + "cards";

        _index1HP.text = player1.CurrentHP + "hp";
        _index2HP.text = player2.CurrentHP + "hp";

        _index1Icon.sprite = player1.PlayerPepemon.DisplayIcon;
        _index2Icon.sprite = player2.PlayerPepemon.DisplayIcon;
    }

    public void NewRoundDisplay()
    {
        _newRoundDisplay.SetActive(true);
        _newRoundDisplayText.text = "Round: " + _gameController.GetRoundNumber();
    }

    public void HideNewRoundDisplay()
    {
        _newRoundDisplay.SetActive(false);
    }


    // part1 lay down all AD + DF cards
    // play the hand
    // Reverse role
    // next round

    public void DisplayHands()
    {
        // Cleanup old hand
        foreach (CardController _card in _player1Cards) { GameObject.Destroy(_card.gameObject); }
        foreach (CardController _card in _player2Cards) { GameObject.Destroy(_card.gameObject); }
        foreach (Transform child in _index1CardContainer) GameObject.Destroy(child.gameObject);
        foreach (Transform child in _index2CardContainer) GameObject.Destroy(child.gameObject);
        _player1Cards.Clear();
        _player2Cards.Clear();
        // Display new hand
        StartCoroutine(DrawCards(_player1));
        StartCoroutine(DrawCards(_player2));



        // Update deck count
        _index1DeckCount.text = _player1.CurrentDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = _player2.CurrentDeck.GetDeck().Count + "cards";
        _roundCount.text = "R: " + _gameController.GetRoundNumber();
    }

    IEnumerator DrawCards(Player _whichPlayer)
    {
        for (int i = 0; i < _whichPlayer.CurrentHand.GetCardsInHand.Count; i++)
        {
            //delay between each card spawn for effect
            yield return new WaitForSeconds(.2f);
            GameObject go = new GameObject("Card Container", typeof(RectTransform));
            GameObject card;
            if (_whichPlayer == _player1) card = Instantiate(_cardPrefab, _deck1Transform.position, Quaternion.identity);
            else card = Instantiate(_cardPrefab, _deck2Transform.position, Quaternion.identity);
            card.transform.SetParent(_board);


            if (_whichPlayer == _player1) go.transform.SetParent(_index1CardContainer);
            else go.transform.SetParent(_index2CardContainer);
            go.transform.localPosition = Vector3.zero;
            card.GetComponent<CardController>().PouplateCard(_whichPlayer.CurrentHand.GetCardsInHand[i]);

            card.GetComponent<CardController>().SetTargetTransform(go.transform);
            if (_whichPlayer == _player1) _player1Cards.Add(card.GetComponent<CardController>());
            else _player2Cards.Add(card.GetComponent<CardController>());
        }
    }


    // disables cards based on attacking or defending
    public void FlipCards(int attackIndex)
    {
        if (attackIndex == 1) // p1
        {
            for (int i = 0; i < _player1Cards.Count; i++)
            {
                if (_player1Cards[i].HostedCard.IsAttackingCard() == false)
                {
                    _player1Cards[i].SetAttackingTransform(1);
                    _player1Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }

            for (int i = 0; i < _player2Cards.Count; i++)
            {
                if (_player2Cards[i].HostedCard.IsAttackingCard() == true)
                {
                    _player2Cards[i].SetAttackingTransform(2);
                    _player2Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }
        }
        else if (attackIndex == 2) // p2
        {
            for (int i = 0; i < _player2Cards.Count; i++)
            {
                if (_player2Cards[i].HostedCard.IsAttackingCard() == false)
                {
                    _player2Cards[i].SetAttackingTransform(2);

                    _player2Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }

            for (int i = 0; i < _player1Cards.Count; i++)
            {
                if (_player1Cards[i].HostedCard.IsAttackingCard() == true)
                {
                    _player1Cards[i].SetAttackingTransform(1);

                    _player1Cards[i].GetComponent<Image>().color = Color.gray;
                }
            }
        }
        else if (attackIndex == 3) // reset cards
        {
            for (int i = 0; i < _player2Cards.Count; i++)
            {
                _player2Cards[i].ReturnToBaseTransform();

                _player2Cards[i].GetComponent<Image>().color = Color.black;
            }

            for (int i = 0; i < _player1Cards.Count; i++)
            {
                _player1Cards[i].ReturnToBaseTransform();

                _player1Cards[i].GetComponent<Image>().color = Color.black;
            }
        }
    }

    /// <summary>
    /// Sequence where the shielf and sword icons are displayed and the total attk/def are displayed
    /// </summary>
    /// <param name="attackIndex"></param>
    /// <param name="_totalAttack"></param>
    /// <param name="_totalDef"></param>
    /// <returns></returns>
    public IEnumerator DisplayTotalValues(int attackIndex, int _totalAttack, int _totalDef)
    {

        _player1TotalDisplay.gameObject.SetActive(true);
        _player2TotalDisplay.gameObject.SetActive(true);
        TallyUpCardValues(attackIndex);


        yield return new WaitForSeconds(2f);

        _player1TotalDisplay.GetComponent<Animator>().SetTrigger("Clash");
        _player2TotalDisplay.GetComponent<Animator>().SetTrigger("Clash");


        int _maxTallyP1;
        int _maxTallyP2;
        int _currentTallyP1;
        int _currentTallyP2;

        //display the proper attack and defend symbols
        if (attackIndex == 1)
        {
            _player1TotalTextDisplay.text = _totalAttack.ToString();
            _player2TotalTextDisplay.text = _totalDef.ToString();
            _player1TotalDisplay.sprite = _attackIcon;
            _player2TotalDisplay.sprite = _defendIcon;
        }
        else if (attackIndex == 2)
        {
            _player1TotalTextDisplay.text = _totalDef.ToString();
            _player2TotalTextDisplay.text = _totalAttack.ToString();
            _player1TotalDisplay.sprite = _defendIcon;
            _player2TotalDisplay.sprite = _attackIcon;
        }




        yield return new WaitForSeconds(1f);

        _player1TotalDisplay.gameObject.SetActive(false);
        _player2TotalDisplay.gameObject.SetActive(false);

    }

    /// <summary>
    /// Show the effect/animation of the player's total def/attack being tallied up. This involves particle systems spawning and moving toward the display.
    /// </summary>
    /// <param name="_attackIndex"></param>
    void TallyUpCardValues(int _attackIndex)
    {
        if (_attackIndex == 1)
        {
            foreach (CardController _card in _player1Cards)
            {
                if (_card.HostedCard.IsAttackingCard())
                {
                    GameObject _ps = Instantiate(_attackTallyPS, _card.transform.position, Quaternion.identity);
                    _ps.GetComponent<TallyParticleEffect>().targetPosition = _player1TotalDisplay.transform.position;
                }
            }
            foreach (CardController _card in _player2Cards)
            {
                if (_card.HostedCard.Type == PlayCardType.Defense)
                {
                    GameObject _ps = Instantiate(_attackTallyPS, _card.transform.position, Quaternion.identity);
                    _ps.GetComponent<TallyParticleEffect>().targetPosition = _player2TotalDisplay.transform.position;
                }
            }
        }
        else if (_attackIndex == 2)
        {
            foreach (CardController _card in _player1Cards)
            {
                if (_card.HostedCard.Type == PlayCardType.Defense)
                {
                    GameObject _ps = Instantiate(_attackTallyPS, _card.transform.position, Quaternion.identity);
                    //particle system moves toward the tally display for effect
                    _ps.GetComponent<TallyParticleEffect>().targetPosition = _player1TotalDisplay.transform.position;
                }
            }
            foreach (CardController _card in _player2Cards)
            {
                if (_card.HostedCard.IsAttackingCard())
                {
                    GameObject _ps = Instantiate(_attackTallyPS, _card.transform.position, Quaternion.identity);
                    //particle system moves toward the tally display for effect
                    _ps.GetComponent<TallyParticleEffect>().targetPosition = _player2TotalDisplay.transform.position;
                }
            }
        }
    }


    public void UpdateUI()
    {
        _index1HP.text = _player1.CurrentHP + "hp";
        _index2HP.text = _player2.CurrentHP + "hp";

        _index1DeckCount.text = _player1.CurrentDeck.GetDeck().Count + "cards";
        _index2DeckCount.text = _player2.CurrentDeck.GetDeck().Count + "cards";
        _roundCount.text = "R: " + _gameController.GetRoundNumber();
    }

    public void DisplayWinner(Player player)
    {
        Debug.Log("WINNER: " + player.PlayerPepemon.DisplayName);
    }
}
