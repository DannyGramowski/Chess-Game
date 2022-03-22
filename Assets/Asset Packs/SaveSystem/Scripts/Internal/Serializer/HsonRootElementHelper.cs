using Hson;
using System.Collections.Generic;

namespace SaveSystem.Internal
{
    internal static class HsonRootElementHelper
    {
        #region METHOD MakeRootElement
        public static HsonObject MakeRootElement(string key, HsonObject value)
        {
            HsonObjectDescription description = new HsonObjectDescription();
            description.AddElement("key", key);
            return new HsonObject(value, description);
        }
        #endregion

        #region METHOD GetRootElement
        public static HsonObject GetRootElement(List<HsonObject> objects, string key)
        {
            if (!HasRootElement(objects, key))
                throw new KeyNotFoundException("The key '" + key + "' could not be found in the root elements.");
            return objects.Find(
                x => x.Description.ContainsKey("key") &&
                x.Description.GetElement("key") == key);
        }
        #endregion

        #region METHOD HasRootElement
        public static bool HasRootElement(List<HsonObject> objects, string key)
        {
            return objects.Exists(
                x => x.Description.ContainsKey("key") &&
                x.Description.GetElement("key") == key);
        }
        #endregion

        #region METHOD RemoveRootElement
        public static void RemoveRootElement(List<HsonObject> objects, string key)
        {
            objects.RemoveAll(
                x => x.Description.ContainsKey("key") &&
                x.Description.GetElement("key") == key);
        }
        #endregion
    }
}
