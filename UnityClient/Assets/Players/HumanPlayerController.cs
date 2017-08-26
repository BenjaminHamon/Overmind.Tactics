using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using Overmind.Tactics.Model.Commands;
using Overmind.Tactics.UnityClient.Navigation;
using Overmind.Tactics.UnityClient.Unity;
using Overmind.Tactics.UnityClient.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;

namespace Overmind.Tactics.UnityClient
{
	/// <summary>Controls a player based on local input by a human.</summary>
	public class HumanPlayerController : MonoBehaviour
	{
		public GameView Game;
		public PlayerView Player;
		public Camera Camera;
		public float CameraSpeed;

		private bool isPickingTarget;
		private bool autoCurrentAbility;
		private IAbility currentAbility;
		
		[SerializeField]
		private GameUserInterface UserInterface;
		[SerializeField]
		private AbilityCastView abilityCastView;

		public void Start()
		{
			UserInterface.Game = Game.Model;
			UserInterface.Player = Player;

			UserInterface.EndTurnButton.onClick.AddListener(() => Game.Model.ExecuteCommand(new EndTurnCommand() { Player = Player.Model, PlayerId = Player.Model.Id }));
			UserInterface.SelectPreviousCharacterButton.onClick.AddListener(() => Player.SelectPrevious());
			UserInterface.SelectNextCharacterButton.onClick.AddListener(() => Player.SelectNext());
			UserInterface.AbilityPanel.AbilityButtonClick += (caster, ability) => PickTarget(caster, false, ability);
			abilityCastView.TargetPositionChanged += UpdateCurrentAbility;
			Player.Model.TurnEnded += _ => ClearCurrentCommand();
		}

		public void Update()
		{
			UpdateCamera();

			if (isPickingTarget)
				UpdateForCommand();
			else
				UpdateForDefault();
		}

		private void UpdateCamera()
		{
			Vector2 move = Vector2.zero;
			if (Input.GetKey(KeyCode.W))
				move.y += 1;
			if (Input.GetKey(KeyCode.D))
				move.x += 1;
			if (Input.GetKey(KeyCode.S))
				move.y -= 1;
			if (Input.GetKey(KeyCode.A))
				move.x -= 1;

			Camera.transform.Translate(move * CameraSpeed * Time.deltaTime);
		}

		private void UpdateForDefault()
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			if (Input.GetMouseButtonUp(InputExtensions.LeftClick))
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				Player.Selection = hit.collider != null ? hit.collider.GetComponentInParent<CharacterView>() : null;
			}

			if ((Player.Selection != null) && (Player.Selection.Model.Owner == Player.Model))
			{
				if (Input.GetMouseButtonDown(InputExtensions.RightClick))
					PickTarget(Player.Selection.Model, true, null);
			}
		}

		private void UpdateForCommand()
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			CharacterModel caster = Player.Selection.Model;

			if (Input.GetMouseButtonUp(autoCurrentAbility ? InputExtensions.RightClick : InputExtensions.LeftClick))
			{
				IGameCommand command = null;
				
				if (currentAbility == null)
				{
					command = new MoveCommand()
					{
						Character = caster,
						CharacterId = caster.Id,
						Path = currentPath ?? new List<Data.Vector2>(),
					};
				}
				else
				{
					Data.Vector2 targetPosition = abilityCastView.TargetPosition.ToModelVector();
					if (caster.ActionPoints < currentAbility.ActionPoints)
						UserInterface.GameMessageView.AddMessage("Not enough action points");
					else if ((caster.Position - targetPosition).Norm > currentAbility.Range)
						UserInterface.GameMessageView.AddMessage("Not in range");
					else
					{
						command = new CastAbilityCommand()
						{
							Character = caster,
							CharacterId = caster.Id,
							Ability = currentAbility,
							AbilityName = currentAbility.Name,
							Target = targetPosition,
						};
					}
				}

				if (command != null)
					Game.Model.ExecuteCommand(command);
				ClearCurrentCommand();
			}

			if (Input.GetMouseButtonUp(autoCurrentAbility ? InputExtensions.LeftClick : InputExtensions.RightClick))
				ClearCurrentCommand();
		}

		private void UpdateCurrentAbility()
		{
			CharacterModel caster = Player.Selection.Model;

			if (autoCurrentAbility)
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null)
				{
					CharacterView target = hit.collider.GetComponentInParent<CharacterView>();
					currentAbility = target != null ? caster.CharacterClass.DefaultAbility : null;
					if (currentAbility != null)
						ClearPath();
					abilityCastView.Change(true, caster, currentAbility);
				}
			}
			
			if (currentAbility == null)
				ShowPath(caster.Position, abilityCastView.TargetPosition.ToModelVector());
		}

		private void PickTarget(CharacterModel caster, bool autoAbility, IAbility ability)
		{
			ClearCurrentCommand();

			if ((ability != null) && (caster.ActionPoints < ability.ActionPoints))
			{
				UserInterface.GameMessageView.AddMessage("Not enough action points");
				return;
			}

			isPickingTarget = true;
			autoCurrentAbility = autoAbility;
			currentAbility = ability;
			UpdateCurrentAbility();
			abilityCastView.Change(true, Player.Selection.Model, ability);
		}

		private void ClearCurrentCommand()
		{
			isPickingTarget = false;
			autoCurrentAbility = false;
			currentAbility = null;
			abilityCastView.Change(false, null, null);
			ClearPath();
		}

		#region Path
		[SerializeField]
		private PathView pathView;
		private Data.Vector2? currentPathStart;
		private Data.Vector2? currentPathEnd;
		private List<Data.Vector2> currentPath;

		private void ShowPath(Data.Vector2 start, Data.Vector2 end)
		{
			start = new Data.Vector2(Mathf.Round(start.X), Mathf.Round(start.Y));
			end = new Data.Vector2(Mathf.Round(end.X),Mathf.Round(end.Y));

			if ((currentPath == null) || (currentPathStart != start) || (currentPathEnd != end))
			{
				// Debug.LogFormat(this, "[HumanPlayerController] FindPath (Start: {0}, End: {1})", start, end);
				currentPath = new List<Data.Vector2>();
				try { currentPath = Game.Model.Navigation.FindPath(start, end); }
				catch (TimeoutException exception) { Debug.LogWarning(exception); }
				
				currentPathStart = start;
				currentPathEnd = end;

				pathView.ShowPath(start.ToUnityVector(), currentPath.Select(node => node.ToUnityVector()).ToList());
			}
		}

		private void ClearPath()
		{
			if (currentPath == null)
				return;

			pathView.Reset();
			currentPathStart = null;
			currentPathEnd = null;
			currentPath = null;
		}
		#endregion // Path
	}
}
