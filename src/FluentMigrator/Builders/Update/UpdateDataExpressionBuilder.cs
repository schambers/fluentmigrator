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

using System.Collections.Generic;
using System.ComponentModel;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;

namespace FluentMigrator.Builders.Update
{
    public class UpdateDataExpressionBuilder : IUpdateSetOrInSchemaSyntax,
        IUpdateWhereSyntax
    {
        private readonly UpdateDataExpression _expression;
        private readonly IMigrationContext _context;

        public UpdateDataExpressionBuilder(UpdateDataExpression expression, IMigrationContext context)
        {
            _context = context;
            _expression = expression;
        }

        public IUpdateSetSyntax InSchema(string schemaName)
        {
            _expression.SchemaName = schemaName;
            return this;
        }

        public IUpdateWhereSyntax Set(object dataAsAnonymousType)
        {
            _expression.Set.Add(new ReflectedDataDefinition(dataAsAnonymousType));
            return this;
        }

        public void Where(object dataAsAnonymousType)
        {
            _expression.Where.Add(new ReflectedDataDefinition(dataAsAnonymousType));
        }

        public void AllRows()
        {
            _expression.IsAllRows = true;
        }
    }
}
