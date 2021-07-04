using System.Collections.Generic;

namespace WebsiteCrawler.Extensions
{
    public static class DictionaryExtension
    {
        public static void CustomConcat<Tkey, TValue>(this Dictionary<Tkey, TValue> baseDict, Dictionary<Tkey, TValue> supplementDict)
        {
            foreach (var item in supplementDict)
                baseDict.TryAdd(item.Key, item.Value);
        }
    }
}
