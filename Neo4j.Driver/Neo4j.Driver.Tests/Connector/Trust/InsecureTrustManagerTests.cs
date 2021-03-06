﻿// Copyright (c) 2002-2018 "Neo4j,"
// Neo4j Sweden AB [http://neo4j.com]
// 
// This file is part of Neo4j.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Security.Authentication;
using FluentAssertions;
using Neo4j.Driver.Internal.Connector.Trust;
using Neo4j.Driver.Tests.TestUtil;
using Xunit;

namespace Neo4j.Driver.Tests.Connector.Trust
{
    public class InsecureTrustManagerTests
    {

        [Fact]
        public void ShouldTrust()
        {
            var pkcs12 = X509TestUtils.CreateCert("localhost", DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1),
                null, null, null);
            var cert = X509TestUtils.ToDotnetCertificate(pkcs12);
            var trustManager = new InsecureTrustManager(true);

            new TrustManagerHandshaker(new Uri("bolt://localhost"), cert, trustManager).Perform();
        }

        [Fact]
        public void ShouldNotTrustIfHostnameDiffers()
        {
            var pkcs12 = X509TestUtils.CreateCert("localhost", DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1),
                null, null, null);
            var cert = X509TestUtils.ToDotnetCertificate(pkcs12);
            var trustManager = new InsecureTrustManager(true);

            var ex = Record.Exception(() =>
                new TrustManagerHandshaker(new Uri("bolt://localhost2"), cert, trustManager).Perform());

            ex.Should().NotBeNull().And.BeOfType<AuthenticationException>();
        }

        [Fact]
        public void ShouldTrustIfHostnameDiffers()
        {
            var pkcs12 = X509TestUtils.CreateCert("localhost", DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1),
                null, null, null);
            var cert = X509TestUtils.ToDotnetCertificate(pkcs12);
            var trustManager = new InsecureTrustManager(false);

            new TrustManagerHandshaker(new Uri("bolt://localhost2"), cert, trustManager).Perform();
        }
    }
}