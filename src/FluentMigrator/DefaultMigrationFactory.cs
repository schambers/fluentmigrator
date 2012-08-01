﻿#region License
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

namespace FluentMigrator
{
    /// <summary>Constructs the default implementations of FluentMigrator interfaces.</summary>
    public class DefaultMigrationFactory : IMigrationFactory
    {
        /// <summary>Get an object which defines default rules for migration mappings.</summary>
        /// <param name="applicationContext">The arbitrary application context passed to the task runner.</param>
        public virtual IMigrationConventions GetMigrationConventions(object applicationContext)
        {
            return new MigrationConventions();
        }
    }
}
