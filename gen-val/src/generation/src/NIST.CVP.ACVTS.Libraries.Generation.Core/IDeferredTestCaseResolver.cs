﻿using System;
using NIST.CVP.ACVTS.Libraries.Crypto.Common;

namespace NIST.CVP.ACVTS.Libraries.Generation.Core
{
    /// <summary>
    /// Describes functions necessary for completing a deferred crypto operation
    /// </summary>
    /// <typeparam name="TTestGroup">The <see cref="ITestGroup{TTestGroup,TTestCase}"/></typeparam>
    /// <typeparam name="TTestCase">The <see cref="ITestCase{TTestGroup,TTestCase}"/></typeparam>
    /// <typeparam name="TCryptoResult">The <see cref="ICryptoResult"/> of the completed crypto operation</typeparam>
    [Obsolete("Being replace by IDeferredTestCaseResolverAsync")]
    public interface IDeferredTestCaseResolver<in TTestGroup, in TTestCase, out TCryptoResult>
        where TTestGroup : ITestGroup<TTestGroup, TTestCase>
        where TTestCase : ITestCase<TTestGroup, TTestCase>
        where TCryptoResult : ICryptoResult
    {
        /// <summary>
        /// Using the provided <see cref="serverTestGroup"/>, <see cref="serverTestCase"/>, 
        /// and <see cref="iutTestCase"/>, complete and return a <see cref="ICryptoResult"/>
        /// </summary>
        /// <param name="serverTestGroup">Group information to be utilized in the crypto</param>
        /// <param name="serverTestCase">The pre-generated server information to use in the crypto operation</param>
        /// <param name="iutTestCase">The IUTs provided information necessary to complete the crypto.</param>
        /// <returns>The completed crypto result</returns>
        TCryptoResult CompleteDeferredCrypto(
            TTestGroup serverTestGroup,
            TTestCase serverTestCase,
            TTestCase iutTestCase
        );
    }
}
