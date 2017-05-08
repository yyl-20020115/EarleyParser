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
using System.Linq;

namespace Earley
{
	public static class Util
	{
		public static Terminal AsTerminal(this char c)
			=> new Terminal(c);

		public static Terminal AsTerminal(this string text)
			=> new Terminal((text ?? throw new ArgumentNullException(nameof(text))).ToArray());

		public static Nonterminal AsNonterminal(this string name, params Production[] ps)
			=> new Nonterminal(ps) { Name = name };
	}
}