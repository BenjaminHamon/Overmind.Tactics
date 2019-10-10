using Overmind.Tactics.Model.Commands;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class ReplayController : MonoBehaviour
	{
		public GameView GameView;
		public int CommandIndex;
		public float CommandIntervalSeconds;
		public float LastCommandTime;
		
		public bool IsRunning { get; private set; }

		public void Pause()
		{
			IsRunning = false;
		}

		public void Resume()
		{
			LastCommandTime = Time.time;
			IsRunning = true;
		}

		public void Update()
		{
			if (IsRunning == false)
				return;

			if (CommandIndex >= GameView.Data.CommandHistory.Count)
			{
				Complete();
			}
			else
			{
				ExecuteNextCommand();
			}
		}

		private void ExecuteNextCommand()
		{
			float now = Time.time;
			if (now < (LastCommandTime + CommandIntervalSeconds))
				return;

			// Debug.LogFormat(this, "[ReplayController] Executing command {0}", CommandIndex);
			IGameCommand command = (IGameCommand)GameView.Data.CommandHistory[CommandIndex];
			GameView.Model.ExecuteCommand(command);

			CommandIndex += 1;
			LastCommandTime = now;
		}

		private void Complete()
		{
			IsRunning = false;
		}
	}
}
