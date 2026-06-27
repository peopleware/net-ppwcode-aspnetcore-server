// Copyright 2026 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace PPWCode.AspNetCore.Server.I.Transactional;

/// <summary>
///     Marks a controller or action as requiring (or explicitly not requiring) a database transaction.
/// </summary>
/// <remarks>
///     When applied to a class, the setting applies to all of its actions; an attribute on an individual action
///     overrides the class-level setting.
/// </remarks>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TransactionalAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TransactionalAttribute" /> class.
    /// </summary>
    /// <param name="transactional">
    ///     <see langword="true" /> to run the decorated controller or action within a transaction; otherwise
    ///     <see langword="false" />.
    /// </param>
    public TransactionalAttribute(bool transactional)
    {
        Transactional = transactional;
        IsolationLevel = IsolationLevel.Unspecified;
    }

    /// <summary>
    ///     Indicates whether the decorated controller or action should run within a transaction.
    /// </summary>
    public bool Transactional { get; }

    /// <summary>
    ///     The isolation level to use for the transaction. Defaults to <see cref="System.Data.IsolationLevel.Unspecified" />.
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; }
}
