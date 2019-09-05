using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CreateAssetMenu]
    [ExecuteAlways]
    public class OscMapParser : ScriptableObject
    {
        public static OscMapParser instance;
        
        public static readonly Dictionary<Regex, Type> InputPathRegexToEventType = new Dictionary<Regex, Type>();
        
#pragma warning disable 649
        [SerializeField] 
        ResolumeEventMetaData[] m_PathMetaData;
#pragma warning restore 649
        
#if UNITY_EDITOR_WIN
        internal const string DefaultAvenuePath = "\\Documents\\Resolume Avenue\\Shortcuts\\OSC\\Default.xml";
        internal const string DefaultArenaPath = "\\Documents\\Resolume Arena\\Shortcuts\\OSC\\Default.xml";
#endif

        const string k_VersionInfoNodeName = "versionInfo";
        const string k_SubTargetNodeName = "Subtarget";
        const string k_ShortCut = "Shortcut";
        const string k_ShortCutPath = "ShortcutPath";

        XmlReaderSettings m_XmlSettings;
        XmlReader m_Reader;
        
        List<ResolumeOscShortcut> m_Shortcuts;

        ResolumeOscShortcut m_CurrentShortcut;
        ResolumeOscMap m_Map;
        ResolumeVersion m_Version;

        RegexTypeMapper m_RegexToTypeMapper = new RegexTypeMapper();
        
        public string OutputPath { get; set; }

        public static OscMapParser LoadAsset()
        {
            var parserGuids = AssetDatabase.FindAssets("t: OscMapParser");
            var parserPath = AssetDatabase.GUIDToAssetPath(parserGuids[0]);
            return AssetDatabase.LoadAssetAtPath<OscMapParser>(parserPath);
        }

        void OnEnable()
        {
            instance = this;
            m_XmlSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            m_Shortcuts = new List<ResolumeOscShortcut>();
            GatherTypeMetaData();
            
        }

        public void ParseDefaultFile()
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = userPath + DefaultAvenuePath;
            ParseFile(path);
        }

        public void ParseFile(string filePath)
        {
            m_Shortcuts.Clear();
            GatherTypeMetaData();
            
            m_Reader = XmlReader.Create(filePath, m_XmlSettings);
            m_Reader.MoveToContent();

            while (m_Reader.Read()) 
            {
                switch (m_Reader.NodeType) 
                {
                    case XmlNodeType.Element:
                        HandleNodeByName();
                        break;
                    case XmlNodeType.EndElement:
                        HandleEndElementByName();
                        break;
                } 
            }

            if (m_Shortcuts.Count == 0)
                return;


            foreach (var shortcut in m_Shortcuts)
            {
                var inPath = shortcut.Input.Path;
                if (!m_RegexToTypeMapper.Process(inPath, out var typeForShortcut))
                    continue;

                shortcut.TypeName = typeForShortcut.Name;
            }

            Debug.Log($"{m_Shortcuts.Count} Resolume OSC shortcuts found in map");
            CreateAsset();
        }

        void CreateAsset()
        {
            m_Map = CreateInstance<ResolumeOscMap>();
            m_Map.Version = m_Version;

            m_Map.Shortcuts.Clear();
            foreach (var shortcut in m_Shortcuts)
            {
                m_Map.Shortcuts.Add(shortcut);
            }
            
            m_Map.GroupSubTargets();
            
            // sort by output path label
            m_Map.Shortcuts.Sort();

            AssetDatabase.CreateAsset(m_Map, OutputPath);
        }

        void HandleNodeByName()
        {
            switch (m_Reader.Name)
            {
                case k_VersionInfoNodeName:
                    m_Version = ParseVersion();
                    break;
                case k_ShortCut:
                    m_CurrentShortcut = NewShortcut();
                    break;
                case k_ShortCutPath:
                    ParseShortcutPath();
                    break;
                case k_SubTargetNodeName:
                    // on initial parsing, we don't group subtargets that we find in multiple Shortcut nodes
                    m_CurrentShortcut.SubTargets = new[] { ParseSubTarget() };
                    break;
            }
        }

        ResolumeVersion ParseVersion()
        {
            int.TryParse(m_Reader.GetAttribute("majorVersion"), out var major);
            int.TryParse(m_Reader.GetAttribute("minorVersion"), out var minor);
            int.TryParse(m_Reader.GetAttribute("microVersion"), out var micro);
            
            return new ResolumeVersion()
            {
                Major = major,
                Minor = minor,
                Micro = micro
            };
        }

        SubTarget ParseSubTarget()
        {
            var typeString = m_Reader.GetAttribute("type");
            var optionString = m_Reader.GetAttribute("optionIndex");
            if (string.IsNullOrWhiteSpace(typeString) || string.IsNullOrWhiteSpace(optionString))
                return null;
            
            if(int.TryParse(typeString, out var type) && int.TryParse(optionString, out var optionIndex))
            {
                return new SubTarget()
                {
                    Type = type,
                    OptionIndex = optionIndex
                };
            }

            return null;
        }

        void HandleEndElementByName()
        {
            switch (m_Reader.Name)
            {
                case k_ShortCut:
                    m_Shortcuts.Add(m_CurrentShortcut);
                    m_CurrentShortcut = null;
                    break;
            }
        }

        ResolumeOscShortcut NewShortcut()
        {
            var uniqueIdString = m_Reader.GetAttribute("uniqueId");
            long.TryParse(uniqueIdString, out var id);
            return new ResolumeOscShortcut { UniqueId = id };
        }

        public void ParseShortcutPath()
        {
            int.TryParse(m_Reader.GetAttribute("translationType"), out var translationType);
            int.TryParse(m_Reader.GetAttribute("allowedTranslationTypes"), out var allowedTranslationTypes);
            
            var parsed = new ShortcutPath
            {
                Name = m_Reader.GetAttribute("name"),
                Path = m_Reader.GetAttribute("path"),
                TranslationType = translationType,
                AllowedTranslationTypes = allowedTranslationTypes
            };

            switch (parsed.Name)
            {
                case "InputPath":
                    m_CurrentShortcut.Input = parsed;
                    break;
                case "OutputPath":
                    m_CurrentShortcut.Output = parsed;
                    break;
            }
        }

        public void GatherTypeMetaData()
        {
            m_RegexToTypeMapper.Clear();
            foreach (var asset in m_PathMetaData)
            {
                if (asset == null)
                    continue;
                
                for (var i = 0; i < asset.InputPaths.Count; i++)
                {
                    var path = asset.InputPaths[i];
                    var regex = PathUtils.RegexForPath(path);
                    var type = TypeFromEnum(asset.Types[i]);
                    
                    if(type != null) 
                        m_RegexToTypeMapper.AddRegexToValue(regex, type);
                }
            }
        }

        static Type TypeFromEnum(TypeSelectionEnum selection)
        {
            switch (selection)
            {
                case TypeSelectionEnum.Float:
                    return typeof(float);
                case TypeSelectionEnum.Int:
                    return typeof(int);
                case TypeSelectionEnum.Bool:
                    return typeof(bool);
                case TypeSelectionEnum.String:
                    return typeof(string);
            }
            
            return null;
        }
    }
}