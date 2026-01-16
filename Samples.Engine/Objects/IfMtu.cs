// Copyright (C) 2013 Lex Li
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

/*
 * Created by SharpDevelop.
 * User: Lex
 * Date: 3/3/2013
 * Time: 10:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Net.NetworkInformation;
using Samples.Pipeline;
using System;
using Lextm.SharpSnmpLib;

namespace Samples.Objects
{
    /// <summary>
    /// ifMtu object.
    /// </summary>
    internal sealed class IfMtu : ScalarObject
    {
        private readonly ISnmpData _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfMtu"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfMtu(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.4.{0}", index.ToString())
        {
            var ipProps = networkInterface.GetIPProperties();

            // Prefer IPv4
            if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
            {
                var ipv4 = ipProps.GetIPv4Properties();
                if (ipv4 != null)
                {
                    _data = new Integer32(ipv4.Mtu);
                    return;
                }
            }

            // Fallback to IPv6 (Hyper-V safe)
            if (networkInterface.Supports(NetworkInterfaceComponent.IPv6))
            {
                try
                {
                    var ipv6 = ipProps.GetIPv6Properties();
                    _data = new Integer32(ipv6?.Mtu ?? 0);
                    return;
                }
                catch (NetworkInformationException)
                {
                    // Hyper-V virtual adapters land here
                }
                catch (NotImplementedException)
                {
                }
            }

            // Unavailable / virtual / non-operational
            _data = new Integer32(0);
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        /// <exception cref="AccessFailureException"></exception>
        public override ISnmpData Data
        {
            get { return _data; }
            set { throw new AccessFailureException(); }
        }
    }
}
