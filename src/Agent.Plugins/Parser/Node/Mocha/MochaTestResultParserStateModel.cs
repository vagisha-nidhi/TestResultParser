﻿namespace Agent.Plugins.TestResultParser.Parser.Node.Mocha
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum MochaTestResultParserStateModel
    {
        LookingForTestRunStartIndicator,
        ParsingTestResults,
        SummaryEncountered,
        ParsingStackTracesPostSummary
    }
}
