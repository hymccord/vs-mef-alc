using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mef.Host
{
    internal static partial class ByValueEquality
    {
        internal static IEqualityComparer<AssemblyName> AssemblyName
        {
            get { return AssemblyNameComparer.Default; }
        }

        internal static IEqualityComparer<AssemblyName> AssemblyNameNoFastCheck
        {
            get { return AssemblyNameComparer.NoFastCheck; }
        }

        private class AssemblyNameComparer : IEqualityComparer<AssemblyName>
        {
            internal static readonly AssemblyNameComparer Default = new AssemblyNameComparer();
            internal static readonly AssemblyNameComparer NoFastCheck = new AssemblyNameComparer(fastCheck: false);
            private bool fastCheck;

            internal AssemblyNameComparer(bool fastCheck = true)
            {
                this.fastCheck = fastCheck;
            }

            public bool Equals(AssemblyName? x, AssemblyName? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                // If fast check is enabled, we can compare the code bases
                if (this.fastCheck && x.CodeBase == y.CodeBase)
                {
                    return true;
                }

                // There are some cases where two AssemblyNames who are otherwise equivalent
                // have a null PublicKey but a correct PublicKeyToken, and vice versa. We should
                // compare the PublicKeys first, but then fall back to GetPublicKeyToken(), which
                // will generate a public key token for the AssemblyName that has a public key and
                // return the public key token for the other AssemblyName.
                byte[]? xPublicKey = x.GetPublicKey();
                byte[]? yPublicKey = y.GetPublicKey();

                // Testing on FullName is horrifically slow.
                // So test directly on its components instead.
                if (xPublicKey != null && yPublicKey != null)
                {
                    return x.Name == y.Name
                        && Equals(x.Version, y.Version)
                        && string.Equals(x.CultureName, y.CultureName)
                        && ByValueEquality.Buffer.Equals(xPublicKey, yPublicKey);
                }

                return x.Name == y.Name
                    && Equals(x.Version, y.Version)
                    && string.Equals(x.CultureName, y.CultureName)
                    && ByValueEquality.Buffer.Equals(x.GetPublicKeyToken(), y.GetPublicKeyToken());
            }

            public int GetHashCode(AssemblyName obj) => obj.Name?.GetHashCode() ?? 0;
        }
    }
    internal static partial class ByValueEquality
    {
        internal static IEqualityComparer<byte[]> Buffer
        {
            get { return BufferComparer.Default; }
        }

        private class BufferComparer : IEqualityComparer<byte[]>
        {
            internal static readonly BufferComparer Default = new BufferComparer();

            private BufferComparer()
            {
            }

            public bool Equals(byte[]? x, byte[]? y)
            {
                if (x == y)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(byte[] obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
