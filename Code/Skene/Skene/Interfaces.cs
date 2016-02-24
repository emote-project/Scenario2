using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Thalamus;

namespace Skene.Interfaces
{

    public class LibraryInfo
    {
        public String LibraryName {get; set;}
        public Dictionary<string, List<string>> Categories {get; set;}

        public LibraryInfo(string libraryName, Dictionary<string, List<string>> categories)
        {
            LibraryName = libraryName;
            Categories = categories;
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            (new JsonSerializer()).Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static LibraryInfo DeserializeFromJson(string serialized)
        {
            try
            {
                return (LibraryInfo)(new JsonSerializer()).Deserialize(new StringReader(serialized), typeof(LibraryInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize LibraryInfo from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }

    public interface ILibraryActions : IAction
    {
        void ChangeLibrary(string newLibrary);
        void GetLibraries();
        void GetUtterances(string category, string subcategory);
    }

    public interface ILibraryEvents : IPerception
    {
        void LibraryList(string[] libraries);
        void LibraryChanged(string serialized_LibraryContents);
        void Utterances(string library, string category, string subcategory, string[] utterances);
    }
}
