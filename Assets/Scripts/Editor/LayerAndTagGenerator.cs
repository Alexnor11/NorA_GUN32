using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Tanks.Editor
{
    /// <summary>
    /// Класс для автогенерации констант слоев и тегов в проекте
    /// </summary>
	public class LayerAndTagGenerator
	{
#region Constants

        private static StringBuilder _code;

        private const string c_LayerSpawnPoint = "%LAYERS_COUNT_SPAWN%";
        private const string c_TagSpawnPoint = "%TAGS_COUNT_SPAWN%";

        private const string c_NamespacePoint = "%NAMESPACE_POINT%";
        private const string c_LayerEnumPoint = "%LAYER_NAME_POINT%";
        private const string c_LayerNameValuePoint = "%LAYER_PROPERTY_VALUE_POINT%";
        private const string c_TagEnumPoint = "%TAG_NAME_POINT%";
        private const string c_TagEnumValuePoint = "%TAG_ENUM_VALUE_POINT%";

        private const string c_TemplatePostFix = "Template.txt";

#endregion
        
        private const string _baseNamespace = "Tanks";

        private const string _generatPath = "Scripts/Generated";
        private const string _fileName = "UnityConstants";


        private static (int, string)[] _layers;

        internal static void Generate()
        {
            //Find Template
            var path = (string)null;
            var name = string.Concat(_fileName, c_TemplatePostFix);

            var guids = AssetDatabase.FindAssets("t:TextAsset");
            foreach(var guid in guids)
            {
                path = AssetDatabase.GUIDToAssetPath(guid);
                if(path.Contains(name)) break;
            }

            if (path == null) throw new System.ApplicationException($"<b>[{nameof(LayerAndTagGenerator)}]</b> Can't find template asset in project by name <{name}>");

            _code = new StringBuilder();
            using (var stream = File.OpenRead(path.Replace("Assets", Application.dataPath)))
            {
                using(var reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        LineParse(reader.ReadLine());
					}
				}
			}

            //Create File
            path = Path.Combine(Application.dataPath, _generatPath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, _fileName + ".cs");

            if (!File.Exists(path)) File.Create(path).Close();

            var result = _code.ToString();
            File.WriteAllText(path, result);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void LineParse(string line)
        {
            //Create Layers
            if(line.Contains(c_LayerSpawnPoint))
            {
                Layer(line.Replace(c_LayerSpawnPoint, ""));
                return;
            }
            //Create Tags
            else if (line.Contains(c_TagSpawnPoint))
            {
                Tags(line.Replace(c_TagSpawnPoint, ""));
                return;
            }

            //Replace namespace
            _code.AppendLine(line.Replace(c_NamespacePoint, _baseNamespace));
        }

        private static void Layer(string line)
        {
            for(int i = 0; i < _layers.Length; i++)
            {
                var copy = line.Substring(0);
                _code.AppendLine(copy.Replace(c_LayerEnumPoint, _layers[i].Item2).Replace(c_LayerNameValuePoint, _layers[i].Item1.ToString()));
			}
		}

        private static void Tags(string line)
        {
            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            for (int i = 0; i < tags.Length; i++)
            {
                var copy = line.Clone() as string;
                _code.AppendLine(copy.Replace(c_TagEnumPoint, tags[i]).Replace(c_TagEnumValuePoint, i.ToString()));
            }
        }

        static LayerAndTagGenerator()
        {
            //---Configuration---
            var layers = UnityEditorInternal.InternalEditorUtility.layers;
            var count = layers.Length;

            //---Configuration---
            _layers = new (int, string)[count];
            for (int i = 0; i < count; i++)
            {
                _layers[i] = (LayerMask.NameToLayer(layers[i]), layers[i].Replace(" ", ""));
            }
        }
    }
}