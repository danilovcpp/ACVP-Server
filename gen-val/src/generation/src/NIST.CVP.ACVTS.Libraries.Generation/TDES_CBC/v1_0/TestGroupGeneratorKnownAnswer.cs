﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Generation.Core;

namespace NIST.CVP.ACVTS.Libraries.Generation.TDES_CBC.v1_0
{
    public class TestGroupGeneratorKnownAnswer : ITestGroupGeneratorAsync<Parameters, TestGroup, TestCase>
    {
        private readonly string[] _katTests = new string[]
        {
            "Permutation",
            "InversePermutation",
            "SubstitutionTable",
            "VariableKey",
            "VariableText"
        };

        public Task<List<TestGroup>> BuildTestGroupsAsync(Parameters parameters)
        {
            var testGroups = new List<TestGroup>();
            foreach (var function in parameters.Direction)
            {
                foreach (var katTest in _katTests)
                {
                    TestGroup tg = new TestGroup()
                    {
                        Function = function,
                        KeyingOption = 1,
                        InternalTestType = katTest
                    };

                    testGroups.Add(tg);
                }
            }
            return Task.FromResult(testGroups);
        }
    }
}
