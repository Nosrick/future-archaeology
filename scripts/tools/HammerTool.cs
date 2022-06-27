using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class HammerTool : ITool
    {
        public string TranslationKey => "tools.hammer.name";
        public int Cost => 40;
        public int UsageCooldown => 3;
        public int CooldownTimer { get; protected set; }
        public bool IsUsable()
        {
            return this.CooldownTimer == 0;
        }

        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        public int Damage { get; protected set; }

        public HammerTool()
        {
            this.Damage = 3;
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/hammer-hit-2.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }

        public int Execute(Vector3Int hit, Vector3Int previous)
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
            return this.Cost;
        }
    }
}