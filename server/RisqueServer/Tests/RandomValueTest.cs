using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace RisqueServer.Tests {
    [TestFixture]
    class RandomValueTest {
        [TestCase]
        public void CanGetValues() {
            for (int i = 0; i < 16; i++) {
                int r1 = 0, r2 = 0, r3 = 0, r4 = 0, r5 = 0;
                Assert.DoesNotThrow(() => {
                    r1 = Security.SecurityManager.randomSecurityValue();
                    r2 = Security.SecurityManager.randomSecurityValue();
                    r3 = Security.SecurityManager.randomSecurityValue();
                    r4 = Security.SecurityManager.randomSecurityValue();
                    r5 = Security.SecurityManager.randomSecurityValue();
                });
                Console.WriteLine(r1);
                Console.WriteLine(r2);
                Console.WriteLine(r3);
                Console.WriteLine(r4);
                Console.WriteLine(r5);
            }
        }
    }
}
