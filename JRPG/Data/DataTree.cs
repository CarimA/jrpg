using JRPG.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.Data
{
    public class DataTree
    {
        protected DataTree _parent;
        protected object _children;
        protected string _name { get; set; }
        
        public object Children { get => _children; }

        public DataTree(DataTree parent, string name)
        {
            _children = new Dictionary<string, DataTree>();
            _name = name;
            _parent = parent;
        }

        public DataTree() : this(null, "_")
        {
            _children = new Dictionary<string, DataTree>();
        }
        
        public DataTree(DataTree parent, string name, string value) : this(parent, name)
        {
            _children = value;
        }

        public DataTree AddChild(string key, DataTree node)
        {
            if (_children is Dictionary<string, DataTree>)
            {
                (_children as Dictionary<string, DataTree>).Add(key, node);
            }
            return node;
        }
                
        public DataTree GetParent() => _parent;
        public string GetName() => _name;

        private static DataTree parse(DataTree parent, string key, dynamic data)
        {
            if (data is string)
            {
                DataTree text = new DataTree(parent, data, (data as string));
                return text;
            }
            else
            {
                DataTree tree = new DataTree(parent, key);
                var dict = (Dictionary<string, object>)data.ToObject<Dictionary<string, object>>();
                foreach (var d in dict)
                {
                    tree.AddChild(d.Key, parse(tree, d.Key, d.Value));
                }
                return tree;
            }
        }

        public static DataTree Parse(string json, string filename = "")
        {
            if (!json.StartsWith("{"))
                json = "{" + json + "}";

            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
            return parse(null, filename, data);
        }

        public static DataTree Load(string file)
        {
            if (!Path.HasExtension(file))
                file += ".json";

            if (!File.Exists(file))
                throw new FileNotFoundException("Data not found", file);

            string data = File.ReadAllText(file);
            return Parse(data, file);
        }

        public Dictionary<string, DataTree> GetChildren()
        {
            return (_children as Dictionary<string, DataTree>);
        }

        public DataTree Get(string key)
        {
            if (_children is Dictionary<string, DataTree>)
            {
                var children = (_children as Dictionary<string, DataTree>);
                return children[key];
            }
            return null;
        }

        public string GetString()
        {
            return (_children is string) ? (string)_children : string.Empty;
        }

        public bool GetBool()
        {
            switch (GetString().ToLowerInvariant())
            {
                case "true":
                case "on":
                case "yes":
                    return true;
                default:
                    return false;
            }
        }

        public float GetFloat()
        {
            if (float.TryParse(GetString(), out float result))
            {
                return result;
            }
            return 0;
        }

        public int GetInt()
        {
            if (int.TryParse(GetString(), out int result))
            {
                return result;
            }
            return 0;
        }
    }
}