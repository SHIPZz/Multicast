using CodeBase.Data;
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
        
        public ProgressData Load()
        {
            if (PlayerPrefs.HasKey(DataKey))
            {
                string jsonData = PlayerPrefs.GetString(DataKey);
                return JsonConvert.DeserializeObject<ProgressData>(jsonData);
            }
            
            return new ProgressData();
        }
    }
}