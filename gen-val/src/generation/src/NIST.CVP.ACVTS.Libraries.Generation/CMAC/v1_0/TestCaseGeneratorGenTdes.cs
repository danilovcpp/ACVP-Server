﻿using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ParameterTypes;

namespace NIST.CVP.ACVTS.Libraries.Generation.CMAC.v1_0
{
    public class TestCaseGeneratorGenTdes : TestCaseGeneratorGenBase
    {
        public TestCaseGeneratorGenTdes(IOracle oracle) : base(oracle)
        {
        }

        protected override CmacParameters GetParam(TestGroup group)
        {
            return new CmacParameters()
            {
                CmacType = group.CmacType,
                CouldFail = false,
                KeyingOption = group.KeyingOption,
                KeyLength = group.KeyLength,
                MacLength = group.MacLength,
                PayloadLength = group.MessageLength
            };
        }
    }
}
