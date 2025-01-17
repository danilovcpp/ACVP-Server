﻿using System.Collections.Generic;
using Newtonsoft.Json;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Math.Domain;

namespace NIST.CVP.ACVTS.Libraries.Generation.SHA2.v1_0
{
    public class Parameters : IParameters
    {
        public int VectorSetId { get; set; }
        public string Algorithm { get; set; }
        public string Mode { get; set; }
        public string Revision { get; set; }
        public bool IsSample { get; set; }
        public string[] Conformances { get; set; } = { };

        [JsonProperty(PropertyName = "digestSize")]
        public List<string> DigestSizes { get; set; }

        [JsonProperty(PropertyName = "messageLength")]
        public MathDomain MessageLength { get; set; }

        [JsonProperty(PropertyName = "performLargeDataTest")]
        public int[] PerformLargeDataTest { get; set; } = { };
    }
}
