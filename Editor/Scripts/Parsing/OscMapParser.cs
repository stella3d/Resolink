using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Resolink
{
    [CreateAssetMenu(fileName = "NewOscMapParser.asset", menuName = "Resolink/Osc Map Parser", order = 2)]
    [ExecuteAlways]
    public class OscMapParser : ScriptableObject
    {
        static readonly Dictionary<string, List<SubTarget>> k_TargetGroup = new Dictionary<string, List<SubTarget>>();
        static readonly List<ResolumeOscShortcut> k_NewShortcuts = new List<ResolumeOscShortcut>();
        static readonly HashSet<string> k_ProcessedOutputs = new HashSet<string>();
        
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
            m_XmlSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            m_Shortcuts = new List<ResolumeOscShortcut>();
            GatherTypeMetaData();
        }

        public void ParseFile(string filePath)
        {
            Profiler.BeginSample("Resolink Parse Osc Map");
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
            {
                Profiler.EndSample();
                return;
            }

            foreach (var shortcut in m_Shortcuts)
            {
                var inPath = shortcut.Input.Path;
                if (!m_RegexToTypeMapper.Process(inPath, out var typeForShortcut))
                {
                    if(ResolinkEditorSettings.Instance.WarnOnUnknownType)
                        Debug.LogWarning($"data type for input path '{inPath}' is unknown! This can be fixed by adding " + 
                                        "a ResolumeEventMetaData entry with a regular expression that matches this path.");
                    continue;
                }

                shortcut.TypeName = typeForShortcut.Name;
            }
            
            GroupSubTargets(ref m_Shortcuts);

            // sort by output path label
            m_Shortcuts.Sort();

            FindColorGroups();

            Profiler.EndSample();
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

            AssetDatabase.CreateAsset(m_Map, OutputPath);
            m_Shortcuts.Clear();
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

        public static void GroupSubTargets(ref List<ResolumeOscShortcut> shortcuts)
        {
            k_NewShortcuts.Clear();
            k_ProcessedOutputs.Clear();
            k_TargetGroup.Clear();
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.SubTargets == null)
                    continue;
                
                var outPath = shortcut.Output.Path;
                if (shortcut.SubTargets == null)
                {
                    k_TargetGroup.Add(outPath, null);
                    continue;
                }

                if (k_TargetGroup.TryGetValue(outPath, out var targetList))
                    targetList.Add(shortcut.SubTargets[0]);
                else
                    k_TargetGroup.Add(outPath, new List<SubTarget> {shortcut.SubTargets[0]});
            }

            foreach (var kvp in k_TargetGroup)
            {
                var targets = kvp.Value;
                if(targets == null || targets.Count == 1)
                    continue;
                
                targets.Sort();
            }

            foreach (var shortcut in shortcuts)
            {
                var outPath = shortcut.Output.Path;
                if (k_ProcessedOutputs.Contains(outPath))
                    continue;

                if (k_TargetGroup.TryGetValue(outPath, out var targetList))
                {
                    shortcut.SubTargets = targetList?.ToArray();
                    k_NewShortcuts.Add(shortcut);
                }

                k_ProcessedOutputs.Add(outPath);
            }
            
            shortcuts = k_NewShortcuts.ToArray().ToList();
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
                        m_RegexToTypeMapper.Add(regex, type);
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
        
        static readonly List<ResolumeOscShortcut> k_RedColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_GreenColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_BlueColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_AlphaColorShortcuts = new List<ResolumeOscShortcut>();

        static readonly HashSet<string> k_ColorShortcutPrefixes = new HashSet<string>();
        
        void FindColorGroups()
        {
            k_ColorShortcutPrefixes.Clear();
            k_RedColorShortcuts.Clear();
            k_GreenColorShortcuts.Clear();
            k_BlueColorShortcuts.Clear();
            k_AlphaColorShortcuts.Clear();
            
            foreach (var shortcut in m_Shortcuts)
            {
                var inPath = shortcut.Input.Path;
                if(Regexes.RedColorComponent.IsMatch(inPath))
                    k_RedColorShortcuts.Add(shortcut);
                else if(Regexes.GreenColorComponent.IsMatch(inPath))
                    k_GreenColorShortcuts.Add(shortcut);
                else if(Regexes.BlueColorComponent.IsMatch(inPath))
                    k_BlueColorShortcuts.Add(shortcut);
                else if(Regexes.AlphaColorComponent.IsMatch(inPath))
                    k_AlphaColorShortcuts.Add(shortcut);
            }

            AddToPrefixSet(k_RedColorShortcuts);
            AddToPrefixSet(k_GreenColorShortcuts);
            AddToPrefixSet(k_BlueColorShortcuts);
            AddToPrefixSet(k_AlphaColorShortcuts);

            foreach (var prefix in k_ColorShortcutPrefixes)
            {
                Debug.Log($"checking color prefix : {prefix}");
                if (AllColorComponentsFound(prefix))
                    Debug.Log("all components found!");
            }
        }

        static bool AllColorComponentsFound(string prefix)
        {
            var rFound = PrefixFoundInList(prefix, k_RedColorShortcuts);       
            var gFound = PrefixFoundInList(prefix, k_GreenColorShortcuts);       
            var bFound = PrefixFoundInList(prefix, k_BlueColorShortcuts);       
            var aFound = PrefixFoundInList(prefix, k_AlphaColorShortcuts);       
            return rFound && gFound && bFound && aFound;
        }

        static bool PrefixFoundInList(string prefix, List<ResolumeOscShortcut> shortcuts)
        {
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.Input.Path.StartsWith(prefix))
                    return true;
            }

            return false;
        }

        static void AddToPrefixSet(List<ResolumeOscShortcut> shortcuts)
        {
            foreach (var shortcut in shortcuts)
            {
                var inPath = shortcut.Input.Path;
                var lastSplit = inPath.LastIndexOf('/');
                var prefix = inPath.Substring(0, lastSplit);
                k_ColorShortcutPrefixes.Add(prefix);
            }
        }
    }
}