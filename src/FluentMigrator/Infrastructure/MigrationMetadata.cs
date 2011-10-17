#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;

namespace FluentMigrator.Infrastructure
{
	public class MigrationMetadata
	{
		private readonly Dictionary<string, object> _traits = new Dictionary<string, object>();

		public Type Type { get; set; }
		public long Version { get; set; }
		public bool Transactionless { get; set; }

		public object Trait(string name)
		{
			return _traits.ContainsKey(name) ? _traits[name] : null;
		}

		public bool HasTrait(string name)
		{
			return _traits.ContainsKey(name);
		}

		public void AddTrait(string name, object value)
		{
			_traits.Add(name, value);
		}
	}
}