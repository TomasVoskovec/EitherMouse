using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EitherMouse
{
    class FileManager
    {

        private JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public List<Device> LoadProfiles()
        {
            string path = @"../../Devices.json";
            List<Device> jsonread = new List<Device>();
            try
            {
                string jsonFromFile = File.ReadAllText(path);
                jsonread = JsonConvert.DeserializeObject<List<Device>>(jsonFromFile, settings);
            }
            catch
            {
                throw;
            }
            return jsonread;

        }

        public void SaveProfiles(List<Device> Profiles)
        {
            string path = @"../../Devices.json";
            string jsonToFilet = JsonConvert.SerializeObject(Profiles, settings);
            File.WriteAllText(path, jsonToFilet);
        }
    }
}
