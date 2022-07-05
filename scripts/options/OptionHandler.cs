using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.options
{
    public class OptionHandler
    {
        public Dictionary Options
        {
            get => this.m_Options.Duplicate();
            protected set => this.m_Options = value;
        }

        protected Dictionary m_Options;

        //Key: localised strings of the translation keys
        //Value: the raw translation keys
        protected IDictionary<string, string> m_LocaleMap;

        public readonly string InvertXRotation = "options.invert_x_rotation.label";
        public readonly string InvertYRotation = "options.invert_y_rotation.label";
        public readonly string InvertXPanning = "options.invert_x_panning.label";
        public readonly string InvertYPanning = "options.invert_y_panning.label";
        public readonly string InvertZooming = "options.invert_zooming.label";
        public readonly string RotationSensitivity = "options.rotation_sensitivity.label";
        public readonly string PanningSensitivity = "options.panning_sensitivity.label";
        public readonly string ZoomingSensitivity = "options.zooming_sensitivity.label";
        public readonly string UseMoney = "Use Money";

        public readonly string OptionsFile = "user://options.dat";
        
        public OptionHandler()
        {
            if (this.LoadOptions() == false)
            {
                this.ResetToDefaults();
            }
        }

        protected Dictionary LoadDefaults()
        {
            return new Dictionary
            {
                {InvertXRotation, false},
                {InvertYRotation, false},
                {InvertXPanning, false},
                {InvertYPanning, false},
                {InvertZooming, false},
                {RotationSensitivity, 0.1f},
                {PanningSensitivity, 1f},
                {ZoomingSensitivity, 1f}
            };
        }
        
        public bool SaveOptions()
        {
            string result = JSON.Print(this.m_Options);
            File optionsFile = new File();
            if (optionsFile.Open(OptionsFile, File.ModeFlags.Write) == Error.Ok)
            {
                optionsFile.StoreString(result);
                optionsFile.Close();
                
                return true;
            }

            GD.PrintErr("Failed to save options!");
            return false;
        }

        public bool LoadOptions()
        {
            File optionsFile = new File();
            if (optionsFile.Open(OptionsFile, File.ModeFlags.Read) == Error.Ok)
            {
                string result = optionsFile.GetAsText();
                optionsFile.Close();
                JSONParseResult parseResult = JSON.Parse(result);
                if (parseResult.Error != Error.Ok)
                {
                    GD.PrintErr("Failed to load options!");
                    return false;
                }

                this.m_Options = (Dictionary) parseResult.Result;
                
                this.OptionsToLocale();

                return true;
            }

            GD.PrintErr("Failed to load options!");
            return false;
        }

        public void ResetToDefaults()
        {
            this.m_Options = this.LoadDefaults();
            this.OptionsToLocale();
        }

        protected void OptionsToLocale()
        {
            this.m_LocaleMap = new System.Collections.Generic.Dictionary<string, string>();

            Dictionary defaults = this.LoadDefaults();
            foreach (DictionaryEntry entry in defaults)
            {
                if (!this.m_Options.Contains(entry.Key))
                {
                    this.m_Options.Add(entry.Key, entry.Value);
                }
            }

            foreach (DictionaryEntry entry in this.m_Options)
            {
                string key = entry.Key.ToString();
                this.m_LocaleMap.Add(TranslationServer.Translate(key), key);
            }
        }

        public T GetOption<T>(string name)
        {
            if (this.m_Options.Contains(name))
            {
                return (T) this.m_Options[name];
            }
            if (this.m_LocaleMap.ContainsKey(name)
                && this.m_Options.Contains(this.m_LocaleMap[name]))
            {
                return (T) this.m_Options[this.m_LocaleMap[name]];
            }

            return default;
        }

        public bool SetOption(string name, object value)
        {
            if (this.m_Options.Contains(name))
            {
                this.m_Options[name] = value;
                return true;
            }

            if (this.m_LocaleMap.ContainsKey(name))
            {
                if (!this.m_Options.Contains(this.m_LocaleMap[name]))
                {
                    this.m_Options.Add(this.m_LocaleMap[name], value);
                    return true;
                }

                this.m_Options[this.m_LocaleMap[name]] = value;
                return true;
            }

            return false;
        }
    }
}