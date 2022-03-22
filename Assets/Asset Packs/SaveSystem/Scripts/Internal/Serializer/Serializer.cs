using Hson;
using Hson.Formatter;
using Hson.Serializer;
using Hson.Utility;
using SaveSystem.Settings;
using System.Collections.Generic;

namespace SaveSystem.Internal
{
    internal class Serializer
    {
        private SerializerSettings settings;
        private HsonSerializer hsonSerializer;
        private HsonFormatter hsonFormatter;

        private HashingHelper hashingHelper;
        private EncryptionHelper encryptionHelper;
        private CompressionHelper compressionHelper;

        #region CONSTRUCTOR
        public Serializer()
        {
            settings = SerializerSettings.Default;
            Initialize();
        }
        public Serializer(SerializerSettings settings)
        {
            this.settings = settings;
            Initialize();
        }

        private void Initialize()
        {
            #region Custom types
            HsonSerializeOptions serializeOptions = settings.HsonOptions.SerializeOptions;
            serializeOptions.CustomTypes = CustomTypeReflection.GetCustomTypes();
            #endregion

            #region Serializer and formatter
            hsonSerializer = new HsonSerializer(serializeOptions);
            hsonFormatter = new HsonFormatter(settings.HsonOptions.BuilderOptions);
            #endregion

            #region Helper scripts
            hashingHelper = new HashingHelper(settings.DataModificationSettings);
            encryptionHelper = new EncryptionHelper(settings.EncryptionSettings);
            compressionHelper = new CompressionHelper(settings.CompressionSettings);
            #endregion
        }
        #endregion

        #region REGION Hson root elements
        #region PRIVATE METHOD GetRootElementsFromString
        private List<HsonObject> GetRootElementsFromString(string existing)
        {
            if (!string.IsNullOrEmpty(existing))
            {
                try
                {
                    if (settings.EncryptionSettings.UseEncryption)
                        existing = encryptionHelper.Decrypt(existing);
                    if (settings.CompressionSettings.UseCompression)
                        existing = compressionHelper.Decompress(existing);
                }
                catch { throw; }

                string hash = hashingHelper.ExtractHash(existing);
                existing = existing.Remove(0, hash.Length);
                if (settings.DataModificationSettings.MakeDataImmutable)
                    hashingHelper.CheckHash(hash, existing);
            }
            try
            {
                return hsonFormatter.Parse(existing);
            }
            catch { throw; }
        }
        #endregion

        #region PRIVATE METHOD ConvertRootElementsToString
        private string ConvertRootElementsToString(List<HsonObject> rootElements)
        {
            string rootElementString = hsonFormatter.Build(rootElements);

            rootElementString = hashingHelper.ComputeHash(rootElementString) + rootElementString;
            if (settings.CompressionSettings.UseCompression)
                rootElementString = compressionHelper.Compress(rootElementString);
            if (settings.EncryptionSettings.UseEncryption)
                rootElementString = encryptionHelper.Encrypt(rootElementString);
            return rootElementString;
        }
        #endregion

        #region PRIVATE METHOD GetRootElement
        private HsonObject GetRootElement(string existing, string key)
        {
            try
            {
                List<HsonObject> rootElements = GetRootElementsFromString(existing);
                return HsonRootElementHelper.GetRootElement(rootElements, key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD HasRootElement
        public bool HasRootElement(string serialized, string key)
        {
            try
            {
                List<HsonObject> rootElements = GetRootElementsFromString(serialized);
                return HsonRootElementHelper.HasRootElement(rootElements, key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD RemoveRootElement
        public string RemoveRootElement(string serialized, string key)
        {
            try
            {
                List<HsonObject> rootElements = GetRootElementsFromString(serialized);
                HsonRootElementHelper.RemoveRootElement(rootElements, key);
                return ConvertRootElementsToString(rootElements);
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region METHOD Serialize
        public string Serialize(object value)
        {
            try
            {
                return Serialize("", "default", value);
            }
            catch { throw; }
        }

        public string Serialize(string existing, string key, object value)
        {
            //remove hash
            existing = existing.Remove(0, hashingHelper.ExtractHash(existing).Length);

            try
            {
                List<HsonObject> rootElements = GetRootElementsFromString(existing);
                HsonRootElementHelper.RemoveRootElement(rootElements, key);

                HsonObject newElement = HsonRootElementHelper.MakeRootElement(
                    key, hsonSerializer.Serialize(value)
                );
                rootElements.Add(newElement);

                return ConvertRootElementsToString(rootElements);
            }
            catch { throw; }
        }
        #endregion

        #region REGION Deserialize
        #region METHOD Deserialize
        public object Deserialize<T>(string serialized)
        {
            try
            {
                return Deserialize<T>(serialized, "default");
            }
            catch { throw; }
        }

        public object Deserialize<T>(string existing, string key)
        {
            try
            {
                HsonObject serialized = GetRootElement(existing, key);
                return hsonSerializer.Deserialize(typeof(T), serialized);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeserializeInto
        public void DeserializeInto<T>(string existing, T deserializeInto) where T : class
        {
            try
            {
                DeserializeInto(existing, "default", deserializeInto);
            }
            catch { throw; }
        }

        public void DeserializeInto<T>(string existing, string key, T deserializeInto) where T : class
        {
            try
            {
                HsonObject serialized = GetRootElement(existing, key);
                hsonSerializer.DeserializeInto(deserializeInto, serialized);
            }
            catch { throw; }
        }
        #endregion
        #endregion
    }
}
