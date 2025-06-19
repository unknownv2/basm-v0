using System;
using Basm.Architecture.X64.Assemblers.Tests;
using NUnit.Framework;
using KeystoneNET;
namespace Basm.Architecture.X64.Assemblers.Keystone.Tests
{
    [TestFixture]
    public class KeystoneAssemblerTest : AssemblerTest
    {
        protected override IX64InstructionAssembler MakeAssembler()
        {
            return new KeystoneAssembler();
        }
        [Test]
        public void TestKeystoneNativeSymbolResolver()
        {
            using (var keystone = new KeystoneNET.Keystone(KeystoneArchitecture.KS_ARCH_X86,
                KeystoneMode.KS_MODE_64, false))
            {
                ulong address = 0;
                
                keystone.ResolveSymbol += (string s, ref ulong w) =>
                {
                    if (s == "test")
                    {
                        w = 5;//0x1234abcd;
                        return true;
                    }
                    return false;
                };

                KeystoneEncoded enc = keystone.Assemble("mov dword ptr [rax+44], 20", address);


            }

            throw new NotImplementedException();
        }
    }
}
