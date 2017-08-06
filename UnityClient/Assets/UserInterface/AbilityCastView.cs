using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using Overmind.Tactics.UnityClient.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;

namespace Overmind.Tactics.UnityClient.UserInterface
{
	public class AbilityCastView : MonoBehaviour
	{
		[SerializeField]
		private new Camera camera;
		[SerializeField]
		private Transform targetArea;
		[SerializeField]
		private LineRenderer targetLine;
		[SerializeField]
		private Transform rangeGroup;
		[SerializeField]
		private GameObject rangePrefab;
		[SerializeField]
		private Color inRangeColor;
		[SerializeField]
		private Color outOfRangeColor;

		private Character caster;
		private IAbility ability;

		public Vector2 TargetPosition { get; private set; }

		public void Change(bool enabled, Character caster, IAbility ability)
		{
			targetArea.gameObject.SetActive(false);
			targetLine.gameObject.SetActive(false);
			ClearAbilityRange();

			this.enabled = enabled;
			this.caster = caster;
			this.ability = ability;

			if (enabled == false)
				return;

			Vector2 casterPosition = caster.Position.ToUnityVector();
			targetArea.localScale = new Vector2(1, 1);
			targetLine.SetPosition(0, casterPosition);

			if (ability is AreaAbility)
			{
				AreaAbility areaAbility = (AreaAbility)ability;
				targetArea.localScale = new Vector2(areaAbility.TargetWidth, areaAbility.TargetHeight);
				DrawAbilityRange(casterPosition, ability.Range);
			}
			else if (ability is ProjectileAbility)
				DrawAbilityRange(casterPosition, ability.Range);
		}

		public void Update()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				targetArea.gameObject.SetActive(false);
				targetLine.gameObject.SetActive(false);
				return;
			}

			Vector2 targetPosition = AdjustTargetPosition(camera.ScreenToWorldPoint(Input.mousePosition), ability);
			if (this.TargetPosition != targetPosition)
			{
				targetArea.localPosition = targetPosition;
				targetArea.localEulerAngles = new Vector3(0, 0, ability == null ? 0 : ability.GetRotation(caster.Position, targetPosition.ToModelVector()));

				if (ability is ProjectileAbility)
				{
					targetLine.SetPosition(1, targetPosition);
					targetLine.gameObject.SetActive(true);
				}

				if ((caster.Position - targetPosition.ToModelVector()).Norm <= ability.Range)
				{
					targetArea.GetComponent<SpriteRenderer>().color = inRangeColor;
				}
				else
				{
					targetArea.GetComponent<SpriteRenderer>().color = outOfRangeColor;
					targetLine.gameObject.SetActive(false);
				}

				this.TargetPosition = targetPosition;
			}

			targetArea.gameObject.SetActive(true);
		}

		public Vector2 AdjustTargetPosition(Vector2 targetPosition, IAbility ability)
		{
			if (ability != null)
			{
				if (ability is AreaAbility)
				{
					AreaAbility areaAbility = (AreaAbility)ability;
					return new Vector2(
						areaAbility.TargetWidth % 2 != 0 ? Mathf.Round(targetPosition.x) : Mathf.Floor(targetPosition.x) + 0.5f,
						areaAbility.TargetHeight % 2 != 0 ? Mathf.Round(targetPosition.y) : Mathf.Floor(targetPosition.y) + 0.5f);
				}
			}
			
			return GameView.GetTilePosition(targetPosition);
		}

		// Draw the ability range as a tiled circle
		private void DrawAbilityRange(Vector2 origin, int range)
		{
			Vector2 currentPosition = new Vector2(1, -range);
			while (currentPosition.y <= range)
			{
				Instantiate(rangePrefab, new Vector2(0, currentPosition.y), Quaternion.identity, rangeGroup);

				currentPosition.x = 1;
				while (currentPosition.sqrMagnitude <= range * range)
				{
					Instantiate(rangePrefab, currentPosition, Quaternion.identity, rangeGroup);
					Instantiate(rangePrefab, new Vector2(-currentPosition.x, currentPosition.y), Quaternion.identity, rangeGroup);
					currentPosition.x += 1;
				}

				currentPosition.y += 1;
			}

			rangeGroup.transform.localPosition = origin;
		}

		private void ClearAbilityRange()
		{
			rangeGroup.DestroyAllChildren();
			rangeGroup.transform.localPosition = Vector3.zero;
		}
	}
}
