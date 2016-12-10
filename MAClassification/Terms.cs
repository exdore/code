using System;
using System.Collections.Generic;
using System.Linq;

namespace MAClassification
{
    public class Terms : List<List<Term>>
    {
        public Terms TermsList;

        public Terms()
        {
            
        }

        public Terms(Terms terms, List<Attribute> attributes)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].IsUsed)
                {
                    i++;
                    continue;
                }
                Add(terms[i]);
            }
        }

        public double CumulativeProbability()
        {
            return this.Sum(term => term.Sum(item => item.Probability));
        }

        public Terms InitializeTerms(List<Attribute> attributes, Table data, List<string> results, out int attributesValuesCount)
        {
            TermsList = new Terms();
            attributesValuesCount = GetAllAttributesValuesCount(attributes);
            foreach (Attribute attribute in attributes)
            {
                TermsList.Add(new List<Term>());
                foreach (string attributeValue in attribute.AttributeValues)
                {
                    TermsList.Last().Add(new Term
                    {
                        AttributeName = attribute.AttributeName,
                        AttributeValue = attributeValue,
                        Entropy = data.CalculateGain(attribute.AttributeName, attributeValue, results),
                        IsChosen = false,
                    });
                }
            }
            return TermsList;
        }

        public double GetSumForEntopy(List<Attribute> attributes)
        {
            double result = 0;
            for (int i = 0; i < Count; i++)
            {
                if(attributes[i].IsUsed) continue;
                for (int j = 0; j < this[i].Count; j++)
                {
                    if(this[i][j].IsChosen) continue;
                    result += Math.Log(attributes[i].AttributeValues.Count, 2) - this[i][j].Entropy;
                }
            }
            return result;
        }

        public double GetSumForEurictic(List<Attribute> attributes)
        {
            double result = 0;
            for (int i = 0; i < Count; i++)
            {
                if (attributes[i].IsUsed) continue;
                for (int j = 0; j < this[i].Count; j++)
                {
                    if (this[i][j].IsChosen) continue;
                    result += this[i][j].WeightValue * this[i][j].EuristicFunctionValue;
                }
            }
            return result;
        }

        private static int GetAllAttributesValuesCount(List<Attribute> attributes)
        {
            var attributesValuesCount = 0;
            foreach (Attribute attribute in attributes)
            {
                if(attribute.IsUsed) continue;
                for (int j = 0; j < attribute.AttributeValues.Count; j++)
                {
                    attributesValuesCount++;
                }
            }
            return attributesValuesCount;
        }

        public void UpdateWeights(Rule rule)
        {
            double sumWeight = .0;
            foreach (var items in this)
            {
                foreach (var item in items)
                {
                    if (rule.ConditionsList.Exists(condition => condition.Attribute == item.AttributeName &&
                                                                condition.Value == item.AttributeValue))
                    {
                        item.IsChosen = true;
                        item.WeightValue += item.WeightValue * rule.Quality;
                    }
                    sumWeight += item.WeightValue;
                }
            }
            foreach (var items in this)
            {
                foreach (var item in items)
                {
                    if (!item.IsChosen)
                        item.WeightValue /= sumWeight;
                }
            }
        }
    }
}
