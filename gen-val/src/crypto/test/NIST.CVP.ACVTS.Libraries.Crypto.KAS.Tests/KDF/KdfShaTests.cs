﻿using System;
using System.Numerics;
using NIST.CVP.ACVTS.Libraries.Crypto.CMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.Hash.ShaWrapper;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.Enums;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.FixedInfo;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KDF;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.KAS.KDF.KdfOneStep;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.MAC.CMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.MAC.HMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.Common.MAC.KMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.CSHAKE;
using NIST.CVP.ACVTS.Libraries.Crypto.HKDF;
using NIST.CVP.ACVTS.Libraries.Crypto.HMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.IKEv1;
using NIST.CVP.ACVTS.Libraries.Crypto.IKEv2;
using NIST.CVP.ACVTS.Libraries.Crypto.KAS.FixedInfo;
using NIST.CVP.ACVTS.Libraries.Crypto.KAS.KDF;
using NIST.CVP.ACVTS.Libraries.Crypto.KAS.KDF.OneStep;
using NIST.CVP.ACVTS.Libraries.Crypto.KMAC;
using NIST.CVP.ACVTS.Libraries.Crypto.SHA.NativeFastSha;
using NIST.CVP.ACVTS.Libraries.Crypto.Symmetric.BlockModes;
using NIST.CVP.ACVTS.Libraries.Crypto.Symmetric.Engines;
using NIST.CVP.ACVTS.Libraries.Crypto.TLS;
using NIST.CVP.ACVTS.Libraries.Math;
using NIST.CVP.ACVTS.Tests.Core.TestCategoryAttributes;
using NUnit.Framework;
using KdfFactory = NIST.CVP.ACVTS.Libraries.Crypto.KDF.KdfFactory;

namespace NIST.CVP.ACVTS.Libraries.Crypto.KAS.Tests.KDF
{
    [TestFixture, FastCryptoTest]
    public class KdfShaTests
    {
        private KdfSha _subject;
        private IShaFactory _shaFactory;

        private IKdfVisitor _kdfVisitor;

        [SetUp]
        public void Setup()
        {
            _shaFactory = new NativeShaFactory();
            IHmacFactory hmacFactory = new HmacFactory(_shaFactory);
            IKmacFactory kmacFactory = new KmacFactory(new CSHAKEWrapper());
            ICmacFactory cmacFactory = new CmacFactory(new BlockCipherEngineFactory(), new ModeBlockCipherFactory());

            _kdfVisitor = new KdfVisitor(
                new KdfOneStepFactory(_shaFactory, hmacFactory, kmacFactory),
                new KdfFactory(cmacFactory, hmacFactory),
                hmacFactory,
                cmacFactory,
                new IkeV1Factory(hmacFactory, _shaFactory),
                new IkeV2Factory(hmacFactory),
                new TlsKdfFactory(hmacFactory),
                new HkdfFactory(hmacFactory));

        }

        private static object[] _kdfTestCases = new object[]
        {
            new object[]
            {
                "test1",
                ModeValues.SHA2,
                DigestSizes.d256,
                128,
                // z
                new BitString("3f4863bf95277b51c6112191e9e9a042610b533af346824cfef15a18ec9906ab193eac3c51be6ef23f050004215bd275b824e9b66e115a2610b2645bf0345f06e441b6420687358fe4e69a32942bc399efa4a3e757e725a209bac0fa46adcdb26cb466128ff8b79c8f437595a73889020ac02f606c775570a5d048734d7b85fafa8df8ed5c6d34fec3acf1288a4efdb1811d26548a360f62151ddbc84f6f424adf87188e26a97f23ec8a0817a0166359ad1607f4933a7f0de22c1401fa755ba384faeacf7a3cc59a69ebd3293a4df6548b80efc661334ec3eb10dcb66a9b871d2284e983456cefe03e33eaffb6cc2a1ff67ca93274c18871d11eeacaa72a7671").ToPositiveBigInteger(),
                // otherInfo
                new BitString("434156536964a1b2c3d4e52009a24c98c92d8a86461fbb212c8193db7a69"),
                // expectedDerivedKeyingMaterial
                new BitString("72d5339d984529cf25ecca527c273282")
            },
            new object[]
            {
                "test2",
                ModeValues.SHA2,
                DigestSizes.d256,
                128,
                // z
                new BitString("8e984e8bd2ac95ca61453453baf84bb854d6889fb61f1043d48a1c88726dee411c30aa7a3638e571aa07080bcffe81f4330ab326265817bea0ee60bc708950c4ec93199dcb5c5e94c164471ee5a46b8fa22d139a9ae0531ab4a2ed5883141a0e224ec7d0a84803d98b82ce977ff6decd3481714c66a333e75cb03095800ef0aa1e6cf6869285dcd7d023735badea49e6e3fe89421b167da1224a8c811a503fb23c28d9bb509343bc84d97630a95ae56431edc38f69b5feb882fa1965bce1e55eb6549a687840310179dd2c94b0c64a68babaa5dbd101d4897d6b7e05ecac5449606020297369f91865944fae5761821c3d0046259b688edce22a5a6f4b86f778").ToPositiveBigInteger(),
                // otherInfo
                new BitString("434156536964a1b2c3d4e53955b6e50d671741c61b9ffdd3764eed468bfc"),
                // expectedDerivedKeyingMaterial
                new BitString("4df034a67864b4a5ceb8eeedab5b12e5")
            },
            new object[]
            {
                "test3",
                ModeValues.SHA2,
                DigestSizes.d256,
                128,
                // z
                new BitString("96f8297160c8178d0ec38faf5bd57c6ee0e49c643f5384d2f6e15d0893752c72bdc1135eaefb8eac3e0dbf158aa42cec8729f2dd0caeff9a325978f2e2c38d1cb6124268fad626b7f4c9a092e283ea2f7beaa316af4d42cb97f8a0ff095ff53f831776b1ac35ae7e72bc4a0220c5c9ea1096b2a931412a552cabfd65a1531d42217456afe39d86273e520419678cef8bd44fa018c7f2f0339f1289f989115b0c3d2f8cec57588c5e159bbd1e1a8a1c42a7805d87db8dde08f151bb77feab13a482e71da62a9a9afbd6595a9def0d1309cde7f64ea69cc4d612ee7dc9696d24ff15085020f3001c069880786c492fdf77dd00512345a8a5f91ae609f3d618bf67").ToPositiveBigInteger(),
                // otherInfo
                new BitString("a1b2c3d4e5434156536964fa52970e44840dc70b96e5555fe35d1003b9db"),
                // expectedDerivedKeyingMaterial
                new BitString("d835f00a7b85d2a66fab2055bc7d3d3e")
            },
            new object[]
            {
                "from cavs",
                ModeValues.SHA2,
                DigestSizes.d224,
                256,
                new BitString("489d533a2cdf49d486329029bf633ddbf3e80ea0602de8ad42e3918a9cfae187cf5a408e290b11eee2625fdd0c2a25bc14655efcd100d31ca36570472e4e92296c5df310ff117b2322c4750080fc616ad3f423e614489cf455b0cddf9d7deb4df2ea495473c8dd2ed5b870a55e6eeebd4ff574d9d3da50287558d00c590420ccd1ef30ca294c56ad4c305c3080f36a473a30ff8b1fd939563c55ce6d85a9f535b3459c621e7a7ded1f3ddc51e8e54d292d07ff85505faf544b34122e9759630627f528a53c8bed8061cb32c3eb77140fd6e24f4d17ebb720f499e077001473fd7967c9f89049c7cefc21b27eaf642ef0c4cc5d0802380aa74761730d4481689c").ToPositiveBigInteger(),
                new BitString("a1b2c3d4e543415653696412532948b6fab173270f7d2de3622e61b2efdc"),
                new BitString("a0c5cea7f94deb7980fe78e9ef530a9c2f956df7f40aec2233398bc4f3a1c32f"),
            }
        };

        [Test]
        [TestCaseSource(nameof(_kdfTestCases))]
        public void ShouldKdfCorrectly(
            string label,
            ModeValues modeValue,
            DigestSizes digestSize,
            int kdfSize,
            BigInteger z,
            BitString otherInfo,
            BitString expectedDerivedKeyingMaterial
        )
        {
            var sha = _shaFactory.GetShaInstance(new HashFunction(modeValue, digestSize));
            _subject = new KdfSha(sha, true);

            var result = _subject.DeriveKey(
                new BitString(z),
                kdfSize,
                otherInfo,
                null
            );

            Assert.That(result.Success);
            Assert.AreEqual(expectedDerivedKeyingMaterial.ToHex(), result.DerivedKey.ToHex());
        }

        [Test]
        public void ShouldKdfWithEntropyBits()
        {
            var fixedInfo = new FixedInfoFactory(new FixedInfoStrategyFactory()).Get()
                .Get(new FixedInfoParameter()
                {
                    FixedInfoPattern = "entropyBits[96]",
                    EntropyBits = new BitString("1EE48EE4593B28D592D6ABFD"),
                    L = 256,
                    Encoding = FixedInfoEncoding.Concatenation,
                });
            var result = _kdfVisitor.Kdf(new KdfParameterOneStep()
            {
                L = 256,
                Z = new BitString("A641EE6887A5C679A6398B398DC14394B8BE1750A38616A80A5C3D3A59DDB302"),
                AuxFunction = KdaOneStepAuxFunction.SHA2_D256
            }, fixedInfo);

            var expectedDkm = new BitString("C56E3F1ADA02DB5511C7941839A68A4CAFD7B3F3BBFDBAAD430F49619FFEDF28");

            Assert.AreEqual(expectedDkm.ToHex(), result.DerivedKey.ToHex());
        }

        [Test]
        [TestCase(KdaOneStepAuxFunction.SHA1, 160, "BEEFBEEFBEEFBEEF", "BEEF", "587D84E6CBB525D22C5AA6D83BB76CB14C7D48C2")]
        public void CounterTestShouldPassOneStepCounterAndFailNoCounter(KdaOneStepAuxFunction auxFunction, int l, string zHex, string fixedInfoHex, string expectedHex)
        {
            var z = new BitString(zHex);
            var fixedInfo = new BitString(fixedInfoHex);
            var expected = new BitString(expectedHex);

            var counterKdf = _kdfVisitor.Kdf(new KdfParameterOneStep()
            {
                AuxFunction = auxFunction,
                L = l,
                Z = z,
            }, fixedInfo);

            var noCounterKdf = _kdfVisitor.Kdf(new KdfParameterOneStepNoCounter()
            {
                AuxFunction = auxFunction,
                L = l,
                Z = z,
            }, fixedInfo);

            Assert.AreEqual(expected.ToHex(), counterKdf.DerivedKey.ToHex(), nameof(counterKdf));
            Assert.AreNotEqual(expected.ToHex(), noCounterKdf.DerivedKey.ToHex(), nameof(noCounterKdf));
        }

        [Test]
        [TestCase(KdaOneStepAuxFunction.SHA1, 160, "BEEFBEEFBEEFBEEF", "BEEF", "4389DE49864FF97A9A575D62AA295A34F104C7CB")]
        public void NoCounterTestShouldPassOneStepNoCounterAndFailCounter(KdaOneStepAuxFunction auxFunction, int l, string zHex, string fixedInfoHex, string expectedHex)
        {
            var z = new BitString(zHex);
            var fixedInfo = new BitString(fixedInfoHex);
            var expected = new BitString(expectedHex);

            var noCounterKdf = _kdfVisitor.Kdf(new KdfParameterOneStepNoCounter()
            {
                AuxFunction = auxFunction,
                L = l,
                Z = z,
            }, fixedInfo);

            var counterKdf = _kdfVisitor.Kdf(new KdfParameterOneStep()
            {
                AuxFunction = auxFunction,
                L = l,
                Z = z,
            }, fixedInfo);

            Assert.AreEqual(expected.ToHex(), noCounterKdf.DerivedKey.ToHex(), nameof(noCounterKdf));
            Assert.AreNotEqual(expected.ToHex(), counterKdf.DerivedKey.ToHex(), nameof(counterKdf));
        }

        [Test]
        [TestCase(KdaOneStepAuxFunction.SHA1, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D224, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D256, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D384, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T224, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T256, 112)]

        [TestCase(KdaOneStepAuxFunction.SHA3_D224, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D256, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D384, 112)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D512, 112)]

        [TestCase(KdaOneStepAuxFunction.SHA1, 160)]

        [TestCase(KdaOneStepAuxFunction.SHA2_D224, 224)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D256, 256)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D384, 384)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512, 512)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T224, 224)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T256, 256)]

        [TestCase(KdaOneStepAuxFunction.SHA3_D224, 224)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D256, 256)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D384, 384)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D512, 512)]
        public void NoCounterTestShouldSucceedIfDkmLengthLteOutputLengthOfHash(KdaOneStepAuxFunction auxFunction, int l)
        {
            var z = new BitString("BEEFBEEF");
            var fixedInfo = new BitString("BEEF");

            var noCounterKdf = _kdfVisitor.Kdf(new KdfParameterOneStepNoCounter()
            {
                AuxFunction = auxFunction,
                L = l,
                Z = z,
            }, fixedInfo);

            Assert.IsTrue(noCounterKdf.Success);
        }

        [Test]
        [TestCase(KdaOneStepAuxFunction.SHA2_D224, 224 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D256, 256 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D384, 384 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512, 512 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T224, 224 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA2_D512_T256, 256 + 8)]

        [TestCase(KdaOneStepAuxFunction.SHA3_D224, 224 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D256, 256 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D384, 384 + 8)]
        [TestCase(KdaOneStepAuxFunction.SHA3_D512, 512 + 8)]
        public void NoCounterTestShouldFailIfDkmLengthGtOutputLengthOfHash(
            KdaOneStepAuxFunction auxFunction, int l)
        {
            var z = new BitString("BEEFBEEF");
            var fixedInfo = new BitString("BEEF");

            Assert.Throws<ArgumentException>(() =>
            {
                _kdfVisitor.Kdf(new KdfParameterOneStepNoCounter()
                {
                    AuxFunction = auxFunction,
                    L = l,
                    Z = z,
                }, fixedInfo);
            });

        }
    }
}
