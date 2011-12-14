﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Linq.Expressions;
using Effort.Helpers;

namespace Effort.DbCommandTreeTransform
{
	internal class CanonicalFunctionMapper
	{
		private Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>> mappings;
		private EdmTypeConverter converter;

		public CanonicalFunctionMapper()
		{
			this.converter = new EdmTypeConverter();
			this.mappings = new Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>>();

			// Mappingek leirasa: http://msdn.microsoft.com/en-us/library/bb738681.aspx

			this.mappings["Edm.Round"] = (f, args) => Expression.Call(null,
			   FindMethod(typeof(Math), "Round", converter.ConvertNotNull(f.Parameters[0].TypeUsage)),
			   ExpressionHelper.ConvertToNotNull(args[0]));

			this.mappings["Edm.ToUpper"] = (f, args) => Expression.Call(args[0],
				FindMethod(typeof(string), "ToUpper"));
			this.mappings["Edm.ToLower"] = (f, args) => Expression.Call(args[0],
				FindMethod(typeof(string), "ToLower"));
			this.mappings["Edm.Concat"] = (f, args) => Expression.Call(null,
				FindMethod(typeof(string), "Concat", typeof(string), typeof(string)), args[0], args[1]);
		}

		public MethodCallExpression CreateMethodCall(EdmFunction function, Expression[] arguments)
		{
			try
			{
				return this.mappings[function.FullName](function, arguments);
			}
			catch (KeyNotFoundException exp)
			{
				throw new InvalidOperationException("There is no matching CLR function in MMDB for function " + function.FullName + '.', exp);
			}
		}
		private MethodInfo FindMethod(Type type, string methodName, params Type[] parameterTypes)
		{
			return type.GetMethods().Where(mi => mi.Name == methodName
				&& mi.GetParameters().Count() == parameterTypes.Length
				&& mi.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes)
				).Single();
		}
	}
}
