using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NuttyTracks;
using NuttyTracks.AnimationControllers;
using System.Globalization;

namespace NuttyNAOTool
{
    public class ChoreographToNuttyConverter
    {
        public static void ConvertXarFiles(string path, bool searchSubdirectories = false)
        {
            if (searchSubdirectories)
            {
                Console.WriteLine("Converting XAR files in subdirectories of {0}...", path);
                string[] directories = Directory.GetDirectories(path);
                foreach (string dir in directories) ConvertXarFileInDirectory(dir);
            }
            else
            {
                ConvertXarFileInDirectory(path);
            }
        }

        

        private static void ConvertXarFileInDirectory(string pathDirectory)
        {
            string path = pathDirectory + "\\behavior.xar";
            if (File.Exists(path))
            {
                AnimationFileController.AnimationFile nuttyFile = null;
                try {
                    using (StreamReader file = File.OpenText(path))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(file.ReadToEnd());

                        XmlElement timelineEl = doc["ChoregrapheProject"]["Box"]["Timeline"]["BehaviorLayer"]["BehaviorKeyframe"]["Diagram"]["Box"]["Timeline"];
                        int fps = int.Parse(timelineEl.GetAttribute("fps"), CultureInfo.InvariantCulture.NumberFormat);

                        nuttyFile = new AnimationFileController.AnimationFile();
                        AnimationFileController.NuttyAnimation nuttyAnimation = new AnimationFileController.NuttyAnimation();
                        nuttyFile.NuttyAnimation = nuttyAnimation;
                        AnimationFileController.Meta nuttyAnimationMeta = new AnimationFileController.Meta();
                        nuttyAnimationMeta.BodySet = "NAOTorsoBodySet";
                        nuttyAnimationMeta.Type = AnimationBuffer.AnimationFrameType.AngleDeg;
                        nuttyAnimationMeta.FrameRate = fps;
                        nuttyFile.NuttyAnimation.Meta = nuttyAnimationMeta;
                        nuttyAnimation.Frames = new List<AnimationFileController.AnimationFrame>();

                        Dictionary<int, List<string>> keyFrames = new Dictionary<int, List<string>>();
                        Dictionary<string, Dictionary<int, double>> channelValues = new Dictionary<string, Dictionary<int, double>>();
                        
                        foreach(XmlElement channel in doc["ChoregrapheProject"]["Box"]["Timeline"]["BehaviorLayer"]["BehaviorKeyframe"]["Diagram"]["Box"]["Timeline"]["ActuatorList"].ChildNodes) {
                            string channelName = channel.GetAttribute("actuator");
                            channelValues[channelName] = new Dictionary<int,double>();
                            foreach (XmlElement keys in channel.ChildNodes)
                            {
                                int frame = int.Parse(keys.GetAttribute("frame"), CultureInfo.InvariantCulture.NumberFormat);
                                double value = double.Parse(keys.GetAttribute("value"), CultureInfo.InvariantCulture.NumberFormat);
                                if (!keyFrames.ContainsKey(frame)) keyFrames[frame] = new List<string>();
                                keyFrames[frame].Add(channelName);
                                channelValues[channelName][frame] = value;
                            }
                        }

                        foreach (KeyValuePair<int, List<string>> kf in keyFrames)
                        {
                            AnimationFileController.AnimationFrame af = new AnimationFileController.AnimationFrame();
                            af.Frame = kf.Key;
                            af.Channels = new List<AnimationFileController.AnimationChannel>();
                            foreach (string c in kf.Value)
                            {
                                if (channelValues.ContainsKey(c))
                                {
                                    AnimationFileController.AnimationChannel ac = new AnimationFileController.AnimationChannel("NAOTorsoBodySet." + c);
                                    ac.Speed = 1.0;
                                    ac.Value = channelValues[c][kf.Key];
                                    af.Channels.Add(ac);
                                }
                            }
                            nuttyAnimation.Frames.Add((af));
                        }
                    }
                    if (nuttyFile != null)
                    {
                        int i = 0;
                        string filename = pathDirectory + ".naf";
                        while (File.Exists(filename))
                        {
                            filename = pathDirectory + "_" + (i++ + 1).ToString() + ".naf";
                        }
                        AnimationFileController.AnimationFile.SaveAnimation(filename, nuttyFile);
                        Console.WriteLine("Saved nutty animation file '" + filename + "'.");
                    }
                }
                catch(Exception e) {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("No behavior.xar file found in {0}!", Path.GetDirectoryName(path));
            }
        }

        private static string ConvertXar(string file)
        {
            string nuttyAnim = "";
            
            return nuttyAnim;
        }

        static IEnumerable<XElement> StreamRootChildDoc(StringReader stringReader, string searchElement)
        {
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                reader.MoveToContent();
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == searchElement)
                            {
                                XElement el = XElement.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }
    }
}
