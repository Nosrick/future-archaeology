using Godot;
using Godot.Collections;

namespace DiggyDig.scripts.options
{
    public class OptionHandler
    {
        public Dictionary Options
        {
            get => this.m_Options.Duplicate();
            protected set => this.m_Options = value;
        }

        protected Dictionary m_Options;

        public readonly string InvertXRotation = "Invert X Rotation";
        public readonly string InvertYRotation = "Invert Y Rotation";
        public readonly string InvertXPanning = "Invert X Panning";
        public readonly string InvertYPanning = "Invert Y Panning";
        public readonly string InvertZooming = "Invert Zooming";
        public readonly string RotationSensitivity = "Rotation Sensitivity";
        public readonly string PanningSensitivity = "Panning Sensitivity";
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
                {UseMoney, true}
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
                
                GD.Print("Saved options!");
                return true;
            }

            GD.Print("Failed to save options!");
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
                    GD.Print("Failed to load options!");
                    return false;
                }

                this.m_Options = (Dictionary) parseResult.Result;

                GD.Print("Loaded options!");
                return true;
            }

            GD.Print("Failed to load options!");
            return false;
        }

        public void ResetToDefaults()
        {
            this.m_Options = this.LoadDefaults();
        }

        public T GetOption<T>(string name)
        {
            if (this.m_Options.Contains(name))
            {
                return (T) this.m_Options[name];
            }

            return default;
        }

        public bool SetOption(string name, object value)
        {
            if (!this.m_Options.Contains(name))
            {
                this.m_Options.Add(name, value);
                return true;
            }

            this.m_Options[name] = value;
            return true;
        }
    }
}