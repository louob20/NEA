using System;

namespace NEA_Technical_Solution
{
    class Queue // circular queue
    {
        private const int MAX_SIZE = 100;
        private string[] queue = new string[MAX_SIZE];
        private int frontPointer;
        private int backPointer;
        public int Length { get; private set; }
        public Queue()
        {
            frontPointer = 0;
            backPointer = -1;
            Length = 0;
        }
        public bool IsFull()
        {
            return Length == MAX_SIZE ? true : false;
        }
        public bool IsEmpty()
        {
            return Length == 0 ? true : false;
        }
        public void Enqueue(string data)
        {
            if (!this.IsFull())
            {
                backPointer++;
                Length++;
                queue[backPointer] = data;
            }
            else Console.WriteLine("No room to enqueue");
        }
        public string Dequeue()
        {
            if (!this.IsEmpty())
            {
                string data = queue[frontPointer];
                frontPointer++;
                Length--;
                return data;
            }
            else
            {
                Console.WriteLine("No item to dequeue");
                return null;
            }
        }
    }
}
