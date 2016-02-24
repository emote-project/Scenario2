using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Classification.Classifier
{
    public interface IClassifier : IDisposable
    {
        [JsonIgnore]
        List<string> Labels { get; }

        [JsonProperty]
        uint NumInstances { get; }

        /// <summary>
        ///     Classifies a features vector
        /// </summary>
        /// <param name="featuresVector">a set of feature's name/value pairs</param>
        /// <returns>Classification of the features vector</returns>
        ClassificationResult Classify(IDictionary<string, string> featuresVector);

        /// <summary>
        ///     Trains the classifier with a given instance.
        /// </summary>
        /// <param name="featuresVector">the state of all features</param>
        /// <param name="label">the label associated with the given instance</param>
        void Train(IDictionary<string, string> featuresVector, string label);

        /// <summary>
        ///     Load the classifier from a file.
        /// </summary>
        void Load(string filePath);

        /// <summary>
        ///     Saves the classifier to a file.
        /// </summary>
        void Save(string filePath);

        /// <summary>
        ///     Performs post-processing procedures, i.e., after training.
        /// </summary>
        void PostProcess();
    }
}