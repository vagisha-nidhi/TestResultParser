﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Agent.Plugins.Log.TestResultParser.Contracts;

namespace Agent.Plugins.Log.TestResultParser.Parser
{
    public class JestExpectingTestRunSummary : JestParserStateBase
    {
        public override IEnumerable<RegexActionPair> RegexsToMatch { get; }

        /// <inheritdoc />
        public JestExpectingTestRunSummary(ParserResetAndAttemptPublish parserResetAndAttemptPublish, ITraceLogger logger, ITelemetryDataCollector telemetryDataCollector)
            : base(parserResetAndAttemptPublish, logger, telemetryDataCollector)
        {
            RegexsToMatch = new List<RegexActionPair>
            {
                new RegexActionPair(JestRegexs.TestsSummaryMatcher, TestsSummaryMatched),
                new RegexActionPair(JestRegexs.TestRunTimeMatcher, TestRunTimeMatched),
                new RegexActionPair(JestRegexs.TestRunStart, TestRunStartMatched)
            };
        }

        private Enum TestsSummaryMatched(Match match, TestResultParserStateContext stateContext)
        {
            var jestStateContext = stateContext as JestParserStateContext;

            jestStateContext.LinesWithinWhichMatchIsExpected = 2;
            jestStateContext.NextExpectedMatch = "test run time";

            // Handling parse errors is unnecessary
            int.TryParse(match.Groups[RegexCaptureGroups.PassedTests].Value, out int totalPassed);
            int.TryParse(match.Groups[RegexCaptureGroups.FailedTests].Value, out int totalFailed);
            int.TryParse(match.Groups[RegexCaptureGroups.SkippedTests].Value, out int totalSkipped);

            jestStateContext.TestRun.TestRunSummary.TotalPassed = totalPassed;
            jestStateContext.TestRun.TestRunSummary.TotalFailed = totalFailed;
            jestStateContext.TestRun.TestRunSummary.TotalSkipped = totalSkipped;

            return JestParserStates.ExpectingTestRunSummary;
        }

        private Enum TestRunTimeMatched(Match match, TestResultParserStateContext stateContext)
        {
            var jestStateContext = stateContext as JestParserStateContext;

            // Extract the test run time
            // Handling parse errors is unnecessary
            var timeTaken = double.Parse(match.Groups[RegexCaptureGroups.TestRunTime].Value);

            // Store time taken based on the unit used
            switch (match.Groups[RegexCaptureGroups.TestRunTimeUnit].Value)
            {
                case "ms":
                    jestStateContext.TestRun.TestRunSummary.TotalExecutionTime = TimeSpan.FromMilliseconds(timeTaken);
                    break;

                case "s":
                    jestStateContext.TestRun.TestRunSummary.TotalExecutionTime = TimeSpan.FromMilliseconds(timeTaken * 1000);
                    break;

                case "m":
                    jestStateContext.TestRun.TestRunSummary.TotalExecutionTime = TimeSpan.FromMilliseconds(timeTaken * 60 * 1000);
                    break;

                case "h":
                    jestStateContext.TestRun.TestRunSummary.TotalExecutionTime = TimeSpan.FromMilliseconds(timeTaken * 60 * 60 * 1000);
                    break;
            }

            this.attemptPublishAndResetParser();

            return JestParserStates.ExpectingTestRunStart;
        }

        private Enum TestRunStartMatched(Match match, TestResultParserStateContext stateContext)
        {
            var jestStateContext = stateContext as JestParserStateContext;

            // Do we want to use PASS/FAIL information here?

            return JestParserStates.ExpectingTestResults;
        }
    }
}
