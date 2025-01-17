﻿using System;
using System.Numerics;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Asymmetric.DSA.ECC;

namespace NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Helpers
{
    public static class KeyValidationHelper
    {
        public static bool PerformFfcPublicKeyValidation(BigInteger P, BigInteger Q, BigInteger y, bool shouldThrow = false)
        {
            BigInteger two = new BigInteger(2);

            //Verify that 2 <= y <= p-2
            //Verify that y^q = 1 mod p
            //if either fail output error indicator PUBKEYVALERROR
            if ((y >= two) && (y <= (P - two)))
            {
                var res = BigInteger.ModPow(y, Q, P);
                if (res == BigInteger.One)
                {
                    return true; //good
                }
            }

            if (shouldThrow)
            {
                throw new Exception("public key validation error");
            }

            return false;
        }

        public static bool PerformEccPublicKeyValidation(IEccCurve curve, EccPoint publicKey, bool shouldThrow = false)
        {
            if (!curve.PointExistsOnCurve(publicKey))
            {
                if (shouldThrow)
                {
                    throw new Exception("public key does not exist on curve");
                }

                return false;
            }

            var n = curve.OrderN;
            var nQ = curve.Multiply(publicKey, n);

            if (!nQ.Infinity)
            {
                if (shouldThrow)
                {
                    throw new Exception("public key validation error");
                }

                return false;
            }

            return true;
        }
    }
}
