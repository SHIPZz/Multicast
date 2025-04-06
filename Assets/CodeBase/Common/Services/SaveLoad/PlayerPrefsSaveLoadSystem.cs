using CodeBase.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CodeBase.Common.Services.SaveLoad
{
    public class PlayerPrefsSaveLoadSystem : ISaveLoadSystem
    {
        private const string DataKey = "Data";
        
        public void Save(ProgressData data)
        {
            string jsonData = JsonConvert.SerializeObject(data,Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            
            PlayerPrefs.SetString(DataKey, jsonData);
            PlayerPrefs.Save();
        }
        
        public UniTask<ProgressData> Load()
        {
            if (PlayerPrefs.HasKey(DataKey))
            {
                string jsonData = PlayerPrefs.GetString(DataKey);
                ProgressData data = JsonConvert.DeserializeObject<ProgressData>(jsonData);
                return UniTask.FromResult(data);
            }

            return UniTask.FromResult(new ProgressData());
        }

    }
}