namespace Tanks.Legacy
{
	public static class LegacyCurveUtility
	{
		public static float CalcY(float x, float a, float b)
		{
			//return x * a; 
			var sqt = x * x;
			return sqt / (a * (sqt - x) + b);
		}
	}
}