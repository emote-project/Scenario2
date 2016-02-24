using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmoteCommonMessages;
using Newtonsoft.Json;
using PhysicalSpaceManager;

namespace Skene
{
    internal class TargetsFile
    {
        public List<TargetInfo> Targets { get; set; }

        private static string currentFileName = "";

        public TargetsFile()
        {
            Targets = new List<TargetInfo>();
        }

        public static void Load(string filename, SkeneClient skene)
        {
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                TargetsFile f = (TargetsFile)serializer.Deserialize(file, typeof(TargetsFile));
                currentFileName = filename;
                foreach (TargetInfo t in f.Targets)
                {
                    skene.Targets[t.TargetName.ToLower()] = t;
                }
            }
        }

        public static void Save(SkeneClient bpc)
        {
            if (currentFileName != "") Save(currentFileName, bpc);
        }
        public static void Save(string filename, SkeneClient bpc)
        {
            TargetsFile t = new TargetsFile();
            lock (bpc.Targets.Values)
            {
                t.Targets = new List<TargetInfo>(bpc.Targets.Values);
            }
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, t);
            }
        }
    }

    internal class TargetInfo
    {
        public GazeTarget GazeTarget { get; set; }
        public Vector2D Coordinates { get; set; }
        public string TargetName { get; set; }
        public bool Linked { get; set; }
        public string LinkedTargetName { get; set; }

        private SkeneClient skene;

        public TargetInfo() { }
        public TargetInfo(string targetName, string linkedGazeTarget, SkeneClient skene)
        {
            Linked = true;
            LinkedTargetName = linkedGazeTarget;
            TargetName = targetName;
            this.skene = skene;
            if (Linked && skene != null && skene.Targets.ContainsKey(LinkedTargetName))
            {
                Coordinates = skene.Targets[LinkedTargetName].Coordinates;
                GazeTarget = skene.Targets[LinkedTargetName].GazeTarget;
            }
        }

        public TargetInfo(GazeTarget gazeTarget)
        {
            this.GazeTarget = gazeTarget;
            this.TargetName = gazeTarget.ToString().ToLower();
            this.Coordinates = Vector2D.Zero;
            Linked = false;
        }
        public TargetInfo(string targetName, GazeTarget gazeTarget)
        {
            this.GazeTarget = gazeTarget;
            this.TargetName = targetName;
            this.Coordinates = Vector2D.Zero;
            Linked = false;
        }

        public TargetInfo(Vector2D coordinates, string targetName = "", GazeTarget targetType = EmoteCommonMessages.GazeTarget.ScreenPoint)
        {
            this.GazeTarget = targetType;
            this.Coordinates = coordinates;
            this.TargetName = targetName;
            Linked = false;
        }

        public void Generate(SkeneClient bpc, TargetType target = TargetType.Gaze, bool dontPerform = false)
        {
            string realTargetName = TargetName;
            if (Linked && skene != null && skene.Targets.ContainsKey(LinkedTargetName))
            {
                realTargetName = skene.Targets[LinkedTargetName].TargetName;
                Coordinates = skene.Targets[LinkedTargetName].Coordinates;
                GazeTarget = skene.Targets[LinkedTargetName].GazeTarget;
            }
            switch (target)
            {
                case TargetType.Waving:
                    if (GazeTarget == EmoteCommonMessages.GazeTarget.ScreenPoint)
                    {
                        bpc.Debug("Generated WavingScreen {0} {1}", Coordinates.X, Coordinates.Y);
                        bpc.WaveAtScreen("", Coordinates.X, Coordinates.Y, 5, 4, 2);
                    }
                    else
                    {
                        bpc.Debug("Generated WavingAngle {0} {1}", Coordinates.X, Coordinates.Y);
                        bpc.SkPublisher.Waving("", Coordinates.X, Coordinates.Y, 2, 10, 2);
                    }
                    break;
                case TargetType.Pointing:
                    if (GazeTarget == EmoteCommonMessages.GazeTarget.ScreenPoint)
                    {
                        bpc.Debug("Generated PointScreen {0} {1}", Coordinates.X, Coordinates.Y);
                        bpc.PointAtScreen("", Coordinates.X, Coordinates.Y);
                    }
                    else
                    {
                        bpc.Debug("Generated PointAngle {0} {1}", Coordinates.X, Coordinates.Y);
                        bpc.SkPublisher.PointingAngle("", Coordinates.X, Coordinates.Y, 0.5, Coordinates.X < 0 ? Thalamus.Actions.PointingMode.LeftHand : Thalamus.Actions.PointingMode.RightHand);
                    }
                    break;
                case TargetType.Highlight:
                    if (realTargetName != "")
                    {
                        switch (realTargetName)
                        {
                            case "rightAnswer":
                                bpc.SkPublisher.HighlightRightAnswer();
                                break;
                        }
                    }
                    else
                    {
                        bpc.Debug("Generated Highlight {0} {1}", Coordinates.X, Coordinates.Y);
                        bpc.SkPublisher.Highlight(Coordinates.X, Coordinates.Y);
                    }
                    break;
                case TargetType.Glance:
                    if (GazeTarget == GazeTarget.ScreenPoint)
                    {
                        bpc.Debug("Generated Glance Screen: " + Coordinates);
                        bpc.GazeManager.Glance(this);
                    }
                    else if (GazeTarget == GazeTarget.Angle)
                    {
                        bpc.Debug("Generated Glance Angle: " + Coordinates);
                        bpc.GazeManager.Glance(this);
                    }
                    else
                    {
                        bpc.Debug("Generated Glance: " + GazeTarget);
                        bpc.GazeManager.Glance(this);
                    }
                    break;
                case TargetType.Gaze:
                default:
                    if (GazeTarget == GazeTarget.ScreenPoint)
                    {
                        bpc.Debug("Generated Gaze Screen: " + Coordinates);
                        bpc.GazeManager.Gaze(new TargetInfo(Coordinates), dontPerform);
                    }
                    else if (GazeTarget == GazeTarget.Angle)
                    {
                        bpc.Debug("Generated Gaze Angle: " + Coordinates);
                        bpc.GazeManager.Gaze(new TargetInfo(Coordinates, "", EmoteCommonMessages.GazeTarget.Angle), dontPerform);
                    }
                    else
                    {
                        bpc.Debug("Generated Gaze: " + GazeTarget);
                        bpc.GazeManager.Gaze(GazeTarget, dontPerform);
                    }
                    break;
            }
        }
    }
}
