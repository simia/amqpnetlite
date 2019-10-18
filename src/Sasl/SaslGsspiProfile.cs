using System;
using System.Collections.Generic;
using System.Text;
using Amqp.Framing;
using Amqp.Types;
using NSspi.Contexts;
using NSspi.Credentials;

namespace Amqp.Sasl
{
    sealed class SaslGsspiProfile : SaslProfile
    {
        //[StructLayout(LayoutKind.Sequential)]
        //public struct TimeStamp
        //{
        //    /// <summary>
        //    /// Returns the calendar date and time corresponding a zero timestamp.
        //    /// </summary>
        //    public static readonly DateTime Epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //    /// <summary>
        //    /// Stores the time value. Infinite times are often represented as values near, but not exactly
        //    /// at the maximum signed 64-bit 2's complement value.
        //    /// </summary>
        //    private long time;

        //    /// <summary>
        //    /// Converts the TimeStamp to an equivalant DateTime object. If the TimeStamp represents
        //    /// a value larger than DateTime.MaxValue, then DateTime.MaxValue is returned.
        //    /// </summary>
        //    /// <returns></returns>
        //    public DateTime ToDateTime()
        //    {
        //        ulong test = (ulong)this.time + (ulong)(Epoch.Ticks);

        //        // Sometimes the value returned is massive, eg, 0x7fffff154e84ffff, which is a value
        //        // somewhere in the year 30848. This would overflow DateTime, since it peaks at 31-Dec-9999.
        //        // It turns out that this value corresponds to a TimeStamp's maximum value, reduced by my local timezone
        //        // http://stackoverflow.com/questions/24478056/
        //        if (test > (ulong)DateTime.MaxValue.Ticks)
        //        {
        //            return DateTime.MaxValue;
        //        }
        //        else
        //        {
        //            return DateTime.FromFileTimeUtc(this.time);
        //        }
        //    }
        //}

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //internal struct RawSspiHandle
        //{
        //    private IntPtr lowPart;
        //    private IntPtr highPart;

        //    /// <summary>
        //    /// Returns whether or not the handle is set to the default, empty value.
        //    /// </summary>
        //    /// <returns></returns>
        //    public bool IsZero()
        //    {
        //        return this.lowPart == IntPtr.Zero && this.highPart == IntPtr.Zero;
        //    }

        //    /// <summary>
        //    /// Sets the handle to an invalid value.
        //    /// </summary>
        //    /// <remarks>
        //    /// This method is executed in a CER during handle release.
        //    /// </remarks>            
        //    public void SetInvalid()
        //    {
        //        this.lowPart = IntPtr.Zero;
        //        this.highPart = IntPtr.Zero;
        //    }
        //}

        /////[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        //[DllImport("Secur32.dll", EntryPoint = "AcquireCredentialsHandle", CharSet = CharSet.Unicode)]
        //internal static extern uint AcquireCredentialsHandle(
        //    string principleName,
        //    string packageName,
        //    uint credentialUse,
        //    IntPtr loginId,
        //    IntPtr packageData,
        //    IntPtr getKeyFunc,
        //    IntPtr getKeyData,
        //    ref RawSspiHandle credentialHandle,
        //    ref TimeStamp expiry
        //);
        ClientCurrentCredential clientCred = null;

        ClientContext client = null;

        public SaslGsspiProfile(string name, string empty)
            : base(name)
        {
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
                SaslChallenge c = (SaslChallenge)command;//new SaslChallenge { Challenge = command };
                return new SaslOutcome() { Code = SaslCode.Ok };
                //return c;
                //Trace.WriteLine(TraceLevel.Frame, "Challenge: {0}", c);

            }
            return null;
            //throw new NotImplementedException();
        }

        protected override ITransport UpgradeTransport(ITransport transport)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}
