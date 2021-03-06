﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Agent.Plugins.Log.TestResultParser.Contracts;

namespace Agent.Plugins.Log.TestResultParser.Parser
{
    /// <summary>
    /// Base class for a jest test result parser state
    /// Has common methods that each state will need to use
    /// </summary>
    public class JestParserStateBase : ITestResultParserState
    {
        protected ITraceLogger logger;
        protected ITelemetryDataCollector telemetryDataCollector;
        protected ParserResetAndAttemptPublish attemptPublishAndResetParser;

        /// <summary>
        /// List of Regexs and their corresponding post successful match actions
        /// </summary>
        public virtual IEnumerable<RegexActionPair> RegexsToMatch => throw new NotImplementedException();

        /// <summary>
        /// Constructor for a jest parser state
        /// </summary>
        /// <param name="parserResetAndAttempPublish">Delegate sent by the parser to reset the parser and attempt publication of test results</param>
        /// <param name="logger"></param>
        /// <param name="telemetryDataCollector"></param>
        protected JestParserStateBase(ParserResetAndAttemptPublish parserResetAndAttempPublish, ITraceLogger logger, ITelemetryDataCollector telemetryDataCollector)
        {
            this.logger = logger;
            this.telemetryDataCollector = telemetryDataCollector;
            this.attemptPublishAndResetParser = parserResetAndAttempPublish;
        }

        /// <summary>
        /// Returns a test result with the outcome set and name extracted from the match
        /// </summary>
        /// <param name="testOutcome">Outcome of the test</param>
        /// <param name="match">Match object for the test case result</param>
        /// <returns></returns>
        protected TestResult PrepareTestResult(TestOutcome testOutcome, Match match)
        {
            return new TestResult
            {
                Outcome = testOutcome,
                Name = match.Groups[RegexCaptureGroups.TestCaseName].Value
            };
        }
    }
}
