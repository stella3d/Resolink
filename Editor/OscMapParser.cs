using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace UnityResolume
{
    public class OscMapParser
    {
#if UNITY_EDITOR_WIN
        const string k_DefaultAvenuePath = "\\Documents\\Resolume Avenue\\Shortcuts\\OSC\\Default.xml";
        const string k_DefaultArenaPath = "\\Documents\\Resolume Arena\\Shortcuts\\OSC\\Default.xml";
#endif

        const string k_ShortCut = "Shortcut";
        const string k_ShortCutPath = "ShortcutPath";

        readonly XmlReaderSettings m_XmlSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
        
        readonly List<ResolumeOscShortcut> m_Shortcuts = new List<ResolumeOscShortcut>();
        
        XmlReader m_Reader;

        ResolumeOscShortcut m_CurrentShortcut;
        ResolumeOscMap m_Map;
        
        public string OutputPath { get; set; }
        
        public void ParseDefaultFile()
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = userPath + k_DefaultAvenuePath;
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

            foreach (var shortcut in m_Shortcuts)
            {
                m_Map.Shortcuts.Add(shortcut);
            }

            AssetDatabase.CreateAsset(m_Map, OutputPath);
        }

        void HandleNodeByName()
        {
            switch (m_Reader.Name)
            {
                case k_ShortCut:
                    m_CurrentShortcut = NewShortcut();
                    break;
                case k_ShortCutPath:
                    ParseShortcutPath();
                    break;
            }
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
            long.TryParse(m_Reader.GetAttribute("uniqueId"), out var id);
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
    }
}