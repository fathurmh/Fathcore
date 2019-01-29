using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fathcore.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Determine the json object is a valid json
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Returns boolean value</returns>
        public static bool IsValidJson(this string text)
        {
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
                (text.StartsWith("[") && text.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(text);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
