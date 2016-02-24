using System;
using System.IO;
using Newtonsoft.Json;

namespace EmoteEvents.ComplexData
{
    public enum RepetitionType
    {
        Undefined,
        OnceInASession,
        OnceAndForever
    }

    public class Utterance : JsonSerializable
    {
        private string _id;
        private string _library;
        private string _text;
        private string _category;
        private string _subcategory;
        private bool _isQuestion;
        private RepetitionType _repetitions = RepetitionType.Undefined;

        // Auxiliaries
        private string[] _textArray;
        private string[] _bookmarkArray;
        private string _thalamusID;

        /// <summary>
        /// The ID  identifying the utteranec in its library
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The excel library file where the utterance comes from
        /// </summary>
        public string Library
        {
            get { return _library; }
            set { _library = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string[] TextArray
        {
            get { return _textArray; }
            set { _textArray = value; }
        }

        public string[] BookmarkArray
        {
            get { return _bookmarkArray; }
            set { _bookmarkArray = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Subcategory
        {
            get { return _subcategory; }
            set { _subcategory = value; }
        }

        public bool IsQuestion
        {
            get { return _isQuestion; }
            set { _isQuestion = value; }
        }

        public RepetitionType Repetitions
        {
            get { return _repetitions; }
            set { _repetitions = value; }
        }

        /// <summary>
        /// The ID representing this Utterance when sent through Thalamus
        /// </summary>
        public string ThalamusId
        {
            get { return _thalamusID; }
            set { _thalamusID = value; }
        }


        public Utterance(string text, string id = "-1")
        {
            _text = text;
            _textArray = new string[1] { text };
            _id = id;
        }

        public Utterance()
        {
        }

        public Utterance(string id, string library, string text, string category, string subcategory, bool isQuestion, RepetitionType repetitions)
        {
            _id = id.Equals("") ? "Undefined" : id;
            _library = library;
            _text = text;
            _category = category;
            _subcategory = subcategory;
            _isQuestion = isQuestion;
            _repetitions = repetitions;
            _textArray = new string[1] { text };
            _bookmarkArray = new string[0];
        }

        public Utterance(string id, string library, string text, string category, string subcategory, string isQuestion, string repetitions)
        {
            _id = id.Equals("") ? "Undefined" : id;
            _library = library;
            _text = text;
            _category = category;
            _subcategory = subcategory;
            bool.TryParse(isQuestion, out _isQuestion);
            Enum.TryParse(repetitions, out _repetitions);
            _textArray = new string[1] { text };
            _bookmarkArray = new string[0];
        }

        public void SetTextBookmarks(string[] text, string[] bookmarks)
        {
            _textArray = text;
            _bookmarkArray = bookmarks;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ (_id == null ? 0 : _id.GetHashCode());
                hash = hash * 16777619 ^ (_library == null ? 0 : _library.GetHashCode());
                hash = hash * 16777619 ^ (_category == null ? 0 : _category.GetHashCode());
                hash = hash * 16777619 ^ (_subcategory == null ? 0 : _subcategory.GetHashCode());
                hash = hash * 16777619 ^ _isQuestion.GetHashCode();
                hash = hash * 16777619 ^ _repetitions.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Utterance)) return false;
            Utterance other = (Utterance)obj;
            bool libraries;
            if (other.Library != null && this.Library != null)
            {
                libraries = other.Library.Equals(this.Library);
            }
            else
            {
                if (other.Library == null && this.Library == null)
                    libraries = true;
                else
                    libraries = false;
            }
            bool texts;
            if (other.Text != null && this.Text != null)
            {
                texts = other.Text.Equals(this.Text);
            }
            else
            {
                if (other.Text == null && this.Text == null)
                    texts = true;
                else
                    texts = false;
            }
            bool categories;
            if (other.Category != null && this.Category != null)
            {
                categories = other.Category.Equals(this.Category);
            }
            else
            {
                if (other.Category == null && this.Category == null)
                    categories = true;
                else
                    categories = false;
            }
            bool subcategories;
            if (other.Subcategory != null && this.Subcategory != null)
            {
                subcategories = other.Subcategory.Equals(this.Subcategory);
            }
            else
            {
                if (other.Subcategory == null && this.Subcategory == null)
                    subcategories = true;
                else
                    subcategories = false;
            }
            return other.Id.Equals(this.Id) &&
                   libraries &&
                   texts &&
                   categories &&
                   subcategories &&
                   other.IsQuestion == this.IsQuestion &&
                   other.Repetitions == this.Repetitions;
        }

    }
}
