using Godot;
using Godot.Collections;

namespace DiggyDig.scripts.options
{
    public class OptionHandler
    {
        protected Dictionary Options { get; set; }

        public readonly string InvertXRotation = "Invert X Rotation";
        public readonly string InvertYRotation = "Invert Y Rotation";
        public readonly string InvertXPanning = "Invert X Panning";
        public readonly string InvertYPanning = "Invert Y Panning";
        public readonly string InvertZooming = "Invert Zooming";

        public readonly string OptionsFile = "user://options.dat";
        
        public OptionHandler()
        {
            if (this.LoadOptions() == false)
            {
                this.Options = new Dictionary
                {
                    {InvertXRotation, false},
                    {InvertYRotation, false},
                    {InvertXPanning, false},
                    {InvertYPanning, false},
                    {InvertZooming, false}
                };
            }
        }

        public bool SaveOptions()
        {
            string result = JSON.Print(this.Options);
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

                this.Options = (Dictionary) parseResult.Result;

                GD.Print("Loaded options!");
                return true;
            }

            GD.Print("Failed to load options!");
            return false;
        }

        public bool GetOption(string name)
        {
            if (this.Options.Contains(name))
            {
                return (bool) this.Options[name];
            }

            return false;
        }

        public bool SetOption(string name, object value)
        {
            if (!this.Options.Contains(name))
            {
                return false;
            }
            
            this.Options[name] = value;
            return true;
        }
    }
}