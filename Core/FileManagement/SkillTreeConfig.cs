using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillTree.Core.FileManagement
{
    [Serializable]
    public class SkillConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UnityEngine.KeyCode MenuHotkey = UnityEngine.KeyCode.C;
    }
}
