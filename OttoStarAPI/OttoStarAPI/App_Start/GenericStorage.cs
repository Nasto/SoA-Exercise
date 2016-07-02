using OttoStar.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OttoStar
{
    public class GenericStorage
    {
        private string _filePath;

        public GenericStorage()
        {
            var webAppsHome = Environment.GetEnvironmentVariable("HOME")?.ToString();
            if (String.IsNullOrEmpty(webAppsHome))
            {
                _filePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\";
            }
            else
            {
                _filePath = webAppsHome + "\\site\\wwwroot\\";
            }
        }

        public async Task Save(IEnumerable<StarSign> target, string filename)
        {
            var json = JsonConvert.SerializeObject(target);
            File.WriteAllText(_filePath + filename, json);
        }

        public async Task<IEnumerable<StarSign>> Get(string filename)
        {
            var starSignsText = String.Empty;
            if (File.Exists(_filePath + filename))
            {
                starSignsText = File.ReadAllText(_filePath + filename);
            }

            var starSigns = JsonConvert.DeserializeObject<StarSign[]>(starSignsText);
            return starSigns;
        }
    }
}
