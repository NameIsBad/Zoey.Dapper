namespace Zoey.Dapper.Configuration
{

    /// <summary>
    /// 查询操作
    /// </summary>
    public class SqlQueryElement : SqlElement
    {
        public SqlQueryElement()
            : base()
        {
        }

        protected override SqlElement CreateInstance()
        {
            return new SqlQueryElement();
        }
    }
}
