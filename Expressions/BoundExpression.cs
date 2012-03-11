﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions
{
    public sealed class BoundExpression
    {
        private readonly DynamicExpression _dynamicExpression;
        private readonly Type _ownerType;
        private readonly Import[] _imports;
        private readonly Type[] _identifierTypes;
        private readonly int[] _parameterMap;
        private readonly DynamicSignature _compiledMethod;

#if DEBUG
        // For unit testing
        internal IResolvedAstNode ResolvedTree { get; private set; }
#endif

        internal BoundExpression(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (imports == null)
                throw new ArgumentNullException("imports");
            if (identifierTypes == null)
                throw new ArgumentNullException("identifierTypes");

            _dynamicExpression = dynamicExpression;
            _ownerType = ownerType;
            _imports = imports;
            _identifierTypes = identifierTypes;

            _parameterMap = BuildParameterMap();

            Resolve();

            //_compiledMethod = CompileExpression();
        }

        private void Resolve()
        {
            var resolver = new Resolver(_dynamicExpression, _ownerType, _imports, _identifierTypes, _parameterMap);

            var resolvedTree = _dynamicExpression.ParseResult.RootNode.Resolve(resolver);

#if DEBUG
            ResolvedTree = resolvedTree;
#endif
        }

        private int[] BuildParameterMap()
        {
            var parameterMap = new List<int>();

            if (_ownerType != null)
                parameterMap.Add(-1);

            for (int i = 0; i < _identifierTypes.Length; i++)
            {
                if (_identifierTypes[i] != null)
                    parameterMap.Add(i);
            }

            return parameterMap.ToArray();
        }

        //private DynamicSignature CompileExpression()
        //{
        //    var method = new DynamicMethod(
        //        "DynamicMethod",
        //        typeof(object),
        //        new[] { typeof(object[]) },
        //        false /* restrictedSkipVisibility */
        //    );

        //    var il = method.GetILGenerator();

        //    new Compiler(_dynamicExpression, _ownerType, _imports, _identifierTypes, _parameterMap, il).Compile();

        //    return (DynamicSignature)method.CreateDelegate(typeof(DynamicSignature));
        //}

        public object Invoke(IExecutionContext executionContext)
        {
            if (executionContext == null)
                throw new ArgumentNullException("executionContext");

            var parameters = new object[_parameterMap.Length];

            bool ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
            var identifiers = _dynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < parameters.Length; i++)
            {
                int index = _parameterMap[i];

                if (index == -1)
                {
                    parameters[i] = executionContext.Owner;
                }
                else
                {
                    parameters[i] = executionContext.GetVariableValue(
                        identifiers[index].Name, ignoreCase
                    );
                }
            }

            return _compiledMethod(parameters);
        }

        private delegate object DynamicSignature(object[] parameters);
    }
}