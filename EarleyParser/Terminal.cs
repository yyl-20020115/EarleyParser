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
	public class Terminal : Symbol
	{
		public static readonly Terminal EOF = new Terminal(-1);

		protected static int[] CharsToInts(char[] cs) =>
			cs?.Select(ch => (int)ch).ToArray();

		protected readonly HashSet<int> cs = null;

		public Terminal(params char[] cs) : this(CharsToInts(cs)) { }

		public Terminal(params int[] cs) => this.cs = new HashSet<int>(cs ?? throw new ArgumentNullException(nameof(cs)));

		public override bool Equals(object obj) => (obj is Terminal other) && this.cs.SetEquals(other.cs);

		public override int GetHashCode() => cs.Sum();

		public virtual bool Contains(int ch) => cs.Contains(ch);

		public virtual int Count => cs.Count;

		public override string ToString() => string.Join("",
			cs.Select(i => i == -1 ? "(EOF)" : ((char)i).ToString())
			.OrderBy(i => i));
	}
}