using System;
using System.Collections.Generic;
using System.Text;
using Amqp.Framing;
using Amqp.Types;
using NSspi;
using NSspi.Contexts;
using NSspi.Credentials;

namespace Amqp.Sasl
{
    sealed class SaslGssapiProfile : SaslProfile
    {
        ClientCurrentCredential clientCred = null;
        ClientContext client = null;
        byte[] clientToken;

        SecurityStatus clientStatus;

        public SaslGssapiProfile(string name, string principal)
            : base(name)
        {
            clientCred = new ClientCurrentCredential(PackageNames.Kerberos);
            client = new ClientContext(
                    clientCred,
                    principal,
                    ContextAttrib.MutualAuth |
                    //ContextAttrib.InitIdentify |
                    //ContextAttrib.Confidentiality |
                    //ContextAttrib.ReplayDetect |
                    //ContextAttrib.SequenceDetect |
                    //ContextAttrib.Connection |
                    ContextAttrib.Delegate
                );
            clientToken = null;
        }

        protected override DescribedList GetStartCommand(string hostname)
        {
            SaslInit init = new SaslInit()
            {
                Mechanism = this.Mechanism,
                HostName = hostname
            };
            return init;
        }

         
        protected override DescribedList OnCommand(DescribedList command)
        {
            Trace.WriteLine(TraceLevel.Frame, "On Command: {0}", command);
            if (command.Descriptor.Code == Codec.SaslChallenge.Code)
            {
                try
                {
                    SaslChallenge c = (SaslChallenge)command;
                    clientStatus = client.Init(c.Challenge.Length == 0 ? null : c.Challenge, out clientToken);
                }
                catch
                {
                    return new SaslOutcome() { Code = SaslCode.Auth }; 
                }
                return new SaslResponse() { Response = clientToken };
            }

            return null;
        }

        protected override ITransport UpgradeTransport(ITransport transport)
        {
            return transport;
        }
    }
}
