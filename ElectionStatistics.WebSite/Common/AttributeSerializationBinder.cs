using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Newtonsoft.Json.Serialization;

namespace ElectionStatistics.WebSite
{
	public class AttributeSerializationBinder : ISerializationBinder
	{
		private static Dictionary<string, Type> TypesByName { get; }
		private static Dictionary<Type, string> NameByTypes { get; }

		static AttributeSerializationBinder()
		{
			var typeAndNames = Assembly.GetCallingAssembly().GetTypes()
				.Select(type => new
				{
					Type = type,
					Name = type
						.GetCustomAttributes(typeof(DataContractAttribute), inherit: false)
						.OfType<DataContractAttribute>()
						.SingleOrDefault()
						?.Name
				})
				.Where(arg => arg.Name != null)
				.ToArray();

			TypesByName = typeAndNames.ToDictionary(arg => arg.Name, arg => arg.Type);
			NameByTypes = typeAndNames.ToDictionary(arg => arg.Type, arg => arg.Name);
		}


		public Type BindToType(string assemblyName, string typeName)
		{
			return TypesByName.TryGetValue(typeName, out var type) ? type : null;
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			NameByTypes.TryGetValue(serializedType, out typeName);
		}
	}
}