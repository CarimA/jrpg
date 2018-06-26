using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services
{
    public interface IData : IService
    {
        Dictionary<string, object> Maps { get; }
        Dictionary<string, string> Scripts { get; }
        Dictionary<string, object> PlayerData { get; }

        void LoadMaps();
        void LoadScripts();

        void SavePlayerData(string name);
        void LoadPlayerData(string name);
        void SetPlayerData(string key, object value);
        object GetPlayerData(string key);

        bool FileExists(string filepath);
        string GetSaveDirectory();
        string GetAbsoluteDirectory(string directory);
    }
}
