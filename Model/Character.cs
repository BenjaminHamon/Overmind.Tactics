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
		public string Id = Guid.NewGuid().ToString();

		[DataMember]
		public string OwnerId;
		public Player Owner;

		[DataMember(Name = nameof(CharacterClass))]
		public string CharacterClass_Key;
		public CharacterClass CharacterClass;

		[DataMember]
		public Vector2 Position;
		public event Action<Character, List<Vector2>> Moved;

		private int healthPointsField;
		[DataMember(EmitDefaultValue = false)]
		public int HealthPoints
		{
			get { return healthPointsField; }
			set
			{
				int oldValue = healthPointsField;
				healthPointsField = value;
				if (healthPointsField <= 0)
					healthPointsField = 0;

				HealthPointsChanged?.Invoke(this, oldValue, healthPointsField);
				if (healthPointsField == 0)
				{
					Died?.Invoke(this);

					Moved = null;
					HealthPointsChanged = null;
					Died = null;
				}
			}
		}

		public event Action<Character, int, int> HealthPointsChanged;
		public bool IsAlive { get { return HealthPoints > 0; } }
		public event Action<Character> Died;

		[DataMember(EmitDefaultValue = false)]
		public int ActionPoints;
		
		internal void Initialize()
		{
			healthPointsField = CharacterClass.HealthPoints;
			ActionPoints = CharacterClass.ActionPoints;
		}

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
	}
}
