// Project OldRod - A KoiVM devirtualisation utility.
// Copyright (C) 2019 Washi
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using OldRod.Core.Ast.IL;

namespace OldRod.Core.Memory
{
    public class DefaultFrameLayout : IFrameLayout
    {
        /* Vanilla KoiVM uses the following stack frame layout.
         * Assuming that n is the number of arguments, and m is the number of
         * local variables, vanilla KoiVM then uses the following stack layout:
         *
         *  | Offset | Value
         *  +--------+---------------
         *  | BP - n | Argument 0
         *  : ...    : ...
         *  | BP - 3 | Argument n-2
         *  | BP - 2 | Argument n-1
         *  | BP - 1 | Return Address
         *  | BP     | Caller's BP
         *  | BP + 1 | Local 0
         *  | BP + 2 | Local 1
         *  : ...    : ...
         *  | BP + m | Local m-1
         *
         * Locals are allocated by simply increasing the SP pointer, like many
         * other calling conventions do.
         */
        
        public const string Tag = "FrameLayout";

        public DefaultFrameLayout(int parameters, int locals, bool returnsValue)
        {
            for (int i = 0; i < parameters; i++)
                Parameters.Add(new FrameField(i, FrameFieldType.Parameter, true));
            for (int i = 0; i < locals; i++)
                Parameters.Add(new FrameField(i, FrameFieldType.LocalVariable, true));
            ReturnsValue = returnsValue;
        }
        
        public IList<FrameField> Parameters
        {
            get;
        } = new List<FrameField>();

        public IList<FrameField> Locals
        {
            get;
        } = new List<FrameField>();

        public bool ReturnsValue
        {
            get;
        }

        public FrameField Resolve(int offset)
        {
            if (offset <= -2)
            {
                int argumentIndex = Parameters.Count + offset + 1;
                if (argumentIndex < 0 || argumentIndex >= Parameters.Count)
                {
                    return new FrameField(argumentIndex, FrameFieldType.Parameter, false);
                }

                return Parameters[argumentIndex];
            }

            switch (offset)
            {
                case -1:
                    return new FrameField(0, FrameFieldType.ReturnAddress, true);
                case 0:
                    return new FrameField(0, FrameFieldType.CallersBasePointer, true);
                default:
                    int variableIndex = offset - 1;
                    return new FrameField(variableIndex, FrameFieldType.LocalVariable, true);
            }
        }
        
    }
}