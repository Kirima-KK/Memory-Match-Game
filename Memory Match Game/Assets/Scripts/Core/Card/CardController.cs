using MemoryMatch.Core.ApplicationStates.ControllerInterfaces;
using MemoryMatch.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MemoryMatch.Core.Card
{
    public class CardController : MonoBehaviour, ICardControllable
    {
        [SerializeField]
        private GameObject m_CardPrefab;

        [SerializeField]
        private Transform m_CardContainer;

        [SerializeField]
        private GridLayoutGroup m_LayoutGroup;

        [SerializeField]
        private List<Texture2D> m_CardFrontList;

        private Dictionary<Texture2D, int> m_SetTextureAmountDictionary = new Dictionary<Texture2D, int>();

        private List<ICardElementUI> m_CurrentFlipedIds = new List<ICardElementUI>(2);
        private List<ICardElementUI> m_SpawnedCards = new List<ICardElementUI>();

        private const float CardLifeTime = 0.5f;
        private const float TotalCardAmount = 16;
        private RectTransform rect;

        public UnityAction OnAllCardFliped { get; set; }

        private void Update()
        {
            //SetupGridSize();

            if (CheckIsAllCardFliped())
            {
                OnAllCardFliped?.Invoke();
            }
        }

        private bool CheckIsAllCardFliped()
        {
            if (m_SpawnedCards.Any(card => card.IsAlreadyMatch == false)) return false;
            return true;
        }

        public void GenerateCards()
        {
            InitFrontSprite();

            for (int i = 0; i < TotalCardAmount; i++)
            {
                var card = Instantiate(m_CardPrefab, m_CardContainer);
                var cardElement = card.GetComponent<ICardElementUI>();
                cardElement.Id = i;
                cardElement.OnCardFliped += OnCardFlipedHandler;

                // random texture
                if (m_SetTextureAmountDictionary.Count == 0) break;
                int randomTextureIndex = UnityEngine.Random.Range(0, m_SetTextureAmountDictionary.Count - 1);
                var texture = m_SetTextureAmountDictionary.Select(pair => pair.Key).ToList();
                cardElement.SetFrontTexture(texture[randomTextureIndex]);
                m_SetTextureAmountDictionary[texture[randomTextureIndex]]++;
                if (m_SetTextureAmountDictionary[texture[randomTextureIndex]] >= 2) m_SetTextureAmountDictionary.Remove(texture[randomTextureIndex]);

                m_SpawnedCards.Add(cardElement);
            }

            SetupMatchId();
        }

        private void InitFrontSprite()
        {
            foreach (var texture in m_CardFrontList)
            {
                m_SetTextureAmountDictionary.Add(texture, 0);
            }
        }

        private void SetupMatchId()
        {
            foreach (var card in m_SpawnedCards)
            {
                var spawnedTextureList = m_SpawnedCards.Where(spawnedCard => spawnedCard.Id != card.Id && spawnedCard.FrontTexture == card.FrontTexture).ToList();

                card.MatchId = spawnedTextureList[0].Id;
            }
        }

        private void FaceDownAllCards()
        {
            foreach (var card in m_SpawnedCards)
            {
                if (!card.IsAlreadyMatch && card.CurrentCardStatus == CardStatus.FaceUp) card.FlipCard(CardStatus.FaceDown);
            }

            m_CurrentFlipedIds.Clear();
        }

        private IEnumerator StartCardCountDown(ICardElementUI card)
        {
            yield return new WaitForSeconds(CardLifeTime);
            FaceDownAllCards();
        }

        private void OnCardFlipedHandler(ICardElementUI card)
        {
            if (card.CurrentCardStatus == CardStatus.FaceUp) return;
            else if (card.CurrentCardStatus == CardStatus.FaceDown)
            {
                card.FlipCard(CardStatus.FaceUp);
                m_CurrentFlipedIds.Add(card);

                if (m_CurrentFlipedIds.Count > 2)
                {
                    FaceDownAllCards();
                    return;
                }

                if (m_CurrentFlipedIds.Count == 2)
                {
                    if (m_CurrentFlipedIds[0].MatchId == card.Id)
                    {
                        m_CurrentFlipedIds[0].IsAlreadyMatch = true;
                        card.IsAlreadyMatch = true;
                        m_CurrentFlipedIds.Clear();
                    }
                    else
                    {
                        StartCoroutine(StartCardCountDown(card));
                    }
                }
            }
        }

        //private void SetupGridSize()
        //{
        //    if (rect == null) rect = m_CardContainer.GetComponent<RectTransform>();

        //    int gridRowColumns = m_LayoutGroup.constraintCount;

        //    // Get container dimensions minus padding
        //    float width = rect.rect.width - (m_LayoutGroup.padding.left + m_LayoutGroup.padding.right);
        //    float height = rect.rect.height - (m_LayoutGroup.padding.top + m_LayoutGroup.padding.bottom);

        //    // Calculate available space minus spacing
        //    float cellW = (width - (m_LayoutGroup.spacing.x * (gridRowColumns - 1))) / gridRowColumns;
        //    float cellH = (height - (m_LayoutGroup.spacing.y * (gridRowColumns - 1))) / gridRowColumns;

        //    // Use the smaller value to ensure it fits both ways (Perfect Squares)
        //    float finalSize = Mathf.Min(cellW, cellH);

        //    if (finalSize > 0)
        //        m_LayoutGroup.cellSize = new Vector2(finalSize, finalSize);
        //}
    }
}