﻿using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FabricTransportServiceRemotingProvider(
    RemotingListenerVersion = RemotingListenerVersion.V2,
    RemotingClientVersion = RemotingClientVersion.V2)]
