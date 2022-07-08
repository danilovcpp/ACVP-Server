﻿using System;
using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Generation.Core.Async;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ParameterTypes;
using NLog;

namespace NIST.CVP.ACVTS.Libraries.Generation.LMS.v1_0.SigVer
{
    public class TestCaseGenerator : ITestCaseGeneratorAsync<TestGroup, TestCase>
    {
        private readonly IOracle _oracle;

        public int NumberOfTestCasesToGenerate { get; private set; } = 12;

        public TestCaseGenerator(IOracle oracle)
        {
            _oracle = oracle;
        }

        public async Task<TestCaseGenerateResponse<TestGroup, TestCase>> GenerateAsync(TestGroup group, bool isSample, int caseNo = -1)
        {
            if (isSample)
            {
                NumberOfTestCasesToGenerate = 4;
            }

            try
            {
                var param = new LmsSignatureParameters
                {
                    Disposition = group.TestCaseExpectationProvider.GetRandomReason().GetReason(),
                    Advance = 0,
                    Layers = group.LmsTypes.Count,
                    LmotsTypes = group.LmotsTypes.ToArray(),
                    LmsTypes = group.LmsTypes.ToArray()
                };

                var result = await _oracle.GetLmsVerifyResultAsync(param);

                var testCase = new TestCase
                {
                    Message = result.VerifiedValue.Message,
                    PublicKey = result.VerifiedValue.PublicKey,
                    Reason = param.Disposition,
                    TestPassed = result.Result,
                    Signature = result.VerifiedValue.Signature
                };

                return new TestCaseGenerateResponse<TestGroup, TestCase>(testCase);
            }
            catch (Exception ex)
            {
                ThisLogger.Error(ex);
                return new TestCaseGenerateResponse<TestGroup, TestCase>($"Error generating case: {ex.Message}");
            }
        }

        private static ILogger ThisLogger => LogManager.GetCurrentClassLogger();
    }
}