using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.AspNetCore.BackgroundTasks.Tests
{
    public static class Assertion
    {
        public static void ShouldContain<T>(this IEnumerable source)
        {
            Assert.IsNotNull(source);
            if (!source.OfType<T>().Any())
                throw new AssertFailedException($"The collection did not contain any items of type '{typeof(T)}'.");
        }
    }
}
