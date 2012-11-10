﻿// --------------------------------------------------------------------------------------------
// <copyright file="CommandTreeBuilder.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.Common
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal static class CommandTreeBuilder
    {
        public static DbCommandTree CreateSelectAll(MetadataWorkspace workspace, EntitySet entitySet)
        {
            ConstructorInfo treeConstructor =
                typeof(DbQueryCommandTree)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(MetadataWorkspace), typeof(DataSpace), typeof(DbExpression) },
                    null);

            Type expressionBuilder = typeof(DbExpression).Assembly.GetType("System.Data.Common.CommandTrees.ExpressionBuilder.DbExpressionBuilder");

            object scanExpression = expressionBuilder.GetMethod("Scan").Invoke(null, new object[] { entitySet });

            object tree = treeConstructor.Invoke(new object[] { workspace, DataSpace.SSpace, scanExpression });
            return tree as DbCommandTree;
        }
    }
}
