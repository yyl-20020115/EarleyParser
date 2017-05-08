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
	public class State
	{
		protected readonly List<Item> items = new List<Item>();
		public virtual int ItemsCount => items.Count;

		public virtual Item this[int index]
			=> index >= 0 && index < items.Count
			? items[index]
			: throw new ArgumentOutOfRangeException(nameof(index));

		protected virtual Item this[Item item]
			=> this.items.Where(i => i.Equals(item)).FirstOrDefault();

		public State() { }

		public virtual Item Add(Item item)
		{
			if (this.Contains(item))
			{
				throw new InvalidOperationException("item already exists");
			}
			else
			{
				items.Add(item);

				return item;
			}
		}

		public virtual bool Contains(Item item) => items.Contains(item ?? throw new ArgumentNullException(nameof(item)));

		// gets items in this state with the dot before non-terminals that
		// contain the specified production
		public virtual IEnumerable<Item> GetItemsFor(Production production)
			=> production == null
				? throw new ArgumentNullException(nameof(production))
				: this.items.Where(
					item => !item.AtEnd
						&& item.Symbol is Nonterminal nt
						&& nt.Contains(production)
						);

		public virtual Item Import(Item item)
			=> (this[item ?? throw new ArgumentNullException(nameof(item))]
				 is Item existing)
			? existing.IsImportCompatible(item)
				? existing
				: throw new ArgumentException(nameof(item))
			: Add(item);
	}
}