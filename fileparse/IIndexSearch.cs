// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;

namespace PFDB.Parsing
{
    public interface IIndexSearch
    {
        public List<int> ListOfIndices { get; }
        public StringComparison StringComparisonMethod { get; }
        public string Text { get; }
        public string? Word { get; }
        public bool isEmpty();

        public List<int> Search();
    }
}