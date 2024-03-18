using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PFDB
{
    namespace Parsing
    {
        public sealed class IndexSearch : IIndexSearch
		{
			public string Text { get; init; }
			public string? Word { get; init; }
			public StringComparison StringComparisonMethod { get; init; }
			public List<int> ListOfIndices { get; private set; }



			public IndexSearch(string text, string? word)
			{
				ListOfIndices = new List<int>();
				Text = text;
				Word = word;
				StringComparisonMethod = StringComparison.InvariantCultureIgnoreCase;
				Search(); //automatically searches
			}

			public IndexSearch(string text, string? word, StringComparison stringComparisonMethod)
			{
				ListOfIndices = new List<int>();
				Text = text;
				Word = word;
				StringComparisonMethod = stringComparisonMethod;
				Search(); //automatically searches
			}

			public bool isEmpty()
			{
				return ListOfIndices.Count == 0;
			}

			public void RemoveFromList(IEnumerable<int> list)
			{
				ListOfIndices = ListOfIndices.Except(list).ToList();

			}

			public IEnumerable<int> Search()
			{
				int startIndex = 0;
				try
				{
					startIndex = Text.LastIndexOf(Environment.GetEnvironmentVariable("pythonSignalText"));
					
				}
				catch { }
				if (startIndex < 0 || startIndex > Text.Length - 1)
				{
					startIndex = 0;
				}


				if (Word == null) return ListOfIndices; //early return if null
				string _filetext = Text.Substring(startIndex);
				bool _isChar = Word.Length == 1;
				for (; //if we are searching for a single character, use char overload for String.Contains()
					(_isChar) ? _filetext.Contains(Word[0], StringComparisonMethod) : _filetext.Contains(Word, StringComparisonMethod);
					)
				{
					try
					{
						((List<int>)ListOfIndices).Add(
							(_isChar) ? _filetext.LastIndexOf(Word[0]) : _filetext.LastIndexOf(Word, StringComparisonMethod)
							);
						_filetext = (_isChar) ? _filetext.Remove(_filetext.LastIndexOf(Word[0])) : _filetext.Remove(_filetext.LastIndexOf(Word, StringComparisonMethod), Word.Length);
					}
					catch (ArgumentOutOfRangeException) { break; }
				}
				return ListOfIndices;
			}
		}
	}
}