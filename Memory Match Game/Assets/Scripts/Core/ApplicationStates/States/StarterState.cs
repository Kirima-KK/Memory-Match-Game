using MemoryMatch.Core.ApplicationStates.ControllerInterfaces;
using MemoryMatch.Models;
using System.Linq;
using UnityEngine;

namespace MemoryMatch.Core.ApplicationStates.States
{
    public class StarterState : BaseApplicationState
    {
        public StarterState(IStateManagable manager) : base(manager) { }

        public override int ID => (int)StateIndex.Starter;

        public override string Name => StateIndex.Starter.ToString();

        private IMenuControllable m_MenuController;

        public override void StateIn(params object[] args)
        {
            Debug.Log($"[StateIn] Enter {Name}");
            m_MenuController = Object.FindObjectsByType<MonoBehaviour>().OfType<IMenuControllable>().FirstOrDefault();
            m_MenuController.OnGameplayStarted += GameplayStartedHandler;
        }

        public override void StateOut()
        {
            Debug.Log($"[StateOut] Exit {Name}");
            m_MenuController.OnGameplayStarted -= GameplayStartedHandler;
        }

        private void GameplayStartedHandler()
        {
            m_AppStateManager.StartLoadScene(StateIndex.Gameplay);
        }
    }
}