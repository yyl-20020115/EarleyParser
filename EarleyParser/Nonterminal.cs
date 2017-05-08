// Copyright 2004 Dominic Cooney. All Rights Reserved.

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Earley
{
	public class Nonterminal : Symbol
	{
		protected readonly HashSet<Production> productions = null;

		public virtual int ProductionsCount => productions.Count;

		public virtual IEnumerable<Production> Productions => productions;

		public Nonterminal(params Production[] ps)
			=> this.productions = new HashSet<Production>(
				(ps == null || ps.Any(p => p == null))
				? throw new ArgumentNullException(nameof(ps))
				: ps);

		public virtual void Add(Production p) => productions.Add(p ?? throw new ArgumentNullException(nameof(p)));

		public virtual bool Contains(Production p) => productions.Contains(p ?? throw new ArgumentNullException(nameof(p)));
	}
}
