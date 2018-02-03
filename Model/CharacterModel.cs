using Overmind.Tactics.Data;
using Overmind.Tactics.Model.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Tactics.Model
{
	public class CharacterModel
	{
		public CharacterModel(CharacterData characterData, CharacterClass characterClass, PlayerModel owner, bool assignFromClass)
		{
			this.characterData = characterData;
			this.CharacterClass = characterClass;
			this.Owner = owner;

			if (assignFromClass)
			{
				characterData.HealthPoints = characterClass.HealthPoints;
				characterData.ActionPoints = characterClass.ActionPoints;
			}
		}

		private readonly CharacterData characterData;
		public string Id { get { return characterData.Id; } }
		public CharacterClass CharacterClass { get; }
		public PlayerModel Owner { get; }

		public Vector2 Position { get { return characterData.Position; } set { characterData.Position = value; } }
		public event Action<CharacterModel, List<Vector2>> Moved;
		
		public int HealthPoints
		{
			get { return characterData.HealthPoints; }
			set
			{
				int oldValue = characterData.HealthPoints;
				characterData.HealthPoints = value;
				if (characterData.HealthPoints <= 0)
					characterData.HealthPoints = 0;

				HealthPointsChanged?.Invoke(this, oldValue, characterData.HealthPoints);
				if (characterData.HealthPoints == 0)
				{
					Died?.Invoke(this);

					Moved = null;
					HealthPointsChanged = null;
					AbilityCast = null;
					Died = null;
				}
			}
		}

		public event Action<CharacterModel, int, int> HealthPointsChanged;
		public bool IsAlive { get { return HealthPoints > 0; } }
		public event Action<CharacterModel> Died;
		
		public int ActionPoints
		{
			get { return characterData.ActionPoints; }
			set { characterData.ActionPoints = value; }
		}

		internal void StartTurn()
		{
			ActionPoints = CharacterClass.ActionPoints;
		}

		public bool Move(List<Vector2> path)
		{
			if (path.Any() == false)
				return false;

			int moveLimit = CharacterClass.MoveSpeed * ActionPoints;
			if (moveLimit == 0)
				return false;

			List<Vector2> pathTravelled = path.Take(moveLimit).ToList();
			ActionPoints -= (int)Math.Ceiling((float)pathTravelled.Count / (float)CharacterClass.MoveSpeed);
			Position = pathTravelled.Last();
			Moved?.Invoke(this, pathTravelled);
			return true;
		}

		public event Action<CharacterModel, IAbility, Vector2> AbilityCast;

		public bool Cast(ICharacterFinder characterFinder, IAbility ability, Vector2 targetPosition)
		{
			if (ActionPoints < ability.ActionPoints)
				return false;

			bool didCast = ability.Cast(characterFinder, this, targetPosition);
			if (didCast)
			{
				ActionPoints -= ability.ActionPoints;
				AbilityCast?.Invoke(this, ability, targetPosition);
			}

			return didCast;
		}
	}
}
