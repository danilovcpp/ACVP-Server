﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Symmetric.TDES;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Generation.Core.Async;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ParameterTypes;
using NLog;

namespace NIST.CVP.ACVTS.Libraries.Generation.TDES_OFBI.v1_0
{
    public class TestCaseGeneratorMonteCarlo : ITestCaseGeneratorAsync<TestGroup, TestCase>
    {
        private readonly IOracle _oracle;

        public int NumberOfTestCasesToGenerate => 1;

        public TestCaseGeneratorMonteCarlo(IOracle oracle)
        {
            _oracle = oracle;
        }

        public async Task<TestCaseGenerateResponse<TestGroup, TestCase>> GenerateAsync(TestGroup group, bool isSample, int caseNo = 0)
        {
            var param = new TdesParameters
            {
                Mode = BlockCipherModesOfOperation.Ofbi,
                DataLength = 64 * 3, // This mode operates on a minimum of 3 blocks
                Direction = group.Function,
                KeyingOption = group.KeyingOption
            };

            try
            {
                var oracleResult = await _oracle.GetTdesMctCaseAsync(param);

                return new TestCaseGenerateResponse<TestGroup, TestCase>(new TestCase
                {
                    PlainText = oracleResult.Seed.PlainText,
                    CipherText = oracleResult.Seed.CipherText,
                    Keys = oracleResult.Seed.Key,
                    IV = oracleResult.Seed.Iv,
                    ResultsArray = Array.ConvertAll(oracleResult.Results.ToArray(), element => new AlgoArrayResponse
                    {
                        PlainText = element.PlainText,
                        CipherText = element.CipherText,
                        Keys = element.Key,
                        IV = element.Iv
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                ThisLogger.Error(ex);
                return new TestCaseGenerateResponse<TestGroup, TestCase>($"Failed to generate. {ex.Message}");
            }
        }

        private static ILogger ThisLogger => LogManager.GetCurrentClassLogger();
    }
}
