using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.tools
{
    public class HammerTool : AbstractTool
    {
        public override string TranslationKey => "tools.hammer.name";

        public const int DEFAULT_COST = 40;
        public const int DEFAULT_COOLDOWN = 9;

        public int Damage { get; protected set; }

        public HammerTool()
        {
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;
            
            this.Damage = 3;
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/hammer-hit-2.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }

        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digSite = GlobalConstants.GameManager.DiggingSpace;

            Vector3Int dir = hit - previous;

            Vector3.Axis hitAxis = Vector3.Axis.X;

            if (dir.y != 0)
            {
                hitAxis = Vector3.Axis.Y;
            }
            else if (dir.z != 0)
            {
                hitAxis = Vector3.Axis.Z;
            }

            int xStep = 0;
            int yStep = 0;
            int zStep = 0;

            switch (hitAxis)
            {
                case Vector3.Axis.X:
                {
                    yStep = 1;
                    zStep = 1;

                    int x = hit.x;

                    for (int y = hit.y - yStep; y <= hit.y + yStep; y += yStep)
                    {
                        for (int z = hit.z - zStep; z <= hit.z + zStep; z += zStep)
                        {
                            digSite.DamageCell(x, y, z, this.Damage);
                        }
                    }

                    break;
                }
                case Vector3.Axis.Y:
                {
                    xStep = 1;
                    zStep = 1;

                    int y = hit.y;

                    for (int x = hit.x - xStep; x <= hit.x + xStep; x += xStep)
                    {
                        for (int z = hit.z - zStep; z <= hit.z + zStep; z += zStep)
                        {
                            digSite.DamageCell(x, y, z, this.Damage);
                        }
                    }

                    break;
                }
                default:
                {
                    xStep = 1;
                    yStep = 1;

                    int z = hit.z;

                    for (int x = hit.x - xStep; x <= hit.x + xStep; x += xStep)
                    {
                        for (int y = hit.y - yStep; y <= hit.y + yStep; y += yStep)
                        {
                            digSite.DamageCell(x, y, z, this.Damage);
                        }
                    }

                    break;
                }
            }

            this.CooldownTimer = this.UsageCooldown;
            AABB area = new AABB
            {
                Position = new Vector3(hit.x - xStep, hit.y - yStep, hit.z - zStep),
                End = new Vector3(hit.x + xStep, hit.y + yStep, hit.z + zStep)
            };
            
            return area;
        }

        public override void Load(Dictionary data)
        {
            base.Load(data);

            this.Damage = (int) data[DAMAGE_KEY];
        }

        public override Dictionary Save()
        {
            Dictionary saveDict = base.Save();

            saveDict.Add(DAMAGE_KEY, this.Damage);

            return saveDict;
        }
    }
}