using System;
using System.Collections.Generic;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class SurveyTool : AbstractTool
    {
        public override string TranslationKey => "tools.survey.name";
        public int PingSize { get; protected set; }
        public float PingDuration { get; protected set; }

        public const int DEFAULT_COST = 30;
        public const int DEFAULT_COOLDOWN = 16;

        public SurveyTool()
        {
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;
            
            this.PingSize = 2;
            this.PingDuration = 1f;

            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/survey-1.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }

        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digSite = GlobalConstants.GameManager.DiggingSpace;

            Vector3Int dir = hit - previous;
            Vector3Int.Axis hitAxis = Vector3Int.Axis.X;

            int xMin = this.PingSize, xMax = this.PingSize;
            int yMin = this.PingSize, yMax = this.PingSize;
            int zMin = this.PingSize, zMax = this.PingSize;
            int step = 1;

            if (dir.y != 0)
            {
                hitAxis = Vector3Int.Axis.Y;
            }
            else if (dir.z != 0)
            {
                hitAxis = Vector3Int.Axis.Z;
            }

            switch (hitAxis)
            {
                case Vector3Int.Axis.X:
                    if (Mathf.Sign(dir.x) > 0)
                    {
                        xMin = 0;
                        xMax = this.PingSize * 2 + 1;
                        step = -1;
                    }
                    else
                    {
                        xMin = this.PingSize * 2 + 1;
                        xMax = 0;
                    }
                    break;

                case Vector3Int.Axis.Y:
                    if (Mathf.Sign(dir.y) > 0)
                    {
                        yMin = 0;
                        yMax = this.PingSize * 2 + 1;
                        step = -1;
                    }
                    else
                    {
                        yMin = this.PingSize * 2 + 1;
                        yMax = 0;
                    }
                    break;
                
                case Vector3Int.Axis.Z:
                    if (Mathf.Sign(dir.z) > 0)
                    {
                        zMin = 0;
                        zMax = this.PingSize * 2 + 1;
                        step = -1;
                    }
                    else
                    {
                        zMin = this.PingSize * 2 + 1;
                        zMax = 0;
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Vector3Int begin = new Vector3Int(hit.x - xMin, hit.y - yMin, hit.z - zMin);
            Vector3Int end = new Vector3Int(hit.x + xMax, hit.y + yMax, hit.z + zMax);
            
            digSite.MakeAreaFlash(
                begin, 
                end, 
                hitAxis, 
                step, 
                (this.PingSize * 2 + 1), 
                this.PingDuration, 
                true, 
                true);

            this.CooldownTimer = this.UsageCooldown;
            AABB area = new AABB
            {
                Position = begin.ToVector3(),
                End = end.ToVector3()
            };
            return area;
        }
    }
}