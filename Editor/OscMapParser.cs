using System;
using System.Collections.Generic;
using System.Xml;
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
        const string k_EndShortCut = "/Shortcut";
        const string k_ShortCutPath = "ShortcutPath";

        readonly XmlReaderSettings m_XmlSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
        
        Dictionary<string, ResolumeOscShortcut> m_ShortcutsByOutputPath = new Dictionary<string, ResolumeOscShortcut>();
        List<ResolumeOscShortcut> m_Shortcuts = new List<ResolumeOscShortcut>();
        
        
        XmlReader m_Reader ;

        ResolumeOscShortcut m_CurrentShortcut;

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
            
            // Parse the file and display each of the nodes.
            while (m_Reader.Read()) {
                switch (m_Reader.NodeType) {
                    case XmlNodeType.Element:
                        //Debug.LogFormat("<{0}>", m_Reader.Name);
                        // this is where most things happen
                        HandleNodeByName();
                        break;
                    case XmlNodeType.Text:
                        //Debug.Log(m_Reader.Value);
                        break;
                    case XmlNodeType.CDATA:
                        //Debug.LogFormat("<![CDATA[{0}]]>", m_Reader.Value);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        //Debug.LogFormat("<?{0} {1}?>", m_Reader.Name, m_Reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        //Debug.LogFormat("<!--{0}-->", m_Reader.Value);
                        break;
                    case XmlNodeType.XmlDeclaration:
                        //Debug.Log("<?xml version='1.0'?>");
                        break;
                    case XmlNodeType.Document:
                        break;
                    case XmlNodeType.DocumentType:
                        //Debug.LogFormat("<!DOCTYPE {0} [{1}]", m_Reader.Name, m_Reader.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        //Debug.Log(m_Reader.Name);
                        break;
                    case XmlNodeType.EndElement:
                        //Debug.LogFormat("</{0}>", m_Reader.Name);
                        HandleEndElementByName();
                        break;
                } 
            }
            
            Debug.Log($"{m_Shortcuts.Count} Resolume OSC shortcuts found in map");
        }

        void HandleNodeByName()
        {
            switch (m_Reader.Name)
            {
                case k_ShortCut:
                    m_CurrentShortcut = new ResolumeOscShortcut();
                    break;
                case k_EndShortCut:
                    Debug.Log("finished parsing shortcut");
                    m_Shortcuts.Add(m_CurrentShortcut);
                    m_CurrentShortcut = null;
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
                    Debug.Log("finished parsing shortcut");
                    m_Shortcuts.Add(m_CurrentShortcut);
                    m_CurrentShortcut = null;
                    break;
            }
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