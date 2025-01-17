﻿using System;
using System.Linq;
using Newtonsoft.Json.Serialization;
using NIST.CVP.ACVTS.Libraries.Generation.Core.ContractResolvers;

namespace NIST.CVP.ACVTS.Libraries.Generation.CSHAKE.v1_0.ContractResolvers
{
    public class PromptProjectionContractResolver : ProjectionContractResolverBase<TestGroup, TestCase>
    {
        protected override Predicate<object> TestGroupSerialization(JsonProperty jsonProperty)
        {
            var mctProperties = new[]
            {
                nameof(TestGroup.MinOutputLength),
                nameof(TestGroup.MaxOutputLength)
            };

            if (mctProperties.Contains(jsonProperty.UnderlyingName, StringComparer.OrdinalIgnoreCase))
            {
                return jsonProperty.ShouldSerialize = instance =>
                {
                    GetTestGroupFromTestGroupObject(instance, out var testGroup);
                    return testGroup.TestType.Equals("mct", StringComparison.OrdinalIgnoreCase);
                };
            }

            return jsonProperty.ShouldSerialize = instance => true;
        }

        protected override Predicate<object> TestCaseSerialization(JsonProperty jsonProperty)
        {
            var includeProperties = new[]
            {
                nameof(TestCase.TestCaseId),
                nameof(TestCase.Message),
                nameof(TestCase.MessageLength),
                nameof(TestCase.FunctionName)
            };

            if (includeProperties.Contains(jsonProperty.UnderlyingName, StringComparer.OrdinalIgnoreCase))
            {
                return jsonProperty.ShouldSerialize = instance => true;
            }

            switch (jsonProperty.UnderlyingName)
            {
                case nameof(TestCase.Customization):
                    return jsonProperty.ShouldSerialize = instance =>
                    {
                        GetTestCaseFromTestCaseObject(instance, out var testGroup, out var testCase);
                        return !testGroup.HexCustomization;
                    };
                case nameof(TestCase.CustomizationHex):
                    return jsonProperty.ShouldSerialize = instance =>
                    {
                        GetTestCaseFromTestCaseObject(instance, out var testGroup, out var testCase);
                        return testGroup.HexCustomization;
                    };
                case nameof(TestCase.DigestLength):
                    return jsonProperty.ShouldSerialize = instance =>
                    {
                        GetTestCaseFromTestCaseObject(instance, out var testGroup, out var testCase);
                        return testGroup.TestType.Equals("aft", StringComparison.OrdinalIgnoreCase);
                    };
                default:
                    return jsonProperty.ShouldSerialize = instance => false;
            }
        }
    }
}
