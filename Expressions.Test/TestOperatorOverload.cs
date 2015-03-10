﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Expressions.Test
{
    abstract class BaseClass
    {
        protected double Val { get; set; }

        public static double operator +(BaseClass first, BaseClass second)
        {
            return first.Val + second.Val;
        }
    }

    class FirstClass : BaseClass
    {
        public FirstClass(double val)
        {
            this.Val = val;
        }
    }

    class SecondClass : BaseClass
    {
        public SecondClass(double val)
        {
            this.Val = val;
        }
    }

    class TestOperatorOverload
    {
        [Test]
        public void TestOverload()
        {
            var a = new FirstClass(5);
            var b = new SecondClass(7);

            var c = a + b;

            var context = new ExpressionContext();
            context.Variables.Add("a", a);
            context.Variables.Add("b", b);

            var expr = new DynamicExpression<double>("a + b", ExpressionLanguage.Csharp);

            var s = expr.Invoke(context);
        }

    }
}
