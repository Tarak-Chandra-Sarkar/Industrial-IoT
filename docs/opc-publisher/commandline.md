# OPC Publisher configuration via command line options and environment variables

[Home](./readme.md)

The following OPC Publisher configuration can be applied by Command Line Interface (CLI) options or as environment variable settings.

CamelCase options can also be provided using environment variables. When both environment variable and CLI argument are provided, the command line option will override the environment variable.

```text

 ██████╗ ██████╗  ██████╗    ██████╗ ██╗   ██╗██████╗ ██╗     ██╗███████╗██╗  ██╗███████╗██████╗
██╔═══██╗██╔══██╗██╔════╝    ██╔══██╗██║   ██║██╔══██╗██║     ██║██╔════╝██║  ██║██╔════╝██╔══██╗
██║   ██║██████╔╝██║         ██████╔╝██║   ██║██████╔╝██║     ██║███████╗███████║█████╗  ██████╔╝
██║   ██║██╔═══╝ ██║         ██╔═══╝ ██║   ██║██╔══██╗██║     ██║╚════██║██╔══██║██╔══╝  ██╔══██╗
╚██████╔╝██║     ╚██████╗    ██║     ╚██████╔╝██████╔╝███████╗██║███████║██║  ██║███████╗██║  ██║
 ╚═════╝ ╚═╝      ╚═════╝    ╚═╝      ╚═════╝ ╚═════╝ ╚══════╝╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
                                                                         2.9.0

General
-------

  -h, --help                 Show help and exit.
  -f, --pf, --publishfile, --PublishedNodesFile=VALUE
                             The name of the file containing the configuration
                               of the nodes to be published as well as the
                               information to connect to the OPC UA server
                               sources.
                               This file is also used to persist changes made
                               through the control plane, e.g., through IoT Hub
                               device method calls.
                               When this file is specified, or the default file
                               is accessible by the module, OPC Publisher will
                               start in standalone mode.
                               Default: `publishednodes.json`
      --id, --publisherid, --PublisherId=VALUE
                             Sets the publisher id of the publisher.
                               Default: `not set` which results in the IoT edge
                               identity being used
  -s, --site, --SiteId=VALUE Sets the site name of the publisher module.
                               Default: `not set`
      --rs, --runtimestatereporting, --RuntimeStateReporting[=VALUE]
                             Enable that when publisher starts or restarts it
                               reports its runtime state using a restart
                               message.
                               Default: `False` (disabled)

Messaging configuration
-----------------------

  -c, --strict, --UseStandardsCompliantEncoding[=VALUE]
                             Use strict UA compliant encodings. Default is '
                               false' for backwards (2.5.x - 2.8.x)
                               compatibility. It is recommended to run the
                               publisher in compliant mode for best
                               interoperability.
                               Default: `False`
      --mm, --messagingmode, --MessagingMode=VALUE
                             The messaging mode for messages
                               Allowed values:
                                   `PubSub`
                                   `Samples`
                                   `FullNetworkMessages`
                                   `FullSamples`
                                   `DataSetMessages`
                                   `RawDataSets`
                               Default: `PubSub` if `-c` is specified,
                               otherwise `Samples` for backwards compatibility.
      --me, --messageencoding, --MessageEncoding=VALUE
                             The message encoding for messages
                               Allowed values:
                                   `Binary`
                                   `Json`
                                   `Xml`
                                   `IsReversible`
                                   `Uadp`
                                   `JsonReversible`
                                   `IsGzipCompressed`
                                   `JsonGzip`
                                   `JsonReversibleGzip`
                               Default: `Json`.
      --bi, --batchtriggerinterval, --BatchTriggerInterval=VALUE
                             The network message publishing interval in
                               milliseconds. Determines the publishing period
                               at which point messages are emitted.
                               When `--bs` is 1 and `--bi` is set to 0 batching
                               is disabled.
                               Default: `10000` (10 seconds).
                               Alternatively can be set using `
                               BatchTriggerInterval` environment variable in
                               the form of a time span string formatted string `
                               [d.]hh:mm:ss[.fffffff]`.
      --bs, --batchsize, --BatchSize=VALUE
                             The number of incoming OPC UA subscription
                               notifications to collect until sending a network
                               messages. When `--bs` is set to 1 and `--bi` is
                               0 batching is disabled and messages are sent as
                               soon as notifications arrive.
                               Default: `50`.
      --ms, --maxmessagesize, --iothubmessagesize, --IoTHubMaxMessageSize=VALUE
                             The maximum size of the messages to emit. In case
                               the encoder cannot encode a message because the
                               size would be exceeded, the message is dropped.
                               Otherwise the encoder will aim to chunk messages
                               if possible.
                               Default: `256k` in case of IoT Hub messages, `0`
                               otherwise.
      --npd, --maxnodesperdataset, --MaxNodesPerDataSet=VALUE
                             Maximum number of nodes within a Subscription.
                               When there are more nodes configured for a data
                               set writer, they will be added to new
                               subscriptions. This also affects metadata
                               message size.
                               Default: `1000`.
      --kfc, --keyframecount, --DefaultKeyFrameCount=VALUE
                             The default number of delta messages to send until
                               a key frame message is sent. If 0, no key frame
                               messages are sent, if 1, every message will be a
                               key frame.
                               Default: `0`.
      --msi, --metadatasendinterval, --DefaultMetaDataUpdateTime=VALUE
                             Default value in milliseconds for the metadata
                               send interval which determines in which interval
                               metadata is sent.
                               Even when disabled, metadata is still sent when
                               the metadata version changes unless `--mm=*
                               Samples` is set in which case this setting is
                               ignored. Only valid for network message
                               encodings.
                               Default: `0` which means periodic sending of
                               metadata is disabled.
      --dm, --disablemetadata, --DisableDataSetMetaData[=VALUE]
                             Disables sending any metadata when metadata
                               version changes. This setting can be used to
                               also override the messaging profile's default
                               support for metadata sending.
                               Default: `False` if the messaging profile
                               selected supports sending metadata, `True`
                               otherwise.
      --om, --maxoutgressmessages, --MaxOutgressMessages=VALUE
                             The maximum number of messages to buffer on the
                               send path before messages are dropped.
                               Default: `4096`

Transport settings
------------------

  -b, --mqc, --mqttclientconnectionstring, --MqttClientConnectionString=VALUE
                             An mqtt connection string to use. Use this option
                               to connect OPC Publisher to a MQTT Broker
                               endpoint.
                               To connect to an MQTT broker use the format '
                               HostName=<IPorDnsName>;Port=<Port>[;Username=<
                               Username>;Password=<Password>;Protocol=<'v5'|'
                               v311'>]'.
                               Default: `not set`.
      --ec, --edgehubconnectionstring, --dc, --deviceconnectionstring, --EdgeHubConnectionString=VALUE
                             A edge hub or iot hub connection string to use if
                               you run OPC Publisher outside of IoT Edge. The
                               connection string can be obtained from the IoT
                               Hub portal. It is not required to use this
                               option if running inside IoT Edge.
                               Default: `not set`.
      --ht, --ih, --iothubprotocol, --Transport=VALUE
                             Protocol to use for communication with EdgeHub.
                               Allowed values:
                                   `None`
                                   `AmqpOverTcp`
                                   `AmqpOverWebsocket`
                                   `Amqp`
                                   `MqttOverTcp`
                                   `Tcp`
                                   `MqttOverWebsocket`
                                   `Websocket`
                                   `Mqtt`
                                   `Any`
                               Default: `Mqtt` if device or edge hub connection
                               string is provided, ignored otherwise.
      --http, --httpserver, --EnableHttpServer[=VALUE]
                             Specify this to enable the OPC Publisher REST api.
                               .Default: `disabled`.
  -p, --httpserverport, --HttpServerPort=VALUE
                             The port on which the REST api of OPC Publisher is
                               is listening. Implicitly enables the http server
                               and REST api capabilities.
                               Default: `not set` if https is not enabled,
                               otherwise `443` if no value is provided.

Routing configuration
---------------------

      --rtt, --roottopictemplate, --RootTopicTemplate[=VALUE]
                             The default root topic of OPC Publisher.
                               If not specified, the `{PublisherId}` template
                               is the root topic.
                               Currently only the template variables
                                   `{SiteId}` and
                                   `{PublisherId}`
                               can be used as dynamic substituations in the
                               template. If the template variable does not
                               exist it is replaced with the `$default` string.
                               Default: `{PublisherId}`.
      --mtt, --methodtopictemplate, --MethodTopicTemplate=VALUE
                             The topic at which OPC Publisher's method handler
                               is mounted.
                               If not specified, the `{RootTopic}/methods`
                               template will be used as root topic with the
                               method names as sub topic.
                               Only
                                   `{RootTopic}`
                                   `{SiteId}` and
                                   `{PublisherId}`
                               can currently be used as replacement variables
                               in the template.
                               Default: `{RootTopic}/methods`.
      --ttt, --telemetrytopictemplate, --TelemetryTopicTemplate[=VALUE]
                             The default topic that all messages are sent to.
                               If not specified, the `{RootTopic}/messages/{
                               DataSetWriterGroup}` template will be used as
                               root topic for all events sent by OPC Publisher.
                               The template variables
                                   `{RootTopic}`
                                   `{SiteId}`
                                   `{PublisherId}`
                                   `{DataSetClassId}`
                                   `{DataSetWriterName}` and
                                   `{DataSetWriterGroup}`
                                can be used as dynamic parts in the template.
                               If a template variable does not exist the name
                               of the variable is emitted.
                               Default: `{RootTopic}/messages/{
                               DataSetWriterGroup}`.
      --ett, --eventstopictemplate, --EventsTopicTemplate=VALUE
                             The topic into which OPC Publisher publishes any
                               events that are not telemetry messages such as
                               discovery or runtime events.
                               If not specified, the `{RootTopic}/events`
                               template will be used.
                               Only
                                   `{RootTopic}`
                                   `{SiteId}` and
                                   `{PublisherId}`
                               can currently be used as replacement variables
                               in the template.
                               Default: `{RootTopic}/events`.
      --mdt, --metadatatopictemplate, --DataSetMetaDataTopicTemplate[=VALUE]
                             The topic that metadata should be sent to.
                               In case of MQTT the message will be sent as
                               RETAIN message with a TTL of either metadata
                               send interval or infinite if metadata send
                               interval is not configured.
                               Only valid if metadata is supported and/or
                               explicitely enabled.
                               The template variables
                                   `{RootTopic}`
                                   `{SiteId}`
                                   `{TelemetryTopic}`
                                   `{PublisherId}`
                                   `{DataSetClassId}`
                                   `{DataSetWriterName}` and
                                   `{DatasetWriterGroup}`
                               can be used as dynamic parts in the template.
                               Default: `{TelemetryTopic}` which means metadata
                               is sent to the same output as regular messages.
                               If specified without value, the default output
                               is `{TelemetryTopic}/$metadata`.
      --ri, --enableroutinginfo, --EnableRoutingInfo[=VALUE]
                             Add routing information to messages. The name of
                               the property is `$$RoutingInfo` and the value is
                               the `DataSetWriterGroup` for that particular
                               message.
                               When the `DataSetWriterGroup` is not configured,
                               the `$$RoutingInfo` property will not be added
                               to the message even if this argument is set.
                               Default: `False`.

Subscription settings
---------------------

      --oi, --opcsamplinginterval, --DefaultSamplingInterval=VALUE
                             Default value in milliseconds to request the
                               servers to sample values. This value is used if
                               an explicit sampling interval for a node was not
                               configured.
                               Default: `1000`.
                               Alternatively can be set using `
                               DefaultSamplingInterval` environment variable in
                               the form of a time span string formatted string `
                               [d.]hh:mm:ss[.fffffff]`.
      --op, --opcpublishinginterval, --DefaultPublishingInterval=VALUE
                             Default value in milliseconds for the publishing
                               interval setting of a subscription created with
                               an OPC UA server. This value is used if an
                               explicit publishing interval was not configured.
                               Default: `1000`.
                               Alternatively can be set using `
                               DefaultPublishingInterval` environment variable
                               in the form of a time span string formatted
                               string `[d.]hh:mm:ss[.fffffff]`.
      --kt, --keepalivethreshold, --MaxKeepAliveCount=VALUE
                             Specify the number of keep alive packets a server
                               can miss, before the session is disconneced.
                               Default: `50`.
      --fd, --fetchdisplayname, --FetchOpcNodeDisplayName[=VALUE]
                             Fetches the displayname for the monitored items
                               subscribed if a display name was not specified
                               in the configuration.
                               Note: This has high impact on OPC Publisher
                               startup performance.
                               Default: `False` (disabled).
      --qs, --queuesize, --DefaultQueueSize=VALUE
                             Default queue size for all monitored items if
                               queue size was not specified in the
                               configuration.
                               Default: `1` (for backwards compatibility).
      --ndo, --nodiscardold, --DiscardNew[=VALUE]
                             The publisher is using this as default value for
                               the discard old setting of monitored item queue
                               configuration. Setting to true will ensure that
                               new values are dropped before older ones are
                               drained.
                               Default: `False` (which is the OPC UA default).
      --mc, --monitoreditemdatachangetrigger, --DefaulDataChangeTrigger=VALUE
                             Default data change trigger for all monitored
                               items configured in the published nodes
                               configuration unless explicitly overridden.
                               Allowed values:
                                   `Status`
                                   `StatusValue`
                                   `StatusValueTimestamp`
                               Default: `StatusValue` (which is the OPC UA
                               default).
      --sf, --skipfirst, --DefaultSkipFirst[=VALUE]
                             The publisher is using this as default value for
                               the skip first setting of nodes configured
                               without a skip first setting. A value of True
                               will skip sending the first notification
                               received when the monitored item is added to the
                               subscription.
                               Default: `False` (disabled).
      --hb, --heartbeatinterval, --DefaultHeartbeatInterval=VALUE
                             The publisher is using this as default value in
                               seconds for the heartbeat interval setting of
                               nodes that were configured without a heartbeat
                               interval setting. A heartbeat is sent at this
                               interval if no value has been received.
                               Default: `0` (disabled)
                               Alternatively can be set using `
                               DefaultHeartbeatInterval` environment variable
                               in the form of a time span string formatted
                               string `[d.]hh:mm:ss[.fffffff]`.
      --slt, --MinSubscriptionLifetime=VALUE
                             Minimum subscription lifetime in seconds as per
                               OPC UA definition.
                               Default: `not set`.

OPC UA Client configuration
---------------------------

      --ki, --keepaliveinterval, --KeepAliveInterval=VALUE
                             The interval in seconds the publisher is sending
                               keep alive messages to the OPC servers on the
                               endpoints it is connected to.
                               Default: `10000` (10 seconds).
      --aa, --acceptuntrusted, --AutoAcceptUntrustedCertificates[=VALUE]
                             The publisher accepts untrusted certificates
                               presented by a server it connects to.
                               This does not include servers presenting bad
                               certificates or certificates that fail chain
                               validation. These errors cannot be suppressed
                               and connection will always be rejected.
                               WARNING: This setting should never be used in
                               production environments!
      --ot, --operationtimeout, --OperationTimeout=VALUE
                             The operation service call timeout of the
                               publisher OPC UA client in milliseconds.
                               Default: `120000` (2 minutes).
      --ct, --createsessiontimeout, --DefaultSessionTimeout=VALUE
                             Maximum amount of time in seconds that a session
                               should remain open by the OPC server without any
                               activity (session timeout) to request from the
                               OPC server at session creation.
                               Default: `not set`.
      --rd, --reconnectdelay, --ReconnectRetryDelay=VALUE
                             Amount of time in seconds to wait between client
                               reconnecting to the server to reastablish
                               connectivity.
                               Set to 0 to disable reconnect handling and
                               instead recreate the session when required.
                               Default: `5` (5 seconds).
      --otl, --opctokenlifetime, --SecurityTokenLifetime=VALUE
                             OPC UA Stack Transport Secure Channel - Security
                               token lifetime in milliseconds.
                               Default: `3600000` (1h).
      --ocl, --opcchannellifetime, --ChannelLifetime=VALUE
                             OPC UA Stack Transport Secure Channel - Channel
                               lifetime in milliseconds.
                               Default: `300000` (5 min).
      --omb, --opcmaxbufferlen, --MaxBufferSize=VALUE
                             OPC UA Stack Transport Secure Channel - Max buffer
                               size.
                               Default: `65535` (64KB -1).
      --oml, --opcmaxmessagelen, --MaxMessageSize=VALUE
                             OPC UA Stack Transport Secure Channel - Max
                               message size.
                               Default: `4194304` (4 MB).
      --oal, --opcmaxarraylen, --MaxArrayLength=VALUE
                             OPC UA Stack Transport Secure Channel - Max array
                               length.
                               Default: `65535` (64KB - 1).
      --ol, --opcmaxstringlen, --MaxStringLength=VALUE
                             The max length of a string opc can transmit/
                               receive over the OPC UA secure channel.
                               Default: `130816` (128KB - 256).
      --obl, --opcmaxbytestringlen, --MaxByteStringLength=VALUE
                             OPC UA Stack Transport Secure Channel - Max byte
                               string length.
                               Default: `1048576` (1MB).
      --au, --appuri, --ApplicationUri=VALUE
                             Application URI as per OPC UA definition inside
                               the OPC UA client application configuration
                               presented to the server.
                               Default: `not set`.
      --pu, --producturi, --ProductUri=VALUE
                             The Product URI as per OPC UA definition insde the
                               OPC UA client application configuration
                               presented to the server.
                               Default: `not set`.
      --rejectsha1, --RejectSha1SignedCertificates=VALUE
                             If set OPC Publisher will reject SHA1 certificates
                               which have been officially deprecated and are
                               unsafe to use.
                               Note: It is recommended to always set this value
                               to `True` if the connected OPC UA servers does
                               not use Sha1 signed certificates.
                               Default: `False` (to support older equipment).
      --mks, --minkeysize, --MinimumCertificateKeySize=VALUE
                             Minimum accepted certificate size.
                               Note: It is recommended to this value to the
                               highest certificate key size possible based on
                               the connected OPC UA servers.
                               Default: 1024.
      --tm, --trustmyself, --AddAppCertToTrustedStore=VALUE
                             Set to `False` to disable adding the publisher's
                               own certificate to the trusted store
                               automatically.
                               Default: `True`.
      --sn, --appcertsubjectname, --ApplicationCertificateSubjectName=VALUE
                             The subject name for the app cert.
                               Default: `CN=Microsoft.Azure.IIoT, C=DE, S=Bav,
                               O=Microsoft, DC=localhost`.
      --an, --appname, --ApplicationName=VALUE
                             The name for the app (used during OPC UA
                               authentication).
                               Default: `Microsoft.Azure.IIoT`
      --pki, --pkirootpath, --PkiRootPath=VALUE
                             PKI certificate store root path.
                               Default: `pki`.
      --ap, --appcertstorepath, --ApplicationCertificateStorePath=VALUE
                             The path where the own application cert should be
                               stored.
                               Default: $"{PkiRootPath}/own".
      --apt, --at, --appcertstoretype, --ApplicationCertificateStoreType=VALUE
                             The own application cert store type.
                               Allowed values:
                                   `Directory`
                                   `X509Store`
                               Default: `Directory`.
      --tp, --trustedcertstorepath, --TrustedPeerCertificatesPath=VALUE
                             The path of the trusted cert store.
                               Default: $"{PkiRootPath}/trusted".
      --tpt, --TrustedPeerCertificatesType=VALUE
                             Trusted peer certificate store type.
                               Allowed values:
                                   `Directory`
                                   `X509Store`
                               Default: `Directory`.
      --rp, --rejectedcertstorepath, --RejectedCertificateStorePath=VALUE
                             The path of the rejected cert store.
                               Default: $"{PkiRootPath}/rejected".
      --rpt, --RejectedCertificateStoreType=VALUE
                             Rejected certificate store type.
                               Allowed values:
                                   `Directory`
                                   `X509Store`
                               Default: `Directory`.
      --ip, --issuercertstorepath, --TrustedIssuerCertificatesPath=VALUE
                             The path of the trusted issuer cert store.
                               Default: $"{PkiRootPath}/issuers".
      --ipt, --TrustedIssuerCertificatesType=VALUE
                             Trusted issuer certificate store types.
                               Allowed values:
                                   `Directory`
                                   `X509Store`
                               Default: `Directory`.

Diagnostic options
------------------

      --di, --diagnosticsinterval, --DiagnosticsInterval=VALUE
                             Shows publisher diagnostic information at this
                               specified interval in seconds in the OPC
                               Publisher log (need log level info). `-1`
                               disables remote diagnostic log and diagnostic
                               output.
                               Default:60000 (60 seconds).
                               Alternatively can be set using `
                               DiagnosticsInterval` environment variable in the
                               form of a time span string formatted string `[d.]
                               hh:mm:ss[.fffffff]`".
      --ll, --loglevel, --LogLevel=VALUE
                             The loglevel to use.
                               Allowed values:
                                   `Trace`
                                   `Debug`
                                   `Information`
                                   `Warning`
                                   `Error`
                                   `Critical`
                                   `None`
                               Default: `Information`.
```

Currently supported combinations of `--mm` snd `--me` can be found [here](./messageformats.md).
