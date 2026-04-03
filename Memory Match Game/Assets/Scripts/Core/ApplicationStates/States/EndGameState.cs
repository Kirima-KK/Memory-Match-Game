using MemoryMatch.Core.ApplicationStates.ControllerInterfaces;
using MemoryMatch.Models;
using MemoryMatch.Utilities;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace MemoryMatch.Core.ApplicationStates.States
{
    public class EndGameState : BaseApplicationState
    {
        public EndGameState(IStateManagable manager) : base(manager)
        {
        }

        public override int ID => (int)StateIndex.End;

        public override string Name => StateIndex.End.ToString();

        private IEndGameControllable m_EndGameplayController;

        public override void StateIn(params object[] args)
        {
            Debug.Log($"[StateIn] Enter {Name}");
            m_EndGameplayController = Object.FindObjectsByType<MonoBehaviour>().OfType<IEndGameControllable>().FirstOrDefault();
            m_EndGameplayController.OnBackToMenu += BackToMenuHandler;
            m_EndGameplayController.OnRestart += RestartGameHandler;

            if(DataFormatValidator.IsArgumentContain(args, out string timerText))
            {
                m_EndGameplayController.ShowGameEndUI(timerText);
            }
            else
            {
                m_EndGameplayController.ShowGameEndUI(null);
            }
        }

        public override void StateOut()
        {
            Debug.Log($"[StateOut] Exit {Name}");
            m_EndGameplayController.OnBackToMenu -= BackToMenuHandler;
            m_EndGameplayController.OnRestart -= RestartGameHandler;
        }

        private void RestartGameHandler()
        {
            m_AppStateManager.StartLoadScene(StateIndex.Gameplay);
        }

        private void BackToMenuHandler()
        {
            m_AppStateManager.StartLoadScene(StateIndex.Starter);
        }
    }
}