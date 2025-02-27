// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Tests.Storage
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using Azure.IIoT.OpcUa.Publisher.Storage;
    using Furly.Extensions.Logging;
    using Furly.Extensions.Serializers;
    using Furly.Extensions.Serializers.Newtonsoft;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// The referenced schema file across these test is a linked asset in the
    /// project file set to copy to the output build directory so that it can
    /// be easily referenced here.
    /// </summary>
    public class PublishedNodesJobConverterTests
    {
        [Fact]
        public void PnPlcEmptyTest()
        {
            const string pn = @"
[
]
";
            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var writerGroups = converter.Read(pn);

            // No writerGroups
            Assert.Empty(writerGroups);
        }

        [Fact]
        public void PnPlcPubSubDataSetWriterIdTest()
        {
            const string pn = @"
[
    {
        ""DataSetWriterId"": ""testid"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""HeartbeatInterval"": 2
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());
            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testid", writerGroups
                .Single().DataSetWriters
                .Single().DataSetWriterName);
        }

        [Fact]
        public void PnPlcPubSubDataSetWriterIdIsNullTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""HeartbeatInterval"": 2
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());
            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("<<UnknownDataSet>>_($b6589fc6ab0dc82cf12099d1c2d40ab994e8410c)", writerGroups
                .Single().DataSetWriters
                .Single().DataSetWriterName);
        }

        [Fact]
        public void PnPlcPubSubDataSetWriterGroupTest()
        {
            const string pn = @"
[
    {
        ""DataSetWriterGroup"": ""testgroup"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""HeartbeatInterval"": 2
            }
        ]
    }
]
";
            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testgroup", writerGroups
                .Single().WriterGroupId);
            Assert.Equal("testgroup", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Group);
        }

        [Fact]
        public void PnPlcPubSubDataSetFieldId1Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testfieldid1""
            }
        ]
    }
]
";
            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testfieldid1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().Id);
        }

        [Fact]
        public void PnPlcPubSubDataSetFieldId2Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testfieldid1""
            },
            {
                ""Id"": ""i=2259"",
                ""DataSetFieldId"": ""testfieldid2""
            }
        ]
    }
]
";
            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Count);
            Assert.Equal("testfieldid1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData[0].Id);
            Assert.Equal("testfieldid2", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().Id);
        }

        [Fact]
        public void PnPlcPubSubDataSetFieldIdDuplicateTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testfieldid""
            },
            {
                ""Id"": ""i=2259"",
                ""DataSetFieldId"": ""testfieldid""
            }
        ]
    }
]
";
            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Count);
            Assert.Equal("testfieldid", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData[0].Id);
            Assert.Equal("testfieldid", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().Id);
        }

        [Fact]
        public void PnPlcPubSubDisplayNameDuplicateTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname""
            },
            {
                ""Id"": ""i=2259"",
                ""DisplayName"": ""testdisplayname""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Count);
            Assert.Equal("testdisplayname", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData[0].PublishedVariableDisplayName);
            Assert.Equal("testdisplayname", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcPubSubFullDuplicateTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname"",
                ""DataSetFieldId"": ""testfieldid""
            },
            {
                ""Id"": ""i=2259"",
                ""DisplayName"": ""testdisplayname"",
                ""DataSetFieldId"": ""testfieldid""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Count);
            Assert.Equal("testfieldid", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData[0].Id);
            Assert.Equal("testfieldid", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().Id);
        }

        [Fact]
        public void PnPlcPubSubFullTest()
        {
            const string pn = @"
[
    {
        ""DataSetWriterGroup"": ""testgroup"",
        ""DataSetWriterId"": ""testwriterid"",
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testfieldid1"",
                ""OpcPublishingInterval"": 2000
            },
            {
                ""Id"": ""i=2259"",
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Single(writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Equal("testfieldid1", writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData[0].Id);
            Assert.Equal("i=2258", writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData[0].PublishedVariableNodeId);
            Assert.Null(writerGroups
                .Single().DataSetWriters
                .Last().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().Id);
            Assert.Equal("i=2259", writerGroups
                .Single().DataSetWriters
                .Last().DataSet.DataSetSource.PublishedVariables.PublishedData.Last().PublishedVariableNodeId);
            Assert.Equal("testgroup", writerGroups
                .Single().WriterGroupId);
            Assert.Equal("testgroup", writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.Connection.Group);
            Assert.Equal("testwriterid_($a4ac914c09d7c097fe1f4f96b897e625b6922069)", writerGroups
                .Single().DataSetWriters[0].DataSetWriterName);
            Assert.Equal(2000, writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters
                .Last().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingInterval1Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingInterval2Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""OpcPublishingInterval"": 2000
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingInterval3Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258""
            },
            {
                ""Id"": ""i=2259""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingInterval4Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""OpcPublishingInterval"": 2000
            },
            {
                ""Id"": ""i=2259""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2000, writerGroups
                .First().DataSetWriters[0].DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingIntervalTimespan1Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingIntervalTimespan"": ""00:00:01"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingIntervalTimespan2Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingIntervalTimespan"": ""00:00:01"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""OpcPublishingIntervalTimespan"": ""00:00:02""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(2000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingIntervalTimespan3Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingIntervalTimespan"": ""00:00:01"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258""
            },
            {
                ""Id"": ""i=2259""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDataSetPublishingIntervalTimespan4Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingIntervalTimespan"": ""00:00:01"",
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
            },
            {
                ""Id"": ""i=2259"",
                ""OpcPublishingInterval"": 3000
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal(1000, writerGroups
                .Single().DataSetWriters[0].DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPubSubDisplayName1Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname1""
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testdisplayname1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcPubSubDisplayName2Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Null(writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().Id);
        }

        [Fact]
        public void PnPlcPubSubDisplayName3Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname1"",
                ""DataSetFieldId"": ""testdatasetfieldid1"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testdatasetfieldid1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().Id);
        }

        [Fact]
        public void PnPlcPubSubDisplayName4Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testdatasetfieldid1"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testdatasetfieldid1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().Id);
        }

        [Fact]
        public void PnPlcPubSubPublishedNodeDisplayName1Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname1""
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testdisplayname1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcPubSubPublishedNodeDisplayName2Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Null(writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcPubSubPublishedNodeDisplayName3Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""testdisplayname1"",
                ""DataSetFieldId"": ""testdatasetfieldid1"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Equal("testdisplayname1", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcPubSubPublishedNodeDisplayName4Test()
        {
            const string pn = @"
[
    {
        ""DataSetPublishingInterval"": 1000,
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DataSetFieldId"": ""testdatasetfieldid1"",
            },
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Null(writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.PublishedVariables.PublishedData.Single().PublishedVariableDisplayName);
        }

        [Fact]
        public void PnPlcHeartbeatInterval2Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""HeartbeatInterval"": 2
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(writerGroups
                .Single().DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
            Assert.Equal(2, j
                .DataSetWriters.Single()
                .DataSet.DataSetSource.PublishedVariables.PublishedData.Single()
                .HeartbeatInterval.Value.TotalSeconds);
        }

        [Fact]
        public void PnPlcHeartbeatIntervalTimespanTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""HeartbeatIntervalTimespan"": ""00:00:01.500""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(writerGroups
                .Single().DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
            Assert.Equal(1500, j
                .DataSetWriters.Single()
                .DataSet.DataSetSource.PublishedVariables.PublishedData.Single()
                .HeartbeatInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcHeartbeatSkipSingleTrueTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""SkipSingle"": true
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(writerGroups
                .Single().DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcHeartbeatSkipSingleFalseTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""SkipSingle"": false
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            Assert.Single(writerGroups);
            Assert.Single(writerGroups
                .Single().DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcPublishingInterval2000Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""OpcPublishingInterval"": 2000
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(j.DataSetWriters);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
            Assert.Equal(2000, j.DataSetWriters.Single()
                .DataSet.DataSetSource.SubscriptionSettings.PublishingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcPublishingIntervalCliTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(j.DataSetWriters);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
            Assert.Null(j.DataSetWriters.Single()
                .DataSet.DataSetSource.SubscriptionSettings.PublishingInterval);
        }

        [Fact]
        public void PnPlcSamplingInterval2000Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""OpcSamplingInterval"": 2000
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(j.DataSetWriters);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
            Assert.Equal(2000, j
             .DataSetWriters.Single()
             .DataSet.DataSetSource.PublishedVariables.PublishedData.Single()
             .SamplingInterval.Value.TotalMilliseconds);
        }

        [Fact]
        public void PnPlcExpandedNodeIdTest()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""ExpandedNodeId"": ""nsu=http://opcfoundation.org/UA/;i=2258""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(j.DataSetWriters);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal("opc.tcp://localhost:50000", writerGroups
                .Single().DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcExpandedNodeId2Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""ExpandedNodeId"": ""nsu=http://opcfoundation.org/UA/;i=2258""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2262""
            },
            {
                ""Id"": ""ns=2;s=AlternatingBoolean""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);
            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(j.DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", j.DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcExpandedNodeId3Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258""
            },
            {
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(j.DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", j.DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcExpandedNodeId4Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
                ""Identifier"": ""i=2258""
        }
        },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [

            {
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2262""
            },
            {
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Single(j.DataSetWriters);
            Assert.Equal("opc.tcp://localhost:50000", j.DataSetWriters
                .Single().DataSet.DataSetSource.Connection.Endpoint.Url);
        }

        [Fact]
        public void PnPlcMultiJob1Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost1:50000"",
        ""NodeId"": {
                ""Identifier"": ""i=2258""
        }
        },
    {
        ""EndpointUrl"": ""opc.tcp://localhost2:50000"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost3:50000"",
        ""OpcNodes"": [

            {
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost4:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2262""
            },
            {
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(4, j.DataSetWriters.Count);
        }

        [Fact]
        public void PnPlcMultiJob2Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50001"",
        ""NodeId"": {
                ""Identifier"": ""i=2258"",
        }
        },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50002"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50003"",
        ""OpcNodes"": [
            {
                ""OpcPublishingInterval"": 1000,
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50004"",
        ""OpcNodes"": [
            {
                ""OpcPublishingInterval"": 1000,
                ""Id"": ""i=2262""
            },
            {
                ""OpcPublishingInterval"": 1000,
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData"",
                ""OpcPublishingInterval"": 1000
            }
        ]
    }
]
";
            var endpointUrls = new string[] {
                "opc.tcp://localhost:50001",
                "opc.tcp://localhost:50002",
                "opc.tcp://localhost:50003",
                "opc.tcp://localhost:50004"
            };

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(4, j.DataSetWriters.Count);
            Assert.True(endpointUrls.ToHashSet().SetEqualsSafe(
                j.DataSetWriters.Select(w => w.DataSet.DataSetSource.Connection.Endpoint.Url)));
        }

        [Fact]
        public void PnPlcMultiJob3Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
                ""Identifier"": ""i=2258"",
        }
        },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50001"",
        ""OpcNodes"": [
            {
                ""OpcPublishingInterval"": 1000,
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50001"",
        ""OpcNodes"": [
            {
                ""OpcPublishingInterval"": 1000,
                ""Id"": ""i=2262""
            },
            {
                ""OpcPublishingInterval"": 1000,
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData"",
                ""OpcPublishingInterval"": 1000
            }
        ]
    }
]
";
            var endpointUrls = new string[] {
                "opc.tcp://localhost:50000",
                "opc.tcp://localhost:50001"
            };

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(2, j.DataSetWriters.Count);
            Assert.True(endpointUrls.ToHashSet().SetEqualsSafe(
                j.DataSetWriters.Select(w => w.DataSet.DataSetSource.Connection.Endpoint.Url)));
        }

        [Fact]
        public void PnPlcMultiJob4Test()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
                ""Identifier"": ""i=2258"",
        }
        },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50001"",
        ""OpcNodes"": [
            {
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean""
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50001"",
        ""OpcNodes"": [
            {
                ""OpcPublishingInterval"": 2000,
                ""Id"": ""i=2262""
            },
            {
                ""OpcPublishingInterval"": 2000,
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData"",
            }
        ]
    }
]
";
            var endpointUrls = new string[] {
                "opc.tcp://localhost:50000",
                "opc.tcp://localhost:50001"
            };

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions());

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(3, j.DataSetWriters.Count);
            Assert.True(endpointUrls.ToHashSet().SetEqualsSafe(
                j.DataSetWriters.Select(w => w.DataSet.DataSetSource.Connection.Endpoint.Url)));
        }

        [Theory]
        [InlineData("Publisher/publishednodes_with_duplicates.json")]
        public async Task PnWithDuplicatesTest(string publishedNodesJsonFile)
        {
            var pn = await File.ReadAllTextAsync(publishedNodesJsonFile).ConfigureAwait(false);

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(2, j.DataSetWriters.Count);
            Assert.All(j.DataSetWriters, dataSetWriter => Assert.Equal("opc.tcp://10.0.0.1:59412",
                dataSetWriter.DataSet.DataSetSource.Connection.Endpoint.Url));
            Assert.Single(j.DataSetWriters, dataSetWriter => TimeSpan.FromMinutes(15) ==
                dataSetWriter.DataSet.DataSetSource.SubscriptionSettings.PublishingInterval);
            Assert.Single(j.DataSetWriters, dataSetWriter => TimeSpan.FromMinutes(1) ==
                dataSetWriter.DataSet.DataSetSource.SubscriptionSettings.PublishingInterval);
            Assert.Single(j.DataSetWriters, dataSetWriter =>
                dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData.Any(
                    p => TimeSpan.FromMinutes(15) == p.SamplingInterval));
            Assert.Equal(3, j.DataSetWriters.Sum(dataSetWriter =>
                dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData.Count));
        }

        [Fact]
        public void PnPlcMultiJobBatching1Test()
        {
            var pn = new StringBuilder(@"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            ");

            for (var i = 1; i < 10000; i++)
            {
                pn
                    .Append("{ \"Id\": \"i=")
                    .Append(i)
                    .Append("\" },");
            }

            pn.Append(@"
            { ""Id"": ""i=10000"" }
        ]
    }
]
");

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn.ToString());
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            // No writerGroups
            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(10, j.DataSetWriters.Count);
            Assert.All(j.DataSetWriters, dataSetWriter => Assert.Equal("opc.tcp://localhost:50000",
                dataSetWriter.DataSet.DataSetSource.Connection.Endpoint.Url));
            Assert.All(j.DataSetWriters, dataSetWriter => Assert.Null(
                dataSetWriter.DataSet.DataSetSource.SubscriptionSettings.PublishingInterval));
            Assert.All(j.DataSetWriters, dataSetWriter => Assert.All(
                dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData,
                    p => Assert.Null(p.SamplingInterval)));
            Assert.All(j.DataSetWriters, dataSetWriter =>
                Assert.Equal(1000,
                    dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData.Count));
        }

        [Fact]
        public void PnPlcMultiJobBatching2Test()
        {
            var pn = new StringBuilder(@"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            ");

            for (var i = 1; i < 10000; i++)
            {
                pn
                    .Append("{ \"Id\": \"i=")
                    .Append(i)
                    .Append('\"')
                    .Append(i % 2 == 1 ? ",\"OpcPublishingInterval\": 2000" : null)
                    .Append("},");
            }

            pn.Append(@"
            { ""Id"": ""i=10000"" }
        ]
    }
]
");

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn.ToString());
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            // No writerGroups
            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);

            Assert.True((j.MessageSettings.NetworkMessageContentMask & NetworkMessageContentMask.MonitoredItemMessage) != 0);

            Assert.Equal(10, j.DataSetWriters.Count);
            Assert.All(j.DataSetWriters, dataSetWriter => Assert.Equal("opc.tcp://localhost:50000",
                dataSetWriter.DataSet.DataSetSource.Connection.Endpoint.Url));
            Assert.Equal(
                new TimeSpan?[] {
                    TimeSpan.FromMilliseconds(2000),
                    TimeSpan.FromMilliseconds(2000),
                    TimeSpan.FromMilliseconds(2000),
                    TimeSpan.FromMilliseconds(2000),
                    TimeSpan.FromMilliseconds(2000),
                    null,
                    null,
                    null,
                    null,
                    null
                }, j.DataSetWriters.ConvertAll(dataSetWriter =>
                    dataSetWriter.DataSet.DataSetSource.SubscriptionSettings?.PublishingInterval));

            Assert.All(j.DataSetWriters, dataSetWriter => Assert.All(
                dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData,
                    p => Assert.Null(p.SamplingInterval)));
            Assert.All(j.DataSetWriters, dataSetWriter =>
                Assert.Equal(1000,
                    dataSetWriter.DataSet.DataSetSource.PublishedVariables.PublishedData.Count));
        }

        [Fact]
        public void PnPlcJobWithAllEventPropertiesTest()
        {
            var pn = new StringBuilder(@"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2258"",
                ""DisplayName"": ""TestingDisplayName"",
                ""EventFilter"": {
                    ""SelectClauses"": [
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""EventId""
                            ],
                            ""AttributeId"": ""BrowseName"",
                            ""IndexRange"": ""5:20""
                        }
                    ],
                    ""WhereClause"": {
                        ""Elements"": [
                            {
                                ""FilterOperator"": ""OfType"",
                                ""FilterOperands"": [
                                    {
                                        ""NodeId"": ""i=2041"",
                                        ""BrowsePath"": [
                                            ""EventId""
                                        ],
                                        ""AttributeId"": ""BrowseName"",
                                        ""Value"": ""ns=2;i=235"",
                                        ""IndexRange"": ""5:20"",
                                        ""Index"": 10,
                                        ""Alias"": ""Test"",
                                    }
                                ]
                            }
                        ]
                    }
                }
            }
        ]
    }
]
");

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn.ToString());
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            // Check writerGroups
            Assert.Single(writerGroups);
            Assert.Single(writerGroups[0].DataSetWriters);
            Assert.Single(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData);
            Assert.NotNull(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedVariables);
            Assert.NotNull(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents);
            Assert.Empty(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Single(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData);

            // Check model
            var model = writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData[0];
            Assert.Equal("TestingDisplayName", model.PublishedEventName);
            Assert.Equal("i=2258", model.EventNotifier);

            // Check select clauses
            Assert.Single(model.SelectClauses);
            Assert.Equal("i=2041", model.SelectClauses[0].TypeDefinitionId);
            Assert.Single(model.SelectClauses[0].BrowsePath);
            Assert.Equal("EventId", model.SelectClauses[0].BrowsePath[0]);
            Assert.Equal(NodeAttribute.BrowseName, model.SelectClauses[0].AttributeId.Value);
            Assert.Equal("5:20", model.SelectClauses[0].IndexRange);
            Assert.NotNull(model.WhereClause);
            Assert.Single(model.WhereClause.Elements);
            Assert.Equal(FilterOperatorType.OfType, model.WhereClause.Elements[0].FilterOperator);
            Assert.Single(model.WhereClause.Elements[0].FilterOperands);
            Assert.Equal("i=2041", model.WhereClause.Elements[0].FilterOperands[0].NodeId);
            Assert.Equal("ns=2;i=235", model.WhereClause.Elements[0].FilterOperands[0].Value);

            // Check where clause
            Assert.Single(model.WhereClause.Elements[0].FilterOperands[0].BrowsePath);
            Assert.Equal("EventId", model.WhereClause.Elements[0].FilterOperands[0].BrowsePath[0]);
            Assert.Equal(NodeAttribute.BrowseName, model.WhereClause.Elements[0].FilterOperands[0].AttributeId.Value);
            Assert.Equal("5:20", model.WhereClause.Elements[0].FilterOperands[0].IndexRange);
            Assert.NotNull(model.WhereClause.Elements[0].FilterOperands[0].Index);
            Assert.Equal((uint)10, model.WhereClause.Elements[0].FilterOperands[0].Index.Value);
            Assert.Equal("Test", model.WhereClause.Elements[0].FilterOperands[0].Alias);
        }

        [Fact]
        public void PnPlcMultiJob1TestWithDataItemsAndEvents()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://localhost1:50000"",
        ""NodeId"": {
            ""Identifier"": ""i=2258""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost2:50000"",
        ""NodeId"": {
            ""Identifier"": ""ns=0;i=2261""
        }
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost3:50000"",
        ""OpcNodes"": [
            {
                ""ExpandedNodeId"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean"",
                ""DisplayName"": ""AlternatingBoolean""
            },
            {
                ""Id"": ""i=2253"",
                ""OpcPublishingInterval"": 5000,
                ""EventFilter"": {
                    ""SelectClauses"": [
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""EventId""
                            ]
                        }
                    ],
                    ""WhereClause"": {
                        ""Elements"": [
                            {
                                ""FilterOperator"": ""OfType"",
                                ""FilterOperands"": [
                                    {
                                        ""Value"": ""ns=2;i=235""
                                    }
                                ]
                            }
                        ]
                    }
                }
            }
        ]
    },
    {
        ""EndpointUrl"": ""opc.tcp://localhost4:50000"",
        ""OpcNodes"": [
            {
                ""Id"": ""i=2262""
            },
            {
                ""Id"": ""ns=2;s=DipData""
            },
            {
                ""Id"": ""nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData""
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            Assert.NotEmpty(writerGroups);
            var writers = Assert.Single(writerGroups).DataSetWriters;

            Assert.Equal(5, writers.Count);
            Assert.Single(writers[0].DataSet.DataSetSource.PublishedVariables.PublishedData);
            var dataItemModel = writers[0].DataSet.DataSetSource.PublishedVariables.PublishedData[0];
            Assert.Equal("i=2258", dataItemModel.PublishedVariableNodeId);
            Assert.Empty(writers[0].DataSet.DataSetSource.PublishedEvents.PublishedData);

            Assert.Single(writers[1].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Empty(writers[1].DataSet.DataSetSource.PublishedEvents.PublishedData);
            dataItemModel = writers[1].DataSet.DataSetSource.PublishedVariables.PublishedData[0];
            Assert.Equal("ns=0;i=2261", dataItemModel.PublishedVariableNodeId);

            Assert.Single(writers[2].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Empty(writers[2].DataSet.DataSetSource.PublishedEvents.PublishedData);
            dataItemModel = writers[2].DataSet.DataSetSource.PublishedVariables.PublishedData[0];
            Assert.Equal("AlternatingBoolean", dataItemModel.PublishedVariableDisplayName);
            Assert.Equal("nsu=http://microsoft.com/Opc/OpcPlc/;s=AlternatingBoolean", dataItemModel.PublishedVariableNodeId);

            Assert.Empty(writers[3].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.NotEmpty(writers[3].DataSet.DataSetSource.PublishedEvents.PublishedData);
            var eventModel = writers[3].DataSet.DataSetSource.PublishedEvents.PublishedData[0];
            Assert.Equal("i=2253", eventModel.EventNotifier);
            Assert.Single(eventModel.SelectClauses);
            Assert.Equal("i=2041", eventModel.SelectClauses[0].TypeDefinitionId);
            Assert.Single(eventModel.SelectClauses[0].BrowsePath);
            Assert.Equal("EventId", eventModel.SelectClauses[0].BrowsePath[0]);
            Assert.NotNull(eventModel.WhereClause);
            Assert.Single(eventModel.WhereClause.Elements);
            Assert.Equal(FilterOperatorType.OfType, eventModel.WhereClause.Elements[0].FilterOperator);
            Assert.Single(eventModel.WhereClause.Elements[0].FilterOperands);
            Assert.Equal("ns=2;i=235", eventModel.WhereClause.Elements[0].FilterOperands[0].Value);

            Assert.NotEmpty(writers[4].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Empty(writers[4].DataSet.DataSetSource.PublishedEvents.PublishedData);
            dataItemModel = writers[4].DataSet.DataSetSource.PublishedVariables.PublishedData[0];
            Assert.Equal("i=2262", dataItemModel.PublishedVariableNodeId);
            dataItemModel = writers[4].DataSet.DataSetSource.PublishedVariables.PublishedData[1];
            Assert.Equal("ns=2;s=DipData", dataItemModel.PublishedVariableNodeId);
            dataItemModel = writers[4].DataSet.DataSetSource.PublishedVariables.PublishedData[2];
            Assert.Equal("nsu=http://microsoft.com/Opc/OpcPlc/;s=NegativeTrendData", dataItemModel.PublishedVariableNodeId);
        }

        [Fact]
        public void PnPlcJobTestWithEvents()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://desktop-fhd2fr4:62563/Quickstarts/SimpleEventsServer"",
        ""UseSecurity"": false,
        ""OpcNodes"": [
            {
                ""Id"": ""i=2253"",
                ""DisplayName"": ""DisplayName2253"",
                ""OpcPublishingInterval"": 5000,
                ""EventFilter"": {
                    ""SelectClauses"": [
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""EventId""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""EventType""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""SourceNode""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""SourceName""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""Time""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""ReceiveTime""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""LocalTime""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""Message""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""Severity""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""2:CycleId""
                            ]
                        },
                        {
                            ""TypeDefinitionId"": ""i=2041"",
                            ""BrowsePath"": [
                                ""2:CurrentStep""
                            ]
                        }
                    ],
                    ""WhereClause"": {
                        ""Elements"": [
                            {
                                ""FilterOperator"": ""OfType"",
                                ""FilterOperands"": [
                                    {
                                        ""Value"": ""ns=2;i=235""
                                    }
                                ]
                            }
                        ]
                    }
                }
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(writerGroups[0].DataSetWriters);
            Assert.Empty(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Single(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData);

            Assert.All(j.DataSetWriters, dataSetWriter =>
               Assert.Single(dataSetWriter.DataSet.DataSetSource.PublishedEvents.PublishedData));

            var eventModel = writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData[0];
            Assert.Equal("DisplayName2253", eventModel.PublishedEventName);
            Assert.Equal("i=2253", eventModel.EventNotifier);
            Assert.Equal(11, eventModel.SelectClauses.Count);
            Assert.All(eventModel.SelectClauses, x =>
            {
                Assert.Equal("i=2041", x.TypeDefinitionId);
                Assert.Single(x.BrowsePath);
            });
            Assert.Equal(new[] {
                "EventId",
                "EventType",
                "SourceNode",
                "SourceName",
                "Time",
                "ReceiveTime",
                "LocalTime",
                "Message",
                "Severity",
                "2:CycleId",
                "2:CurrentStep"
            }, eventModel.SelectClauses.Select(x => x.BrowsePath[0]));
        }

        [Fact]
        public void PnPlcJobTestWithConditionHandling()
        {
            const string pn = @"
[
    {
        ""EndpointUrl"": ""opc.tcp://desktop-fhd2fr4:62563/Quickstarts/SimpleEventsServer"",
        ""UseSecurity"": false,
        ""OpcNodes"": [
            {
                ""Id"": ""i=2253"",
                ""OpcPublishingInterval"": 5000,
                ""EventFilter"": {
                    ""TypeDefinitionId"": ""ns=2;i=235""
                },
                ""ConditionHandling"": {
                    ""UpdateInterval"": 10,
                    ""SnapshotInterval"": 30
                }
            }
        ]
    }
]
";

            var logger = Log.Console<PublishedNodesConverter>();

            var converter = new PublishedNodesConverter(logger, _serializer);

            var entries = converter.Read(pn);
            var writerGroups = converter.ToWriterGroups(entries, GetOptions()).ToList();

            Assert.NotEmpty(writerGroups);
            var j = Assert.Single(writerGroups);
            Assert.Single(writerGroups[0].DataSetWriters);
            Assert.Empty(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedVariables.PublishedData);
            Assert.Single(writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData);

            Assert.All(j.DataSetWriters, dataSetWriter =>
               Assert.Single(dataSetWriter.DataSet.DataSetSource.PublishedEvents.PublishedData));

            var eventModel = writerGroups[0].DataSetWriters[0].DataSet.DataSetSource.PublishedEvents.PublishedData[0];
            Assert.Equal("i=2253", eventModel.EventNotifier);
            Assert.Equal("ns=2;i=235", eventModel.TypeDefinitionId);
            Assert.NotNull(eventModel.ConditionHandling);
            Assert.Equal(10, eventModel.ConditionHandling.UpdateInterval);
            Assert.Equal(30, eventModel.ConditionHandling.SnapshotInterval);
        }

        private static PublisherOptions GetOptions()
        {
            var options = new PublisherConfig(new ConfigurationBuilder().Build()).ToOptions();
            options.Value.MessagingProfile = MessagingProfile.Get(MessagingMode.Samples, MessageEncoding.Json);
            return options.Value;
        }

        private readonly IJsonSerializer _serializer = new NewtonsoftJsonSerializer();
    }
}
