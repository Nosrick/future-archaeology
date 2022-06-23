using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class SurveyTool : ITool
    {
        public string TranslationKey => "tools.survey.name";
        public int Cost => 30;

        public int UsageCooldown => 10;

        public int CooldownTimer { get; protected set; }
        
        public int PingSize { get; protected set; }
        
        public bool IsUsable()
        {
            return this.CooldownTimer == 0;
        }

        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        public SurveyTool()
        {
            this.PingSize = 2;
            
            this.AssociatedSound = new AudioStreamRandomPitch();
            /*
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/survey-1.wav");
            this.AssociatedSound.RandomPitch = 1.2f;
            */
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digSite = GlobalConstants.GameManager.DiggingSpace;
            
            Vector3Int dir = hit - previous;

            int xMin = this.PingSize, xMax = this.PingSize;
            int yMin = this.PingSize, yMax = this.PingSize;
            int zMin = this.PingSize, zMax = this.PingSize;
            int xStep = 1, yStep = 1, zStep = 1;

            if (dir.x != 0)
            {
                if (Mathf.Sign(dir.x) > 0)
                {
                    xMin = 0;
                    xMax = this.PingSize * 2;
                }
                else
                {
                    xMin = this.PingSize * 2;
                    xMax = 0;
                    xStep = -1;
                }
            }
            if (dir.y != 0)
            {
                if (Mathf.Sign(dir.y) > 0)
                {
                    yMin = 0;
                    yMax = this.PingSize * 2;
                }
                else
                {
                    yMin = this.PingSize * 2;
                    yMax = 0;
                    yStep = -1;
                }
            }
            else if (dir.z != 0)
            {
                if (Mathf.Sign(dir.z) > 0)
                {
                    zMin = 0;
                    zMax = this.PingSize * 2;
                }
                else
                {
                    zMin = this.PingSize * 2;
                    zMax = 0;
                    zStep = -1;
                }
            }

            for (int x = hit.x - xMin; x <= hit.x + xMax; x++)
            {
                for (int y = hit.y - yMin; y <= hit.y + yMax; y++)
                {
                    for (int z = hit.z - zMin; z <= hit.z + zMax; z++)
                    {
                        Vector3Int point = new Vector3Int(x, y, z);
                        DigItem item = digSite.GetObjectAt(point);
                        if (item is null == false
                            && item.Flashing == false)
                        {
                            item.MakeMeFlash();
                        }
                    }
                }
            }

            this.CooldownTimer = this.UsageCooldown;
            return this.Cost;
        }
    }
}