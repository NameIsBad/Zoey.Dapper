namespace Zoey.Dapper.Configuration
{

    /// <summary>
    /// 执行操作
    /// </summary>
    public class SqlCommandElement : SqlElement
    {
        public SqlCommandElement()
            : base()
        {
        }

        protected override SqlElement CreateInstance()
        {
            return new SqlCommandElement();
        }
    }
}
