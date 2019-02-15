﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    internal static class LdapSinks
    {
        /// <summary>
        /// <see cref="SinkInfo"/>s for tainted data LDAP injection sinks.
        /// </summary>
        public static ImmutableHashSet<SinkInfo> SinkInfos { get; }

        static LdapSinks()
        {
            var builder = PooledHashSet<SinkInfo>.GetInstance();

            builder.AddSinkInfo(
                WellKnownTypes.SystemDirectoryServicesActiveDirectoryADSearcher,
                SinkKind.Ldap,
                isInterface: false,
                isAnyStringParameterInConstructorASink: true,
                sinkProperties: new[] {
                    "Filter",
                },
                sinkMethodParameters: null);
            builder.AddSinkInfo(
                WellKnownTypes.SystemDirectoryServicesDirectorySearcher,
                SinkKind.Ldap,
                isInterface: false,
                isAnyStringParameterInConstructorASink: true,
                sinkProperties: new[] {
                    "Filter",
                },
                sinkMethodParameters: null);
            builder.AddSinkInfo(
                WellKnownTypes.SystemDirectoryDirectoryEntry,
                SinkKind.Ldap,
                isInterface: false,
                isAnyStringParameterInConstructorASink: false,
                sinkProperties: null,
                sinkMethodParameters: new[] {
                    (".ctor", new[] { "path", "adsObject" } ),
                });

            SinkInfos = builder.ToImmutableAndFree();
        }
    }
}
