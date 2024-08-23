using System;
using Newtonsoft.Json;

namespace ClassLibrary
{
    public static class JsonHelper
    {

        public static dynamic DeserializeObject(string jsonText)
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonText);
        }
        
    }
}