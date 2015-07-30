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

using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Builders.Select;
using FluentMigrator.Builders.Update;

namespace FluentMigrator
{
    public abstract class Migration : MigrationBase
    {
        public IDeleteExpressionRoot Delete
        {
            get { return new DeleteExpressionRoot(_context); }
        }

        public IExecuteExpressionRoot Execute
        {
            get { return new ExecuteExpressionRoot(_context); }
        }

        public IUpdateExpressionRoot Update
        {
            get { return new UpdateExpressionRoot(_context); }
        }

        public ISelectExpressionRoot Select
        {
            get { return new SelectExpressionRoot(_context); }
        }
    }
}
