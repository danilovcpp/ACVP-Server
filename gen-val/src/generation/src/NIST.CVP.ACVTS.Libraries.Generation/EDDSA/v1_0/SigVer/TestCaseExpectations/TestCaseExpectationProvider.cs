﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NIST.CVP.ACVTS.Libraries.Common.ExtensionMethods;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.DispositionTypes;

namespace NIST.CVP.ACVTS.Libraries.Generation.EDDSA.v1_0.SigVer.TestCaseExpectations
{
    public class TestCaseExpectationProvider : ITestCaseExpectationProvider<EddsaSignatureDisposition>
    {
        private readonly ConcurrentQueue<TestCaseExpectationReason> _expectationReasons;

        public TestCaseExpectationProvider(bool isSample = false)
        {
            var expectationReasons = new List<TestCaseExpectationReason>();

            if (isSample)
            {
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.None));
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyMessage));
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyKey));
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyR));
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyS));
            }
            else
            {
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.None), 3);
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyMessage), 3);
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyKey), 3);
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyR), 3);
                expectationReasons.Add(new TestCaseExpectationReason(EddsaSignatureDisposition.ModifyS), 3);
            }

            _expectationReasons = new ConcurrentQueue<TestCaseExpectationReason>(expectationReasons.Shuffle());
        }

        public int ExpectationCount => _expectationReasons.Count;

        public ITestCaseExpectationReason<EddsaSignatureDisposition> GetRandomReason()
        {
            if (_expectationReasons.TryDequeue(out var reason))
            {
                return reason;
            }

            throw new IndexOutOfRangeException($"No {nameof(TestCaseExpectationReason)} remaining to pull");
        }
    }
}
