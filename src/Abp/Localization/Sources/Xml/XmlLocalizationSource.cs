﻿using System.IO;
using System.Reflection;
using Abp.Dependency;

namespace Abp.Localization.Sources.Xml
{
    /// <summary>
    /// XML based localization source.
    /// It uses XML files to read localized strings.
    /// </summary>
    public class XmlLocalizationSource : DictionaryBasedLocalizationSource, ISingletonDependency
    {
        internal static string RootDirectoryOfApplication { get; set; } //TODO: Find a better way of passing root directory

        private readonly ILocalizationDictionaryProvider _dictionaryProvider;

        static XmlLocalizationSource()
        {
            RootDirectoryOfApplication = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Creates an Xml based localization source.
        /// </summary>
        /// <param name="name">Unique Name of the source</param>
        /// <param name="directoryPath">Directory path of the localization XML files</param>
        public XmlLocalizationSource(string name, string directoryPath)
            :this(name, new XmlFileLocalizationDictionaryProvider(directoryPath))
        {

        }

        /// <summary>
        /// Creates an Xml based localization source.
        /// </summary>
        /// <param name="name">Unique Name of the source</param>
        /// <param name="dictionaryProvider">An object to get dictionaries</param>
        public XmlLocalizationSource(string name, ILocalizationDictionaryProvider dictionaryProvider)
            : base(name)
        {
            _dictionaryProvider = dictionaryProvider;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            _dictionaryProvider.AddDictionariesToLocalizationSource(this);
        }
    }
}
