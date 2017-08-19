namespace Overmind.Tactics.UnityClient.Unity
{
	public static class ModelExtensions
	{
		public static UnityEngine.Vector2 ToUnityVector(this Data.Vector2 vector)
		{
			return new UnityEngine.Vector2(vector.X, vector.Y);
		}

		public static Data.Vector2 ToModelVector(this UnityEngine.Vector2 vector)
		{
			return new Data.Vector2(vector.x, vector.y);
		}
	}
}
