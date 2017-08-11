using Overmind.Tactics.Model;
using Overmind.Tactics.Model.Abilities;
using Overmind.Tactics.UnityClient.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class CharacterView : MonoBehaviour
	{
		public Character Model;

		[SerializeField]
		private SpriteRenderer characterSprite;
		[SerializeField]
		private Transform healthBar;
		[SerializeField]
		private GameObject statusTextPrefab;
		[SerializeField]
		private GameObject projectilePrefab;

		public void UpdateFromModel()
		{
			name = String.Format("Character ({0}, {1})", Model.CharacterClass.Name, Model.Owner.Name);
			characterSprite.sprite = Resources.Load<Sprite>("Characters/" + Model.CharacterClass.CharacterSprite);
			transform.localPosition = Model.Position.ToUnityVector();
			healthBar.localScale = new Vector3(Model.HealthPoints == 0 ? 0 : (float)Model.HealthPoints / Model.CharacterClass.HealthPoints, 1, 1);
		}

		public void Start()
		{
			Model.Moved += Move;
			Model.HealthPointsChanged += OnHealthPointsChanged;
			Model.AbilityCast += OnAbilityCast;
			Model.Died += _ => Destroy(gameObject);
		}

		#region Movement

		private List<UnityEngine.Vector2> currentPath = new List<UnityEngine.Vector2>();
		private void Move(Character model, List<Model.Vector2> path)
		{
			bool isCoroutineRunning = currentPath.Any();
			currentPath.AddRange(path.Select(node => node.ToUnityVector()));
			if (isCoroutineRunning == false)
				StartCoroutine(FollowPath());
		}

		private IEnumerator FollowPath()
		{
			while (currentPath.Any())
			{
				Vector3 nextPosition = currentPath.First();
				nextPosition.z = transform.localPosition.z;
				transform.localPosition = nextPosition;
				currentPath.RemoveAt(0);

				yield return new WaitForSeconds(0.1f);
			}
		}

		#endregion // Movement

		private void OnHealthPointsChanged(Character model, int oldValue, int newValue)
		{
			healthBar.localScale = new Vector3(newValue == 0 ? 0 : (float)newValue / Model.CharacterClass.HealthPoints, 1, 1);

			// Status text is not a child so that it is not impacted by character move and death
			StatusText statusText = Instantiate(statusTextPrefab).GetComponent<StatusText>();
			statusText.transform.SetParent(transform.parent);
			statusText.transform.localPosition = transform.localPosition;

			float valueChange = newValue - oldValue;
			statusText.TextElement.text = valueChange.ToString();
			statusText.TextElement.color = valueChange >= 0 ? Color.green : Color.red;
		}

		private void OnAbilityCast(Character model, IAbility ability, Model.Vector2 target)
		{
			if (ability is ProjectileAbility)
			{
				ProjectileView projectile = GameObjectExtensions.Instantiate(projectilePrefab, transform.parent).GetComponent<ProjectileView>();
				projectile.Animator.runtimeAnimatorController
					= UnityContentProvider.LoadAsset<RuntimeAnimatorController>("Abilities/Ability_" + ability.Name + "_Projectile");
				projectile.transform.localPosition = model.Position.ToUnityVector();
				projectile.Target = target.ToUnityVector();
			}
		}
	}
}