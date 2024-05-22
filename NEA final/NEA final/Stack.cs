using System;

namespace NEA_Technical_Solution
{
    public class Stack
    {
        private const int MAX_SIZE = 100;
        private object[] stack = new object[MAX_SIZE];
        private int topPointer;
        public int Length
        {
            get { return topPointer + 1; }
            private set { Length = value; }
        }
        public Stack()
        {
            topPointer = -1;
        }
        public Stack(object[] values)
        {
            values.CopyTo(stack, 0);
            topPointer = values.Length - 1;
        }
        public bool IsEmpty()
        {
            return topPointer == -1 ? true : false;
        }
        public void Push(object data)
        {
            topPointer++;
            stack[topPointer] = data;
        }
        public object Pop()
        {
            if (this.IsEmpty()) // nothing to pop
                return null;
            else
            {
                object poppedValue = stack[topPointer];
                topPointer--;
                return poppedValue;
            }
        }
        public object Peek()
        {
            if (this.IsEmpty()) // nothing to peek
                return null;
            else
                return stack[topPointer];
        }
    }
}
