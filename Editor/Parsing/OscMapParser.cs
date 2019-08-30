using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Resolunity
{
    public class OscMapParser
    {
#if UNITY_EDITOR_WIN
        internal const string DefaultAvenuePath = "\\Documents\\Resolume Avenue\\Shortcuts\\OSC\\Default.xml";
        internal const string DefaultArenaPath = "\\Documents\\Resolume Arena\\Shortcuts\\OSC\\Default.xml";
#endif
        
        const string k_BoolParamNodeName = "RangedParam[bool]";
        const string k_ParamRangeNodeName = "ParamRange";
        const string k_ParamChoiceInt = "ParamChoice[int]";
        const string k_UnsignedLongLongParam = "ParamChoice[unsigned long long]";
        const string k_AutopilotTargetParam = "ParamChoice[struct AutoPilot::Target]";
        const string k_ParamTrigger = "ParamTrigger";
        
        const string k_UnknownResolumeType =
            "The Resolume shortcut with unique id {0}'s attribute 'paramNodeName' had a value of {1}, " +
            "which we don't know the data type for";

        const string k_VersionInfoNodeName = "versionInfo";
        const string k_SubTargetNodeName = "Subtarget";
        const string k_ShortCut = "Shortcut";
        const string k_ShortCutPath = "ShortcutPath";

        readonly XmlReaderSettings m_XmlSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
        
        readonly List<ResolumeOscShortcut> m_Shortcuts = new List<ResolumeOscShortcut>();
        
        XmlReader m_Reader;

        ResolumeOscShortcut m_CurrentShortcut;
        ResolumeOscMap m_Map;
        ResolumeVersion m_Version;
        
        public string OutputPath { get; set; }
        
        public void ParseDefaultFile()
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = userPath + DefaultAvenuePath;
            ParseFile(path);
        }

        public void ParseFile(string filePath)
        {
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
            
            Debug.Log($"{m_Shortcuts.Count} Resolume OSC shortcuts found in map");

            CreateAsset();
        }

        void CreateAsset()
        {
            m_Map = ScriptableObject.CreateInstance<ResolumeOscMap>();
            m_Map.Version = m_Version;

            foreach (var shortcut in m_Shortcuts)
            {
                m_Map.Shortcuts.Add(shortcut);
            }
            
            m_Map.GroupSubTargets();

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

            var paramNodeName = m_Reader.GetAttribute("paramNodeName");
            if (!TryParseType(paramNodeName, out var type))
                Debug.LogWarningFormat(k_UnknownResolumeType, uniqueIdString, paramNodeName);

            return new ResolumeOscShortcut
            {
                UniqueId = id,
                TypeName = type
            };
        }
        
        bool TryParseType(string paramNodeName, out string type)
        {
            Debug.Log("nodeName " + paramNodeName);

            switch (paramNodeName)
            {
                case k_BoolParamNodeName:
                    Debug.Log(m_Reader.Name + " - bool");
                    type = typeof(bool).Name;
                    break;
                case k_ParamRangeNodeName:
                    Debug.Log(m_Reader.Name + " - float");
                    type = typeof(float).Name;
                    break;
                case k_ParamChoiceInt:
                case k_ParamTrigger:
                case k_AutopilotTargetParam:    
                    Debug.Log(m_Reader.Name + " - int");
                    type = typeof(int).Name;
                    break;
                case k_UnsignedLongLongParam:
                    Debug.Log(m_Reader.Name + " - [ulong long]");
                    type = typeof(long).Name;
                    break;
                default:
                    type = default;
                    break;
            }

            return type != default;
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
    }
}