using System;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.tools
{
    public class LaserTool : AbstractTool
    {
        public const string RANGE_KEY = "range";
        
        public override string TranslationKey => "tools.laser.name";

        public const int DEFAULT_COST = 30;
        public const int DEFAULT_COOLDOWN = 12;

        public const int DEFAULT_RANGE = 5;

        public const int DEFAULT_DAMAGE = 3;
        
        public int Range { get; protected set; }
        
        public int Damage { get; protected set; }

        public LaserTool()
        {
            this.Damage = DEFAULT_DAMAGE;
            this.Range = DEFAULT_RANGE;
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;

            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/laser-1.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }

        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digMap = GlobalConstants.GameManager.DiggingSpace;
            
            Vector3Int dir = hit - previous;
            
            Vector3.Axis hitAxis = Vector3.Axis.X;

            int stepDir = 1;

            if (dir.y != 0)
            {
                hitAxis = Vector3.Axis.Y;
                if (dir.y < 0)
                {
                    stepDir = -1;
                }
            }
            else if (dir.z != 0)
            {
                hitAxis = Vector3.Axis.Z;
                if (dir.z < 0)
                {
                    stepDir = -1;
                }
            }
            else if(dir.x < 0)
            {
                stepDir = -1;
            }
            int range = (this.Range * stepDir);

            AABB box = new AABB();
            box.Position = hit.ToVector3();

            switch (hitAxis)
            {
                case Vector3.Axis.X:
                    for (int x = hit.x; x != hit.x + range; x += stepDir)
                    {
                        digMap.DamageCell(x, hit.y, hit.z, this.Damage);
                    }

                    box.End = new Vector3(hit.x + range, hit.y, hit.z);
                    break;
                case Vector3.Axis.Y:
                    for (int y = hit.y; y != hit.y + range; y += stepDir)
                    {
                        digMap.DamageCell(hit.x, y, hit.z, this.Damage);
                    }

                    box.End = new Vector3(hit.x, hit.y + range, hit.z);
                    break;
                case Vector3.Axis.Z:
                    for (int z = hit.z; z != hit.z + range; z += stepDir)
                    {
                        digMap.DamageCell(hit.x, hit.y, z, this.Damage);
                    }

                    box.End = new Vector3(hit.x, hit.y, hit.z + range);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            this.TimesUsed++;
            this.CooldownTimer = this.UsageCooldown;

            return box;
        }

        public override void Load(Dictionary data)
        {
            base.Load(data);

            this.Damage = (int) data[DAMAGE_KEY];
            this.Range = (int) data[RANGE_KEY];
        }

        public override Dictionary Save()
        {
            Dictionary saveDict = base.Save();

            saveDict.Add(DAMAGE_KEY, this.Damage);
            saveDict.Add(RANGE_KEY, this.Range);

            return saveDict;
        }
    }
}