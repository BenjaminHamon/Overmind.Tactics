using Overmind.Tactics.Model;
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
		private Ability currentAbility;
		
		[SerializeField]
		private GameUserInterface UserInterface;
		
		[SerializeField]
		private GameObject targetIndicator;
		[SerializeField]
		private GameObject abilityRangeIndicatorPrefab;
		[SerializeField]
		private Transform abilityRangeIndicatorGroup;

		public void Start()
		{
			UserInterface.Game = Game.Model;
			UserInterface.Player = Player;

			UserInterface.EndTurnButton.onClick.AddListener(() => Game.Model.ExecuteCommand(new EndTurnCommand() { Player = Player.Model, PlayerId = Player.Model.Id }));
			UserInterface.SelectPreviousCharacterButton.onClick.AddListener(() => Player.SelectPrevious());
			UserInterface.SelectNextCharacterButton.onClick.AddListener(() => Player.SelectNext());
			UserInterface.AbilityPanel.AbilityButtonClick += PickTarget;
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
			targetIndicator.SetActive(false);
			
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			if (Input.GetMouseButtonUp(0))
			{
				Vector2 hitOrigin = Camera.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(hitOrigin, Vector2.zero);
				Player.Selection = hit.collider != null ? hit.collider.GetComponent<CharacterView>() : null;
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
						CharacterView target = hit.collider.GetComponent<CharacterView>();

						if (target == null)
							Game.Model.ExecuteCommand(new MoveCommand() { Character = selection, CharacterId = selection.Id, Path = currentPath });
						else
						{
							Ability defaultAbility = Player.Selection.Model.CharacterClass.DefaultAbility;
							if (defaultAbility != null)
							{
								Game.Model.ExecuteCommand(new CastAbilityCommand()
								{
									Character = selection,
									CharacterId = selection.Id,
									Ability = defaultAbility,
									AbilityName = defaultAbility.Name,
									Target = RoundTargetPosition(hitOrigin, defaultAbility).ToModelVector(),
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
			{
				targetIndicator.SetActive(false);
				return;
			}

			Character selection = Player.Selection.Model;
			Vector2 targetPosition = RoundTargetPosition(Camera.ScreenToWorldPoint(Input.mousePosition), currentAbility);
			if ((Vector2)targetIndicator.transform.localPosition != targetPosition)
			{
				targetIndicator.transform.localPosition = targetPosition;
				targetIndicator.transform.localEulerAngles = new Vector3(0, 0, currentAbility.GetRotation(selection.Position, targetPosition.ToModelVector()));
			}
			targetIndicator.SetActive(true);

			if (currentAbility == null)
				ShowPath(selection.Position, targetPosition.ToModelVector());

			if (Input.GetMouseButtonUp(0))
			{
				Game.Model.ExecuteCommand(new CastAbilityCommand()
				{
					Character = selection,
					CharacterId = selection.Id,
					Ability = currentAbility,
					AbilityName = currentAbility.Name,
					Target = targetPosition.ToModelVector(),
				});
				ClearCurrentCommand();
			}

			if (Input.GetMouseButtonUp(1))
				ClearCurrentCommand();
		}

		private void ClearCurrentCommand()
		{
			isPickingTarget = false;
			currentAbility = null;
			ClearPath();
			ClearAbilityRange();
		}

		#region Path
		[SerializeField]
		private PathView pathView;
		private Model.Vector2? currentPathStart;
		private Model.Vector2? currentPathEnd;
		private List<Model.Vector2> currentPath;

		private void ShowPath(Model.Vector2 start, Model.Vector2 end)
		{
			start = new Model.Vector2(Convert.ToSingle(Math.Round(start.X)), Convert.ToSingle(Math.Round(start.Y)));
			end = new Model.Vector2(Convert.ToSingle(Math.Round(end.X)), Convert.ToSingle(Math.Round(end.Y)));

			if ((currentPath == null) || (currentPathStart != start) || (currentPathEnd != end))
			{
				// Debug.LogFormat(this, "[HumanPlayerController] FindPath (Start: {0}, End: {1})", start, end);
				currentPath = Game.Model.ActiveState.Navigation.FindPath(start, end);
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

		public void PickTarget(Character caster, Ability ability)
		{
			ClearCurrentCommand();

			if ((ability != null) && (caster.ActionPoints < ability.ActionPoints))
			{
				UserInterface.GameMessageView.AddMessage("Not enough action points");
				return;
			}

			isPickingTarget = true;
			currentAbility = ability;

			if (ability == null)
			{
				targetIndicator.transform.localScale = new Vector2(1, 1);
			}
			else
			{
				targetIndicator.transform.localScale = new Vector2(ability.TargetWidth, ability.TargetHeight);
				DrawAbilityRange(caster.Position.ToUnityVector(), ability.Range);
			}
		}

		private Vector2 RoundTargetPosition(Vector2 targetPosition, Ability ability)
		{
			if (ability == null)
			{
				targetPosition.x = Mathf.Round(targetPosition.x);
				targetPosition.y = Mathf.Round(targetPosition.y);
			}
			else
			{
				// Center the target area on the current tile or the between tiles depending on the area size
				targetPosition.x = ability.TargetWidth % 2 != 0 ? Mathf.Round(targetPosition.x) : Mathf.Floor(targetPosition.x) + 0.5f;
				targetPosition.y = ability.TargetHeight % 2 != 0 ? Mathf.Round(targetPosition.y) : Mathf.Floor(targetPosition.y) + 0.5f;
			}

			return targetPosition;
		}

		// Draw the ability range as a tiled circle
		private void DrawAbilityRange(Vector2 origin, int range)
		{
			Vector2 currentPosition = new Vector2(1, -range);
			while (currentPosition.y <= range)
			{
				Instantiate(abilityRangeIndicatorPrefab, new Vector2(0, currentPosition.y), Quaternion.identity, abilityRangeIndicatorGroup);

				currentPosition.x = 1;
				while (currentPosition.sqrMagnitude <= range * range)
				{
					Instantiate(abilityRangeIndicatorPrefab, currentPosition, Quaternion.identity, abilityRangeIndicatorGroup);
					Instantiate(abilityRangeIndicatorPrefab, new Vector2(-currentPosition.x, currentPosition.y), Quaternion.identity, abilityRangeIndicatorGroup);
					currentPosition.x += 1;
				}

				currentPosition.y += 1;
			}

			abilityRangeIndicatorGroup.transform.localPosition = origin;
		}

		private void ClearAbilityRange()
		{
			List<GameObject> children = abilityRangeIndicatorGroup.Cast<Transform>().Select(t => t.gameObject).ToList();
			foreach (GameObject child in children)
				Destroy(child);

			abilityRangeIndicatorGroup.transform.localPosition = Vector3.zero;
		}
	}
}
