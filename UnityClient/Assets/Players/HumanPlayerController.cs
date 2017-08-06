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
			UserInterface.AbilityPanel.AbilityButtonClick += PickTarget;

			Player.Model.TurnEnded += _ => ClearCurrentCommand();
		}

		public void Update()
		{
			if (Game.ActivePlayer != Player)
				return;

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

			if (Input.GetMouseButtonUp(0))
			{
				Vector2 hitOrigin = Camera.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(hitOrigin, Vector2.zero);
				Player.Selection = hit.collider != null ? hit.collider.GetComponentInParent<CharacterView>() : null;
			}

			if ((Player.Selection != null) && (Player.Selection.Model.Owner == Player.Model))
			{
				if (Input.GetMouseButton(1))
				{
					ShowPath(Player.Selection.Model.Position, ((Vector2)Camera.ScreenToWorldPoint(Input.mousePosition)).ToModelVector());
				}

				if (Input.GetMouseButtonUp(1))
				{
					Vector2 hitOrigin = Camera.ScreenToWorldPoint(Input.mousePosition);
					RaycastHit2D hit = Physics2D.Raycast(hitOrigin, Vector2.zero);
					if (hit.collider != null)
					{
						Character selection = Player.Selection.Model;
						CharacterView target = hit.collider.GetComponentInParent<CharacterView>();

						if (target == null)
							Game.Model.ExecuteCommand(new MoveCommand() { Character = selection, CharacterId = selection.Id, Path = currentPath });
						else
						{
							IAbility defaultAbility = Player.Selection.Model.CharacterClass.DefaultAbility;
							if (defaultAbility != null)
							{
								Game.Model.ExecuteCommand(new CastAbilityCommand()
								{
									Character = selection,
									CharacterId = selection.Id,
									Ability = defaultAbility,
									AbilityName = defaultAbility.Name,
									Target = abilityCastView.GetTargetPosition(hitOrigin, defaultAbility).ToModelVector(),
								});
							}
						}
					}

					ClearPath();
				}
			}
		}

		private void UpdateForCommand()
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			Character selection = Player.Selection.Model;
			Vector2 targetPosition = abilityCastView.TargetPosition;

			if (currentAbility == null)
				ShowPath(selection.Position, targetPosition.ToModelVector());

			if (Input.GetMouseButtonUp(0))
			{
				IGameCommand command;
				if (currentAbility == null)
				{
					command = new MoveCommand()
					{
						Character = selection,
						CharacterId = selection.Id,
						Path = currentPath,
					};
				}
				else
				{
					command = new CastAbilityCommand()
					{
						Character = selection,
						CharacterId = selection.Id,
						Ability = currentAbility,
						AbilityName = currentAbility.Name,
						Target = targetPosition.ToModelVector(),
					};
				}

				Game.Model.ExecuteCommand(command);
				ClearCurrentCommand();
			}

			if (Input.GetMouseButtonUp(1))
				ClearCurrentCommand();
		}

		private void PickTarget(Character caster, IAbility ability)
		{
			ClearCurrentCommand();

			if ((ability != null) && (caster.ActionPoints < ability.ActionPoints))
			{
				UserInterface.GameMessageView.AddMessage("Not enough action points");
				return;
			}

			isPickingTarget = true;
			currentAbility = ability;
			abilityCastView.Change(true, Player.Selection.Model, ability);
		}

		private void ClearCurrentCommand()
		{
			isPickingTarget = false;
			currentAbility = null;
			abilityCastView.Change(false, null, null);
			ClearPath();
		}

		#region Path
		[SerializeField]
		private PathView pathView;
		private Model.Vector2? currentPathStart;
		private Model.Vector2? currentPathEnd;
		private List<Model.Vector2> currentPath;

		private void ShowPath(Model.Vector2 start, Model.Vector2 end)
		{
			start = new Model.Vector2(Mathf.Round(start.X), Mathf.Round(start.Y));
			end = new Model.Vector2(Mathf.Round(end.X),Mathf.Round(end.Y));

			if ((currentPath == null) || (currentPathStart != start) || (currentPathEnd != end))
			{
				// Debug.LogFormat(this, "[HumanPlayerController] FindPath (Start: {0}, End: {1})", start, end);
				currentPath = new List<Model.Vector2>();
				try { currentPath = Game.Model.ActiveState.Navigation.FindPath(start, end); }
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
