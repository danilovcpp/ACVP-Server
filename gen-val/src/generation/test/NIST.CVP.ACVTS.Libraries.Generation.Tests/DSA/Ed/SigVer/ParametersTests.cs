﻿using NIST.CVP.ACVTS.Libraries.Generation.EDDSA.v1_0.SigVer;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;

namespace NIST.CVP.ACVTS.Libraries.Generation.Tests.DSA.Ed.SigVer
{
    [TestFixture, UnitTest]
    public class ParametersTests
    {
        [Test]
        public void ShouldCoverParametersSet()
        {
            var parameters = new Parameters
            {
                Algorithm = "EDDSA",
                Mode = "SigVer",
                IsSample = false,
                Curve = ParameterValidator.VALID_CURVES
            };

            Assert.IsNotNull(parameters);
        }

        [Test]
        public void ShouldCoverParametersGet()
        {
            var parameters = new Parameters
            {
                Algorithm = "EDDSA",
                Mode = "SigVer",
                IsSample = false,
                Curve = ParameterValidator.VALID_CURVES
            };

            Assert.AreEqual("EDDSA", parameters.Algorithm);
        }
    }
}
