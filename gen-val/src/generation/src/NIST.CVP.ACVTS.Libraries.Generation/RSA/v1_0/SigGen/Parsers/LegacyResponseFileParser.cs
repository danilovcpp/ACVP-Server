﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NIST.CVP.ACVTS.Libraries.Generation.Core;
using NIST.CVP.ACVTS.Libraries.Generation.Core.Parsers;
using NIST.CVP.ACVTS.Libraries.Math;

namespace NIST.CVP.ACVTS.Libraries.Generation.RSA.v1_0.SigGen.Parsers
{
    public class LegacyResponseFileParser : ILegacyResponseFileParser<TestVectorSet, TestGroup, TestCase>
    {
        public ParseResponse<TestVectorSet> Parse(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new ParseResponse<TestVectorSet>("There was no path supplied.");
            }

            if (!File.Exists(path))
            {
                return new ParseResponse<TestVectorSet>($"Could not find file: {path}");
            }

            var lines = new List<string>();
            try
            {
                lines = File.ReadAllLines(path).ToList();
            }
            catch (Exception ex)
            {
                return new ParseResponse<TestVectorSet>(ex.Message);
            }

            var groups = new List<TestGroup>();
            TestGroup curGroup = null;
            TestCase curTestCase = null;
            var inCases = false;
            var caseId = 1;
            var saltVal = "";

            foreach (var line in lines)
            {
                var workingLine = line.ToLower().Trim();
                if (string.IsNullOrEmpty(workingLine))
                {
                    continue;
                }

                if (workingLine.StartsWith("#"))
                {
                    continue;
                }

                if (workingLine.StartsWith("["))
                {
                    if (curGroup == null || inCases)
                    {
                        inCases = false;
                        curGroup = new TestGroup();
                        groups.Add(curGroup);
                    }

                    workingLine = workingLine.Replace("[", "").Replace("]", "");
                    var propParts = workingLine.Split("=".ToCharArray());
                    if (!curGroup.SetString(propParts[0].Trim(), propParts[1].Trim()))
                    {
                        // Salt
                        saltVal = propParts[1].Trim();
                    }

                    continue;
                }

                if (workingLine.StartsWith("msg = "))
                {
                    curTestCase = new TestCase { TestCaseId = caseId };
                    curTestCase.Salt = new BitString(saltVal, curGroup.SaltLen * 8);
                    inCases = true;

                    curGroup.Tests.Add(curTestCase);
                    caseId++;
                }

                var parts = workingLine.Split("=".ToCharArray());
                curTestCase.SetString(parts[0].Trim(), parts[1].Trim());
            }

            return new ParseResponse<TestVectorSet>(new TestVectorSet { Algorithm = "RSA", Mode = "SigGen", TestGroups = groups });
        }
    }
}
