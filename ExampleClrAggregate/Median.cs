
namespace ExampleClrAggregate
{


    // https://stackoverflow.com/questions/1342898/function-to-calculate-median-in-sql-server
    // https://www.sqlservercentral.com/articles/a-genuine-use-for-a-sql-clr-aggregate
    // A median is only defined on ordered one-dimensional data, and is independent of any distance metric.
    // A more efficient implementation of medians can be obtained by using the quantile extension with quantile(some_column,0.5).
    [System.Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Microsoft.SqlServer.Server.Format.UserDefined, 
        IsInvariantToNulls = true, 
        IsInvariantToDuplicates = false, 
        IsInvariantToOrder = true, 
        MaxByteSize = -1, 
        IsNullIfEmpty = true
    )]
    public class Median 
        : Microsoft.SqlServer.Server.IBinarySerialize
    {
        private System.Collections.Generic.List<decimal> _items;


        public void Init()
        {
            _items = new System.Collections.Generic.List<decimal>();
        }


        public void Accumulate(System.Data.SqlTypes.SqlDecimal value)
        {
            if (!value.IsNull)
                _items.Add(value.Value);
        }

        public void Merge(Median other)
        {
            if (other._items != null)
                _items.AddRange(other._items);
        }


        public System.Data.SqlTypes.SqlDecimal Terminate()
        {
            if (_items.Count != 0)
            {
                decimal result;

                // _items = _items.OrderBy(i => i).ToList();
                // _items = System.Linq.Enumerable.ToList(System.Linq.Enumerable.OrderBy(_items, i => i));
                _items.Sort();
                
                if (_items.Count % 2 == 0)
                    result = ( (_items[(_items.Count / 2) - 1]) + (_items[_items.Count / 2]) ) / (decimal)2;
            else
                    result = _items[(_items.Count - 1) / 2];

                return new System.Data.SqlTypes.SqlDecimal(result);
            }
            else
                return new System.Data.SqlTypes.SqlDecimal();
        }


        public void Read(System.IO.BinaryReader r)
        {
            // deserialize it from a string
            string list = r.ReadString();
            _items = new System.Collections.Generic.List<decimal>();

            foreach (string value in list.Split(','))
            {
                decimal number;
                if (decimal.TryParse(value, out number))
                    _items.Add(number);
            } // Next value 

        } // End Sub Read 


        public void Write(System.IO.BinaryWriter w)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            for (int i = 0; i < _items.Count; i++)
            {
                if (i > 0)
                    builder.Append(",");
                builder.Append(_items[i].ToString());
            } // Next i 

            w.Write(builder.ToString());
            builder.Clear();
            builder = null;
        } // End Sub Write 


    } // End Class 


} // End Namespace 
