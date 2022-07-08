﻿using System.Threading.Tasks;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ParameterTypes;
using NIST.CVP.ACVTS.Libraries.Oracle.Abstractions.ResultTypes;
using Orleans;

namespace NIST.CVP.ACVTS.Libraries.Orleans.Grains.Interfaces.Dsa
{
    public interface IOracleObserverDsaKeyCaseGrain : IGrainWithGuidKey, IGrainObservable<DsaKeyResult>
    {
        Task<bool> BeginWorkAsync(DsaKeyParameters param);
    }
}