
namespace DelayedRowFunctionAssembly
{


    public class DelayedRowFunction
    {


        private class DataRecordToReturn
        {
            public int RowNum { get; set; }
            public int YourColumn { get; set; }
            public string OtherColumn { get; set; }


            public DataRecordToReturn(int row, int column, string other)
            {
                this.RowNum = row;
                this.YourColumn = column;
                this.OtherColumn = other;
            } // End Constructor 


        } // End Class DataRecordToReturn 


        private static void SimulateDelay(int milliseconds)
        {
            // Create a timer
            System.Timers.Timer timer = new System.Timers.Timer(milliseconds);
            System.Threading.ManualResetEvent resetEvent = new System.Threading.ManualResetEvent(false);

            // Set up the timer
            timer.Elapsed += (sender, args) => resetEvent.Set();
            timer.AutoReset = false;
            timer.Start();

            // Wait for the timer to elapse
            resetEvent.WaitOne();
        } // End Sub SimulateDelay 


        public static void FillRow(object obj, 
            out System.Data.SqlTypes.SqlInt32 RowNum, 
            out System.Data.SqlTypes.SqlInt32 YourColumn, 
            out System.Data.SqlTypes.SqlString OtherColumn)
        {
            DataRecordToReturn record = (DataRecordToReturn)obj;

            RowNum = new System.Data.SqlTypes.SqlInt32(record.RowNum);
            YourColumn = new System.Data.SqlTypes.SqlInt32(record.YourColumn);
            OtherColumn = new System.Data.SqlTypes.SqlString(record.OtherColumn);
        } // End Sub FillRow 


        [Microsoft.SqlServer.Server.SqlFunction(
              DataAccess = Microsoft.SqlServer.Server.DataAccessKind.Read,
             FillRowMethodName = "FillRow", 
            TableDefinition = "RowNum INT, YourColumn INT, OtherColumn NVARCHAR(100)")
        ]
        public static System.Collections.IEnumerable DelayedSelect()
        {
            // Simulate a delay for each row
            System.TimeSpan delayTime = System.TimeSpan.FromSeconds(5); // 5-second delay

            // Sample data, replace this with your actual data retrieval logic
            System.Collections.Generic.List<DataRecordToReturn> data =
                new System.Collections.Generic.List<DataRecordToReturn>()
            {
                new DataRecordToReturn(1, 100, "First"),
                new DataRecordToReturn(2, 200, "Second"),
                new DataRecordToReturn(3, 300, "Third")
            };

            foreach (DataRecordToReturn row in data)
            {
                // Yield return each row
                yield return row;

                // Wait for the specified delay time
                System.Threading.Thread.Sleep(delayTime);
                // SimulateDelay((int)delayTime.TotalMilliseconds);
            } // Next row 

        } // End Function DelayedSelect


    } // End Class DelayedRowFunction 


} // End Namespace  
