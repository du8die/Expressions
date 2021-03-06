﻿// From http://flee.codeplex.com/

using System.Text;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Expressions.Test
{
    [TestFixture()]
    internal abstract class BulkTests : ExpressionTests
    {
        protected BulkTests(ExpressionLanguage language)
            : base(language)
        {
        }

        [TestFixtureSetUp]
        public void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [Test(Description = "Expressions that should be valid")]
        public void TestValidExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("ValidExpressions.txt", DoTestValidExpressions);
        }

        [Test(Description = "Type descriptor expressions that should be valid")]
        [Ignore("Not yet implemented")]
        public void TestTypeDescriptorExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("TypeDescriptor.txt", DoTestValidExpressions);
        }

        [Test(Description = "On demand method expressions that should be valid")]
        [Ignore("Not yet implemented")]
        public void TestOnDemandMethodExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("OnDemandMethods.txt", DoTestValidExpressions);
        }

        [Test(Description = "Special construct expressions that should be valid")]
        public void TestSpecialConstructExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("SpecialConstructs.txt", DoTestValidExpressions);
        }

        [Test(Description = "Casts that should be valid")]
        public void TestValidCasts()
        {
            MyCurrentContext = MyValidCastsContext;
            ProcessScriptTests("ValidCasts.txt", DoTestValidExpressions);
        }

        [Test(Description = "Test our handling of checked expressions")]
        public void TestCheckedExpressions()
        {
            ProcessScriptTests("CheckedTests.txt", DoTestCheckedExpressions);
        }

        private void DoTestValidExpressions(string[] arr)
        {
            string typeName = string.Concat("System.", arr[0]);
            Type expressionType = Type.GetType(typeName, true, true);

            ExpressionContext context = MyCurrentContext;
            //context.Options.ResultType = expressionType;

            var expression = new DynamicExpression(arr[1], Language);

            DoTest(expression, context, arr[2], expressionType, ExpressionTests.TestCulture);
        }

        private void DoTestCheckedExpressions(string[] arr)
        {
            string expression = arr[0];
            bool @checked = bool.Parse(arr[1]);
            bool shouldOverflow = bool.Parse(arr[2]);

            var imports = new[]
            {
                new Import(typeof(Math))
           };

            ExpressionContext context = new ExpressionContext(imports, MyValidExpressionsOwner);

            try
            {
                var e = new DynamicExpression(expression, Language);
                e.Invoke(context, new BoundExpressionOptions
                {
                    Checked = @checked
                });
                Assert.IsFalse(shouldOverflow);
            }
            catch (OverflowException)
            {
                Assert.IsTrue(shouldOverflow);
            }
        }
    }
}
