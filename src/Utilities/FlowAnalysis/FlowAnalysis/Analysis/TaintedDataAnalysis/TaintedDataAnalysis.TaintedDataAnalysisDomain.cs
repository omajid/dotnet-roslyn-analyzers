﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

    internal partial class TaintedDataAnalysis
    {
        private sealed class TaintedDataAnalysisDomain(MapAbstractDomain<AnalysisEntity, TaintedDataAbstractValue> coreDataAnalysisDomain) : PredicatedAnalysisDataDomain<TaintedDataAnalysisData, TaintedDataAbstractValue>(coreDataAnalysisDomain)
        {
        }
    }
}