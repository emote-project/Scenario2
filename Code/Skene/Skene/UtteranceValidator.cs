using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Skene
{
    public class UtteranceValidationSet
    {
        public class SkeneUtteranceValidationSetFile
        {
            public string TargetInstructionValidator { get; set; }
            public string NonTargetInstructionValidator { get; set; }
            public string AnimationValidator { get; set; }
            public string GameInstructions { get; set; }
            public string TagsValidator { get; set; }
            public string TargetsValidator { get; set; }
            public string FaceExpressionsValidator { get; set; }
            public string TTSValidator { get; set; }

            public static SkeneUtteranceValidationSetFile Load(string filename)
            {
                using (StreamReader file = File.OpenText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SkeneUtteranceValidationSetFile suvs = (SkeneUtteranceValidationSetFile)serializer.Deserialize(file, typeof(SkeneUtteranceValidationSetFile));
                    string directory = Path.GetDirectoryName(filename) + "\\" + "Validators\\";
                    suvs.TargetInstructionValidator = directory + suvs.TargetInstructionValidator;
                    suvs.NonTargetInstructionValidator = directory + suvs.NonTargetInstructionValidator;
                    suvs.AnimationValidator = directory + suvs.AnimationValidator;
                    suvs.GameInstructions = directory + suvs.GameInstructions;
                    suvs.TagsValidator = directory + suvs.TagsValidator;
                    suvs.TargetsValidator = directory + suvs.TargetsValidator;
                    suvs.FaceExpressionsValidator = directory + suvs.FaceExpressionsValidator;
                    suvs.TTSValidator = directory + suvs.TTSValidator;
                    return suvs;
                }
            }
        }

        public UtteranceValidator TargetInstructionValidator { get; set; }
        public UtteranceValidator NonTargetInstructionValidator { get; set; }
        public UtteranceValidator AnimationValidator { get; set; }
        public UtteranceValidator GameInstructions { get; set; }
        public UtteranceValidator TagsValidator { get; set; }
        public UtteranceValidator TargetsValidator { get; set; }
        public UtteranceValidator FaceExpressionsValidator { get; set; }
        public UtteranceValidator TTSValidator { get; set; }

        public string Filename { get; set; }

        public UtteranceValidationSet(string validationSetFile)
        {
            this.Filename = validationSetFile;
            string directory = Path.GetDirectoryName(validationSetFile);
            SkeneUtteranceValidationSetFile suvs = SkeneUtteranceValidationSetFile.Load(validationSetFile);
            TargetInstructionValidator = UtteranceValidator.Load(suvs.TargetInstructionValidator);
            NonTargetInstructionValidator = UtteranceValidator.Load(suvs.NonTargetInstructionValidator);
            AnimationValidator = UtteranceValidator.Load(suvs.AnimationValidator);
            GameInstructions = UtteranceValidator.Load(suvs.GameInstructions);
            TagsValidator = UtteranceValidator.Load(suvs.TagsValidator);
            TargetsValidator = UtteranceValidator.Load(suvs.TargetsValidator);
            FaceExpressionsValidator = UtteranceValidator.Load(suvs.FaceExpressionsValidator);
            TTSValidator = UtteranceValidator.Load(suvs.TTSValidator);
        }

        public UtteranceValidationSet(
            UtteranceValidator TargetInstructionValidator,
            UtteranceValidator NonTargetInstructionValidator,
            UtteranceValidator AnimationValidator,
            UtteranceValidator GameInstructions,
            UtteranceValidator TagsValidator,
            UtteranceValidator TargetsValidator,
            UtteranceValidator FaceExpressionsValidator,
            UtteranceValidator TTSValidator)
        {
            this.TargetInstructionValidator = TargetInstructionValidator;
            this.NonTargetInstructionValidator = NonTargetInstructionValidator;
            this.AnimationValidator = AnimationValidator;
            this.GameInstructions = GameInstructions;
            this.TagsValidator = TagsValidator;
            this.TargetsValidator = TargetsValidator;
            this.FaceExpressionsValidator = FaceExpressionsValidator;
            this.TTSValidator = TTSValidator;
        }
    }

    public class UtteranceValidator
    {
        public List<String> ValidValues { get; set; }
        public bool CaseSensitive { get; set; }

        public UtteranceValidator() { }
        public UtteranceValidator(bool CaseSensitive = false)
        {
            this.ValidValues = new List<String>();
            this.CaseSensitive = CaseSensitive;
        }

        public string BuildRegexParameterString(string basePattern)
        {
            string result = "";
            int i = 0;
            foreach(string s in ValidValues) 
            {
                result += basePattern.Replace("INSTRUCTION", s);
                i++;
                if (i < ValidValues.Count) result += "|";
            }
            return result;
        }

        public UtteranceValidator(List<string> values, bool CaseSensitive = false)
        {
            this.CaseSensitive = CaseSensitive;
            ValidValues = values;
            if (!CaseSensitive) ValidValues = values.ConvertAll(d => d.ToLower());
        }

        public static UtteranceValidator Load(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader file = File.OpenText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    UtteranceValidator uv = (UtteranceValidator)serializer.Deserialize(file, typeof(UtteranceValidator));
                    if (!uv.CaseSensitive) uv.ValidValues = uv.ValidValues.ConvertAll(d => d.ToLower());
                    return uv;
                }
            }
            else 
            {
                Thalamus.Environment.Instance.DebugError("Unable to load Skene UtteranceValidator: File '{0}' does not exist!", filename);
                return new UtteranceValidator();
            }
        }

        public bool IsValid(string p)
        {
            if (!CaseSensitive) return ValidValues.Contains(p.ToLower());
            else return ValidValues.Contains(p);
        }
    }
}
