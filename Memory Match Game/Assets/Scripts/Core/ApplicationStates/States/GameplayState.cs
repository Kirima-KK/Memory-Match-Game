using MemoryMatch.Core.ApplicationStates.ControllerInterfaces;
using MemoryMatch.Models;
using System.Linq;
using UnityEngine;

namespace MemoryMatch.Core.ApplicationStates.States
{
    public class GameplayState : BaseApplicationState
    {
        public GameplayState(IStateManagable manager) : base(manager)
        {
        }

        public override int ID => (int)StateIndex.Gameplay;

        public override string Name => StateIndex.Gameplay.ToString();

        private IGameplayControllable m_GameplayController;

        public override void StateIn(params object[] args)
        {
            Debug.Log($"[StateIn] Enter {Name}");
            m_GameplayController = Object.FindObjectsByType<MonoBehaviour>().OfType<IGameplayControllable>().FirstOrDefault();
            m_GameplayController.StartGameplay();
            m_GameplayController.OnGameplayEnded += GameplayEndedHandler;
        }

        public override void StateOut()
        {
            Debug.Log($"[StateOut] Exit {Name}");
            m_GameplayController.OnGameplayEnded -= GameplayEndedHandler;
        }

        private void GameplayEndedHandler(string timerText)
        {
            m_AppStateManager.ChangeStateTo(StateIndex.End, timerText);
        }
    }
}