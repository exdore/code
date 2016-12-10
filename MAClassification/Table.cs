using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MAClassification
{
    public class Table
    {
        public List<string> Header { get; set; }
        public List<Case> Cases { get; set; }
        public int CountColumns
        {
            get
            {
                return Cases[0].AttributesValuesList.Count;
            }
        }

        public Table(Table data)
        {
            Cases = data.Cases;
            Header = data.Header;
        }

        public Table()
        {
            
        }

        public List<Attribute> GetAttributesInfo()
        {
            List<Attribute> attributes = new List<Attribute>();
            for (int i = 0; i < CountColumns; i++)
            {
                attributes.Add(new Attribute
                {
                    AttributeName = Header[i],
                    AttributeValues = Cases.Select(item => item.AttributesValuesList[i]).Distinct().ToList(),
                    IsUsed = false
                });
            }
            return attributes;
        }

        public List<Case> GetCases()
        {
            return Cases;
        }

        public double CalculateGain(string attributeName, string attributeValue, List<string> resultsList)
        {
            double result = 0;
            var attributeIndex = Header.IndexOf(attributeName);
            var appropriateCases = Cases.Where(item => item.AttributesValuesList[attributeIndex] == attributeValue).ToList();
            var appropriateCasesCount = appropriateCases.Count;
            foreach (var sample in resultsList)
            {
                var casesWithSetResultCount = appropriateCases.Count(item => item.Result == sample);
                result -= (double)casesWithSetResultCount/appropriateCasesCount*
                          Math.Log((double)casesWithSetResultCount/appropriateCasesCount, 2);
            }
            return result;
        }

        public static Table ReadData()
        {
            var streamReader = new StreamReader(@"data.txt", System.Text.Encoding.GetEncoding(1251));
            var sourceData = new List<Case>();
            var line = streamReader.ReadLine();
            if (line != null)
            {
                var header = line.Split('\t').ToList();
                header.RemoveAt(0);
                header.RemoveAt(header.Count - 1);
                while ((line = streamReader.ReadLine()) != null)
                {
                    var sourceList = line.Split('\t').ToList();
                    sourceData.Add(new Case
                    {
                        Number = Convert.ToInt32(sourceList[0]),
                        AttributesValuesList = sourceList.GetRange(1, sourceList.Count - 2),
                        Result = sourceList[sourceList.Count - 1]
                    });
                }
                streamReader.Close();
                return new Table
                {
                    Header = header,
                    Cases = sourceData
                };
            }
            return null;
        }

        public List<string> GetResultsInfo()
        {
            return Cases.Select(item => item.Result).Distinct().ToList();
        }
    }
}
