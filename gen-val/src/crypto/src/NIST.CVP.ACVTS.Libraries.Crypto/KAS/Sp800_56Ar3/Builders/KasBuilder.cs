﻿using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Sp800_56Ar3;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Sp800_56Ar3.Builders;

namespace NIST.CVP.ACVTS.Libraries.Crypto.KAS.Sp800_56Ar3.Builders
{
    public class KasBuilder : IKasBuilder
    {
        private ISchemeBuilder _schemeBuilder;

        public IKasBuilder WithSchemeBuilder(ISchemeBuilder value)
        {
            _schemeBuilder = value;
            return this;
        }

        public IKas Build()
        {
            return new Kas(_schemeBuilder.Build());
        }
    }
}
