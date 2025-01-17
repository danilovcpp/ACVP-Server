﻿using NIST.CVP.ACVTS.Libraries.Crypto.Common.Asymmetric.RSA;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Asymmetric.RSA.Keys;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Asymmetric.RSA.Signatures;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Libraries.Math.Helpers;

namespace NIST.CVP.ACVTS.Libraries.Crypto.RSA.Signatures
{
    public class SignatureBuilder : ISignatureBuilder
    {
        private BitString _message;
        private BitString _signature;
        private PublicKey _publicKey;
        private PrivateKeyBase _privateKey;
        private KeyPair _key;
        private IPaddingScheme _paddingScheme;
        private IRsa _rsa;

        public ISignatureBuilder WithMessage(BitString message)
        {
            _message = message;
            return this;
        }

        public ISignatureBuilder WithSignature(BitString signature)
        {
            _signature = signature;
            return this;
        }

        public ISignatureBuilder WithPrivateKey(PrivateKeyBase privKey)
        {
            _privateKey = privKey;
            return this;
        }

        public ISignatureBuilder WithPublicKey(PublicKey pubKey)
        {
            _publicKey = pubKey;
            return this;
        }

        public ISignatureBuilder WithKey(KeyPair key)
        {
            _publicKey = key.PubKey;
            _privateKey = key.PrivKey;
            return this;
        }

        public ISignatureBuilder WithPaddingScheme(IPaddingScheme paddingScheme)
        {
            _paddingScheme = paddingScheme;
            return this;
        }

        public ISignatureBuilder WithDecryptionScheme(IRsa rsa)
        {
            _rsa = rsa;
            return this;
        }

        public VerifyResult BuildVerify()
        {
            // Check for empty values that prevent building
            if (_paddingScheme == null || _rsa == null || _publicKey == null || _message == null || _signature == null)
            {
                return new VerifyResult("Improper verify build");
            }

            // Encrypt with the public key to open the signature
            var encryptionResult = _rsa.Encrypt(_signature.ToPositiveBigInteger(), _publicKey);

            if (!encryptionResult.Success)
            {
                return new VerifyResult(encryptionResult.ErrorMessage);
            }

            // Verify the padding
            return _paddingScheme.VerifyPadding(_publicKey.N.ExactBitLength(), _message, encryptionResult.CipherText, _publicKey);
        }

        public SignatureResult BuildSign()
        {
            // Check for empty values that prevent building
            if (_paddingScheme == null || _rsa == null || _publicKey == null || _privateKey == null || _message == null)
            {
                return new SignatureResult("Improper signature build");
            }

            _key = new KeyPair { PubKey = _publicKey, PrivKey = _privateKey };

            // Do provided padding method either correct or incorrect
            var preCheck = _paddingScheme.PrePadCheck(_key, _message.GetDeepCopy(), _publicKey.N.ExactBitLength());

            var paddedResult = _paddingScheme.Pad(preCheck.key.PubKey.N.ExactBitLength(), preCheck.message);
            if (!paddedResult.Success)
            {
                return new SignatureResult(paddedResult.ErrorMessage);
            }

            // Perform the RSA Decryption
            var decryptionResult = _rsa.Decrypt(paddedResult.PaddedMessage.ToPositiveBigInteger(), preCheck.key.PrivKey, preCheck.key.PubKey);

            if (!decryptionResult.Success)
            {
                return new SignatureResult(decryptionResult.ErrorMessage);
            }

            // Perform the Post-Check depending on the padding scheme
            var postCheck = _paddingScheme.PostSignCheck(decryptionResult.PlainText, preCheck.key.PubKey);

            return new SignatureResult(postCheck);
        }
    }
}

