namespace DistanceDrivenCalc
{
    internal class DayReport
    {

        public virtual int RowCount => 0;

        internal virtual string ColumnValue(int row, int column) => "";
    }
}