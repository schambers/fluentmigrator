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

using System.Collections.Generic;
using System.Linq;
using FluentMigrator.Model;

namespace FluentMigrator.Expressions
{
    public class CreateIndexExpression : MigrationExpressionBase
    {
        public virtual IndexDefinition Index { get; set; }

        public CreateIndexExpression()
        {
            Index = new IndexDefinition();
        }

        public override void ApplyConventions(IMigrationConventions conventions)
        {
            Index.ApplyConventions(conventions);
        }

        public override void CollectValidationErrors(ICollection<string> errors)
        {
            Index.CollectValidationErrors(errors);
        }

        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        public override IMigrationExpression Reverse()
        {
            return new DeleteIndexExpression { Index = Index.Clone() as IndexDefinition };
        }

        public override string ToString()
        {
            string columns = " (" + string.Join(", ", Index.Columns.Select(x => x.Name).ToArray()) + ")";

            if (Index.IncludeColumns.Count > 0)
                columns += " Include (" + string.Join(", ", Index.IncludeColumns.Select(x => x.Name).ToArray()) + ")";

            return base.ToString() + Index.TableName + columns;
        }
    }
}