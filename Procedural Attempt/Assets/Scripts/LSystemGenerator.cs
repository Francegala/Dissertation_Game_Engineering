using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = true;
    [Range(0, 1)] public float chanceToIgnoreRule = 0.45f;//30% chance of not creating branch where it should be created
    public string GenerateSentence(string word = null)
    {
        if (word == null) word = rootSentence;

        return GrowRecursive(word);

    }

    private void Start()
    {
        //Debug.Log(GenerateSentence()+'\n');
    }

    private string GrowRecursive(string word, int iterationIndex = 0 )
    {
        if (iterationIndex >= iterationLimit) return word;
        StringBuilder newWord = new StringBuilder();

        foreach (var c in word)
        {
            newWord.Append(c);
            ProcessRulesRecursively(newWord, c, iterationIndex);
        }

        return newWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex)
    {
        foreach (var rule in rules)
        {
            if (rule.letter == c.ToString())
            {
                if (randomIgnoreRuleModifier)
                {
                    if (Random.value < chanceToIgnoreRule && iterationIndex>1)
                    {
                        return; // if lower than .45 we can ignore a  rule while creating output
                        //not perfect solution but gives more randomness
                    }
                }
                newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));
            }
        }
    }
}
