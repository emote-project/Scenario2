using System;
using ikvm.extensions;
using java.io;
using weka.classifiers;
using weka.core;
using Attribute = weka.core.Attribute;
using Console = System.Console;
using File = System.IO.File;

namespace WekaWrapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cb = new WClassifier("../test.arff", "../test.model");
        }

        private static void Main2(string[] args)
        {
            string file = "../test.model";
            try
            {
                Console.WriteLine(">> " + File.Exists(file));
                InputStream f = new FileInputStream(file);
                var x = (Classifier) SerializationHelper.read(f);

                FastVector fv = new FastVector();
                var truefalsevec = new FastVector();
                truefalsevec.addElement("true");
                truefalsevec.addElement("false");

                var a0 = new Attribute("aiTurnDetector", truefalsevec);
                var a1 = new Attribute("humanTurnDetector", truefalsevec);
                var a2 = new Attribute("turnChangedDetector", truefalsevec);
                var a3 = new Attribute("aiActionDuringAiTurnDetector", truefalsevec);
                var a4 = new Attribute("gameActionPlayedDetector", truefalsevec);
                var a5 = new Attribute("aiDidGameActionDetector", truefalsevec);
                var a6 = new Attribute("newLevelDetector", truefalsevec);
                var a7 = new Attribute("playerInteractionInThisTurn", truefalsevec);
                var a8 = new Attribute("playerTurnLastingTooLong", truefalsevec);
                var a9 = new Attribute("playerNotInteractingForTooLongDetector", truefalsevec);

                FastVector classValues = new FastVector();
                classValues.addElement("IFMLSpeech.PerformUtterance:GameStarted");
                classValues.addElement("doNothingClass");
                classValues.addElement("IFMLSpeech.PerformUtterance:TurnChanged.Self");
                classValues.addElement("IFMLSpeech.PerformUtterance:ConfirmConstruction.Self");
                classValues.addElement("IFMLSpeech.PerformUtterance:TurnChanged.Other");
                classValues.addElement("IFMLSpeech.PerformUtterance:ConfirmConstruction.Other");
                classValues.addElement("IFMLSpeech.PerformUtterance:ImplementPolicy.Self");
                classValues.addElement("IFMLSpeech.PerformUtterance:PerformUpgrade.Self");
                var a10 = new Attribute("Behaviour ", classValues);

                fv.addElement(a0);
                fv.addElement(a1);
                fv.addElement(a2);
                fv.addElement(a3);
                fv.addElement(a4);
                fv.addElement(a5);
                fv.addElement(a6);
                fv.addElement(a7);
                fv.addElement(a8);
                fv.addElement(a9);
                fv.addElement(a10);

                Instances instances = new Instances("enercities", fv, 1);
                instances.setClassIndex(10);

                Instance instance = new Instance(10);
                instance.setValue(a0, "false");
                instance.setValue(a1, "true");
                instance.setValue(a2, "false");
                instance.setValue(a3, "false");
                instance.setValue(a4, "false");
                instance.setValue(a5, "false");
                instance.setValue(a6, "false");
                instance.setValue(a7, "false");
                instance.setValue(a8, "false");
                instance.setValue(a9, "false");

                instance.setDataset(instances);
                instances.add(instance);

                double labelID = x.classifyInstance(instance);
                string label = instance.classAttribute().value((int) labelID);

                Console.WriteLine("Classification: " + label);
            }
            catch (Exception x)
            {
                x.printStackTrace();
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}