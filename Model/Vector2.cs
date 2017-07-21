using System;
using System.Runtime.Serialization;

namespace Overmind.Tactics.Model
{
	[DataContract]
	[Serializable]
	public struct Vector2
	{
		public static readonly Vector2 Zero = new Vector2(0, 0);
		public static readonly Vector2 Up = new Vector2(0, 1);
		public static readonly Vector2 Right = new Vector2(1, 0);
		public static readonly Vector2 Down = new Vector2(0, -1);
		public static readonly Vector2 Left = new Vector2(-1, 0);

		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		[DataMember]
		public float X;
		[DataMember]
		public float Y;

		public override string ToString()
		{
			return String.Format("[{0}, {1}]", X, Y);
		}

		public float Norm { get { return Convert.ToSingle(Math.Sqrt(X * X + Y * Y)); } }

		/// <summary>Creates a normalized vector from this vector.</summary>
		public Vector2 Normalize()
		{
			return this / Norm;
		}

		public override bool Equals(object other)
		{
			return other is Vector2 && this == (Vector2)other;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + X.GetHashCode();
				hash = hash * 23 + Y.GetHashCode();
				return hash;
			}
		}

		#region Operators

		public static bool operator ==(Vector2 firstVector, Vector2 secondVector)
		{
			return (firstVector.X == secondVector.X) && (firstVector.Y == secondVector.Y);
		}

		public static bool operator !=(Vector2 firstVector, Vector2 secondVector)
		{
			return (firstVector == secondVector) == false;
		}

		public static Vector2 operator +(Vector2 firstVector, Vector2 secondVector)
		{
			return new Vector2(firstVector.X + secondVector.X, firstVector.Y + secondVector.Y);
		}

		public static Vector2 operator -(Vector2 vector)
		{
			return new Vector2( - vector.X, - vector.Y);
		}

		public static Vector2 operator -(Vector2 firstVector, Vector2 secondVector)
		{
			return new Vector2(firstVector.X - secondVector.X, firstVector.Y - secondVector.Y);
		}

		public static Vector2 operator *(Vector2 vector, float multiplier)
		{
			return new Vector2(vector.X * multiplier, vector.Y * multiplier);
		}

		public static Vector2 operator /(Vector2 vector, float divisor)
		{
			return new Vector2(vector.X / divisor, vector.Y / divisor);
		}

		#endregion
	}
}
