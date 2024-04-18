using Unity.Collections;
using UnityEditor;

namespace Tanks.Editor
{
	public static class CustomEditorCommands
	{
		[MenuItem("Customs/Commands/Generate Tags and Layers constants", priority = 101)]
		private static void LayerAndTagGenerate()
		{
			LayerAndTagGenerator.Generate();
		}
		[MenuItem("Customs/Jobs/Leak Detection With Stack Trace")]
		private static void LeakDetectionWithStackTrace()
		{
			NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
		}
		[MenuItem("Customs/Jobs/No Leak Detection")]
		private static void NoLeakDetection()
		{
			NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
		}
	}
}