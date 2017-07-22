using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model
{
	[DataContract]
	[Serializable]
	public class Character
	{
		[DataMember]
		public Guid Id = Guid.NewGuid();

		[DataMember]
		public Guid OwnerId;
		public Player Owner;

		[DataMember(Name = nameof(CharacterClass))]
		public string CharacterClass_Key;
		public CharacterClass CharacterClass;

		[DataMember]
		public Vector2 Position;
		public event Action<Character, List<Vector2>> Moved;

		[DataMember]
		public int HealthPoints;
		public event Action<Character, int, int> HealthPointsChanged;
		public bool IsAlive { get { return HealthPoints > 0; } }
		public event Action<Character> Died;
		[DataMember]
		public int ActionPoints;

		internal void StartTurn()
		{
			ActionPoints = CharacterClass.ActionPoints;
		}

		public bool Move(List<Vector2> path)
		{
			if (path.Any() == false)
				return false;

			List<Vector2> pathTravelled = new List<Vector2>();
			Vector2 newPosition = Position;

			foreach (Vector2 pathNode in path)
			{
				int requiredActionPoints = CharacterClass.MoveSpeed;
				if (ActionPoints < requiredActionPoints)
					break;

				newPosition = pathNode;
				ActionPoints -= requiredActionPoints;
				pathTravelled.Add(pathNode);
			}
			
			Position = newPosition;
			Moved?.Invoke(this, pathTravelled);
			return true;
		}

		public bool Cast(Ability ability, Vector2 targetCenter, Func<Vector2, Vector2, IEnumerable<Character>> getCharactersInArea)
		{
			// Add half a tile to range to make a circle reaching to the tile end instead of the tile center
			if ((ActionPoints < ability.ActionPoints) || ((targetCenter - Position).Norm > ability.Range + 0.5f))
				return false;

			Vector2 bottomLeft = new Vector2(targetCenter.X - ability.TargetWidth / 2f + 0.05f, targetCenter.Y - ability.TargetHeight / 2f + 0.05f);
			Vector2 topRight = new Vector2(targetCenter.X + ability.TargetWidth / 2f - 0.05f, targetCenter.Y + ability.TargetHeight / 2f - 0.05f);
			List<Character> targetCollection = getCharactersInArea(bottomLeft, topRight).Where(target => IsTargetAllowed(ability, target)).ToList();

			if (ability.TargetRequired && (targetCollection.Any() == false))
				return false;

			foreach (Character currentTarget in targetCollection)
				currentTarget.ApplyAbility(this, ability);

			ActionPoints -= ability.ActionPoints;
			return true;
		}

		private void ApplyAbility(Character caster, Ability ability)
		{
			int oldValue = HealthPoints;
			HealthPoints -= ability.Power;
			if (HealthPoints <= 0)
				HealthPoints = 0;

			HealthPointsChanged?.Invoke(this, oldValue, HealthPoints);
			if (HealthPoints == 0)
			{
				Died?.Invoke(this);

				Moved = null;
				HealthPointsChanged = null;
				Died = null;
			}
		}

		private bool IsTargetAllowed(Ability ability, Character target)
		{
			if (ability.TargetTypes.Any() == false)
				return false;
			return ability.TargetTypes.All(type => IsTargetAllowed(type, target));
		}

		private bool IsTargetAllowed(TargetType type, Character target)
		{
			switch (type)
			{
				case TargetType.Self: return this == target;
				case TargetType.Allied: return Owner == target.Owner;
				case TargetType.Enemy: return Owner != target.Owner;
				default: throw new Exception("[Character] Unhandled target type value: " + type);
			}
		}
	}
}
