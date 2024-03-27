using System.Collections;

namespace webapi.Model
{
    public class NumberIterator : IEnumerable<NumberData>
    {
        private decimal number;
        public NumberIterator(decimal number)
        {
            this.number = number;
        }

        public IEnumerator<NumberData> GetEnumerator()
        {
            return new NumberData(number);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
