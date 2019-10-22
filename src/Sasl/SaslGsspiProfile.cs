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
    sealed class SaslGsspiProfile : SaslProfile
    {
        ClientCurrentCredential clientCred = null;
        ClientContext client = null;
        byte[] clientToken;
        //byte[] serverToken;
        SecurityStatus clientStatus;

        public SaslGsspiProfile(string name, string empty)
            : base(name)
        {
            clientCred = new ClientCurrentCredential(PackageNames.Kerberos);
            client = new ClientContext(
                    clientCred,
                    "POWELASA\\toka",//serverCred.PrincipleName, 
                    ContextAttrib.MutualAuth |
                    //ContextAttrib.InitIdentify |
                    //ContextAttrib.Confidentiality |
                    //ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect |
                    //ContextAttrib.Connection |
                    ContextAttrib.Delegate
                );
            //client.Dispose();
            //clientCred.Dispose();
            clientToken = null;
            //clientStatus = client.Init(serverToken, out clientToken);
            //this.empty= empty;
        }

        protected override DescribedList GetStartCommand(string hostname)
        {
            SaslInit init = new SaslInit()
            {
                Mechanism = this.Mechanism,
                //InitialResponse = message,
                HostName = hostname
            };
            return init;
        }

         
        protected override DescribedList OnCommand(DescribedList command)
        {
            Trace.WriteLine(TraceLevel.Frame, "On Command: {0}", command);
            if (command.Descriptor.Code == Codec.SaslChallenge.Code)
            {
                //Trace.WriteLine(TraceLevel.Frame, "Challange: {0}", "bsdfsdf");
                SaslChallenge c = (SaslChallenge)command;
                clientStatus = client.Init(c.Challenge.Length == 0 ? null : c.Challenge, out clientToken);                

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
