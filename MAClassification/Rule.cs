using System;
using System.Collections.Generic;
using System.Linq;

namespace MAClassification
{
    public class Rule 
    {
        public List<Condition> ConditionsList;

        public string Result { get; set; }

        public double Quality { get; set; }

        public List<Case> CoveredCases { get; set; }

        public void GetRuleResult(List<string> resultsList)
        {
            var result = "";
            int max = 0;
            foreach (var item in resultsList)
            {
                if (max < CoveredCases.Count(@case => @case.Result == item))
                {
                    result = item;
                    max = CoveredCases.Count(@case => @case.Result == item);
                }
            }
            Result = result;
        }

        public void CalculateRuleQuality(Table data)
        {
            Rule tmpThis = this;
            int truePositive = tmpThis.CoveredCases.Count(item => item.Result == tmpThis.Result);
            int falsePositive = CoveredCases.Count(item => item.Result != tmpThis.Result);
            var uncoveredData = data.GetCases().Except(CoveredCases).ToList();
            int trueNegative = uncoveredData.Count(item => item.Result == tmpThis.Result);
            int falseNegative = uncoveredData.Count(item => item.Result != tmpThis.Result);
            Quality = (double)truePositive*trueNegative/(truePositive + falseNegative)/(falsePositive + trueNegative);
        }

        public void AddConditionToRule(Terms terms, Table data)
        {
            ConditionsList = ConditionsList ?? new List<Condition>();
            var probability = GetSomeProbability();
            foreach (var term in terms)
            {
                foreach (var item in term)
                {
                    if (probability < item.Probability)
                    {
                        ConditionsList.Add(new Condition
                        {
                            Attribute = item.AttributeName,
                            Value = item.AttributeValue
                        });
                        return;
                    }
                    probability -= item.Probability;
                }
            }
        }

        public double GetSomeProbability()
        {
            return new Random().NextDouble();
        }

        public void GetCoveredCases(Table data)
        {
            var cases = data.GetCases();
            foreach (var condition in ConditionsList)
            {
                int attributeIndex = data.Header.FindIndex(item => item == condition.Attribute);
                cases = cases.Where(item => item.AttributesValuesList[attributeIndex] == condition.Value).ToList();
            }
            CoveredCases = cases;
        }

        public void CheckUsedAttributes(List<Attribute> attributes)
        {
            foreach (var condition in ConditionsList)
            {
                var index = attributes.FindIndex(item => item.AttributeName == condition.Attribute);
                attributes[index].IsUsed = true;
            }
        }

        public Rule Clone()
        {
            return new Rule { 
                ConditionsList = ConditionsList.ToList(),
                Quality = Quality,
                CoveredCases = CoveredCases.ToList(),
                Result = Result
            };
        }

        public Rule PruneRule(Table data, List<string> resultsList)
        {
            var newRule = Clone();
            if(newRule.ConditionsList.Count == 1) return newRule;
            var rulesList = new List<Rule>();
            for (int i = 0; i < ConditionsList.Count; i++)
            {
                newRule = Clone();
                newRule.ConditionsList.RemoveAt(i);
                newRule.GetCoveredCases(data);
                newRule.GetRuleResult(resultsList);
                newRule.CalculateRuleQuality(data);
                rulesList.Add(newRule);
            }
            var bestRule = rulesList.OrderByDescending(item => item.Quality).First();
            return (bestRule.Quality > Quality) ? bestRule.PruneRule(data, resultsList) : this;
        }
    }
}
