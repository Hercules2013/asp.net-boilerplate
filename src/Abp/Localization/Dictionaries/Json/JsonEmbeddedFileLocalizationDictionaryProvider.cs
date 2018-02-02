﻿using System.Reflection;

namespace Abp.Localization.Dictionaries.Json
{
    /// <summary>
    /// Provides localization dictionaries from JSON files embedded into an <see cref="Assembly"/>.
    /// </summary>
    public class JsonEmbeddedFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly Assembly _assembly;
        private readonly string _rootNamespace;

        /// <summary>
        /// Creates a new <see cref="JsonEmbeddedFileLocalizationDictionaryProvider"/> object.
        /// </summary>
        /// <param name="assembly">Assembly that contains embedded json files</param>
        /// <param name="rootNamespace">
        /// <para>
        /// Namespace of the embedded json dictionary files
        /// </para>
        /// <para>
        /// Notice : Json folder name is different from Xml folder name.
        /// </para>
        /// <para>
        /// You must name it like this : Json**** and Xml****; Do not name : ****Json and ****Xml
        /// </para>
        /// </param>
        public JsonEmbeddedFileLocalizationDictionaryProvider(Assembly assembly, string rootNamespace)
        {
            _assembly = assembly;
            _rootNamespace = rootNamespace;
        }

        public override void Initialize(string sourceName)
        {
            var resourceNames = _assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(_rootNamespace))
                {
                    using (var stream = _assembly.GetManifestResourceStream(resourceName))
                    {
                        var jsonString = Utf8Helper.ReadStringFromStream(stream);

                        CommonInitialize(() => CreateJsonLocalizationDictionary(jsonString), resourceName, sourceName, ".json");
                    }
                }
            }
        }

        protected virtual JsonLocalizationDictionary CreateJsonLocalizationDictionary(string jsonString)
        {
            return JsonLocalizationDictionary.BuildFromJsonString(jsonString);
        }
    }
}