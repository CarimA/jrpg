using JRPG.EntityComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Data
{
    public class PrototypeData : IData
    {
        public Dictionary<string, object> Maps { get; private set; }
        public Dictionary<string, string> Scripts { get; private set; }
        
        public Dictionary<string, object> PlayerData { get; private set; }

        public void Initialise()
        {
            Maps = new Dictionary<string, object>();
            Scripts = new Dictionary<string, string>();
            PlayerData = new Dictionary<string, object>();

            // load maps

            // load scripts
            string[] scripts = Directory.GetFiles("Content/Scripts");
            foreach (string script in scripts)
            {
                Scripts.Add(Path.GetFileNameWithoutExtension(script), File.ReadAllText(script));
            }

            // load saved entities and add to EC framework
            string dirEnt = GetAbsoluteDirectory("Entities");


            // load player data
            
        }

        public string GetSaveDirectory()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir = Path.Combine(dir, "PhotoVs");
            Directory.CreateDirectory(dir);
            return dir;
        }

        public string GetAbsoluteDirectory(string directory)
        {
            string dir = Path.Combine(GetSaveDirectory(), directory);
            Directory.CreateDirectory(dir);
            return dir;
        }

        public bool FileExists(string filepath)
        {
            return File.Exists(filepath);
        }
        
        public void LoadMaps()
        {
            throw new NotImplementedException();
        }

        public void LoadScripts()
        {
            throw new NotImplementedException();
        }

        public void LoadEntity()
        {
            throw new NotImplementedException();
        }

        public void SavePlayerData(string name)
        {
            throw new NotImplementedException();
        }

        public void LoadPlayerData(string name)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerData(string key, object value)
        {
            throw new NotImplementedException();
        }

        public object GetPlayerData(string key)
        {
            throw new NotImplementedException();
        }
        
    }
}
