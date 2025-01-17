﻿using System;
using System.Collections.Generic;
using System.Linq;
using NIST.CVP.ACVTS.Libraries.Common.ExtensionMethods;
using NIST.CVP.ACVTS.Libraries.Common.Helpers;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KDF.Enums;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Math;

namespace NIST.CVP.ACVTS.Libraries.Generation.KDF.v1_0
{
    public class ParameterValidator : ParameterValidatorBase, IParameterValidator<Parameters>
    {
        public static KdfModes[] VALID_KDF_MODES = { KdfModes.Counter, KdfModes.Feedback, KdfModes.Pipeline };

        public static MacModes[] VALID_MAC_MODES =
        {
            MacModes.CMAC_AES128,
            MacModes.CMAC_AES192,
            MacModes.CMAC_AES256,
            MacModes.CMAC_TDES,
            MacModes.HMAC_SHA1,
            MacModes.HMAC_SHA224,
            MacModes.HMAC_SHA256,
            MacModes.HMAC_SHA384,
            MacModes.HMAC_SHA512,
            MacModes.HMAC_SHA_d512t224,
            MacModes.HMAC_SHA_d512t256,
            MacModes.HMAC_SHA3_224,
            MacModes.HMAC_SHA3_256,
            MacModes.HMAC_SHA3_384,
            MacModes.HMAC_SHA3_512
        };

        public static CounterLocations[] VALID_DATA_ORDERS =
        {
            CounterLocations.None,
            CounterLocations.AfterFixedData,
            CounterLocations.BeforeFixedData,
            CounterLocations.BeforeIterator,
            CounterLocations.MiddleFixedData
        };
        public static int[] VALID_COUNTER_LENGTHS = { 0, 8, 16, 24, 32 };
        public static int MAX_DATA_LENGTH = 4096;
        public static int MIN_DATA_LENGTH = 1;
        public static int KeyInLenMin = 112;
        public static int KeyInLenMax = 4096;

        public ParameterValidateResponse Validate(Parameters parameters)
        {
            var errors = new List<string>();

            if (errors.AddIfNotNullOrEmpty(ValidateArrayAtLeastOneItem(parameters.Capabilities, "Capabilities")))
            {
                return new ParameterValidateResponse(errors);
            }

            foreach (var capability in parameters.Capabilities)
            {
                string result;
                result = ValidateArray(new[] { capability.KdfMode }, VALID_KDF_MODES, "KDF Mode");
                errors.AddIfNotNullOrEmpty(result);

                result = ValidateArray(capability.MacMode, VALID_MAC_MODES, "MAC Modes");
                errors.AddIfNotNullOrEmpty(result);

                result = ValidateArray(capability.CounterLength, VALID_COUNTER_LENGTHS, "Counter Lengths");
                errors.AddIfNotNullOrEmpty(result);

                ValidateFixedDataOrder(capability, errors);
                ValidateCustomKeyInLen(capability, errors);

                if (errors.AddIfNotNullOrEmpty(ValidateSegmentCountGreaterThanZero(capability.SupportedLengths, "Supported Lengths")))
                {
                    errors.Add("No supported lengths provided");
                    continue;
                }

                if (capability.SupportedLengths.GetDomainMinMax().Minimum < MIN_DATA_LENGTH)
                {
                    errors.Add("Minimum output length must be greater than 0");
                }

                if (capability.SupportedLengths.GetDomainMinMax().Maximum > MAX_DATA_LENGTH)
                {
                    errors.Add("Maximum output length must be less than or equal to 4096");
                }
            }

            return new ParameterValidateResponse(errors);
        }

        private void ValidateCustomKeyInLen(Capability capability, List<string> errors)
        {
            if (capability.CustomKeyInLength == 0)
            {
                return;
            }

            if (capability.CustomKeyInLength < KeyInLenMin || capability.CustomKeyInLength > KeyInLenMax)
            {
                errors.Add($"Provided {nameof(capability.CustomKeyInLength)} of {capability.CustomKeyInLength} must be between {KeyInLenMin} and {KeyInLenMax}.");
            }

            if (capability.CustomKeyInLength % BitString.BITSINBYTE != 0)
            {
                errors.Add($"Provided {nameof(capability.CustomKeyInLength)} of {capability.CustomKeyInLength} must be evenly divisible by 8.");
            }

            ValidateKeyInSymmetricCipher(MacModes.CMAC_AES128, 128, capability, errors);
            ValidateKeyInSymmetricCipher(MacModes.CMAC_AES192, 192, capability, errors);
            ValidateKeyInSymmetricCipher(MacModes.CMAC_AES256, 256, capability, errors);
            ValidateKeyInSymmetricCipher(MacModes.CMAC_TDES, 192, capability, errors);
        }

        private void ValidateKeyInSymmetricCipher(MacModes macMode, int allowedKeyIn, Capability capability, List<string> errors)
        {
            if (capability.MacMode.Contains(macMode) && capability.CustomKeyInLength != allowedKeyIn)
            {
                errors.Add($"Provided {nameof(capability.CustomKeyInLength)} of {capability.CustomKeyInLength} is not valid for {EnumHelpers.GetEnumDescriptionFromEnum(macMode)}.");
            }
        }

        private void ValidateFixedDataOrder(Capability capability, List<string> errors)
        {
            var result = ValidateArray(capability.FixedDataOrder, VALID_DATA_ORDERS, "Data Orders");
            if (!string.IsNullOrEmpty(result))
            {
                errors.Add(result);
                return;
            }

            if (capability.KdfMode == KdfModes.Counter)
            {
                if (capability.FixedDataOrder.Contains(CounterLocations.None))
                {
                    errors.Add("none FixedDataOrder is not valid with Counter KDF");
                }

                if (capability.CounterLength.Contains(0))
                {
                    errors.Add("Counter KDF requires a non-zero counter length");
                }

                if (capability.FixedDataOrder.Contains(CounterLocations.BeforeIterator))
                {
                    errors.Add("before iterator FixedDataOrder is not valid with Counter KDF");
                }
            }
            else
            {
                if (capability.FixedDataOrder.Contains(CounterLocations.MiddleFixedData))
                {
                    errors.Add("middle fixed data FixedDataOrder is not valid with non-Counter KDFs");
                }

                if (capability.FixedDataOrder.Contains(CounterLocations.None) && !capability.CounterLength.Contains(0))
                {
                    errors.Add("none FixedDataOrder must be paired with 0 counterLength");
                }

                if (capability.CounterLength.Contains(0) && !capability.FixedDataOrder.Contains(CounterLocations.None))
                {
                    errors.Add("0 counterLength must be paired with none FixedDataOrder");
                }
            }
        }
    }
}
